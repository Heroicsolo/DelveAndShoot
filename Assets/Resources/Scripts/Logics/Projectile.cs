using Heroicsolo.DI;
using Heroicsolo.Utils;
using UnityEngine;

namespace Heroicsolo.Logics
{
    public class Projectile : MonoBehaviour, IPooledObject
    {
        private const string EnemyTag = "Enemy";
        private const string PlayerTag = "Player";
        private const string TriggerTag = "Trigger";

        [SerializeField] [Min(0f)] private float damageMin = 5f;
        [SerializeField] [Min(0f)] private float damageMax = 10f;
        [SerializeField] [Min(0f)] private float speed = 10f;
        [SerializeField] private bool isGuiding = false;
        [SerializeField] private bool isDynamicGuiding = false;
        [SerializeField] private bool isSingleGuiding = false;
        [SerializeField] private float dynamicGuidingInterval = 0.5f;
        [SerializeField] [Range(0f, 1f)] private float guidingForce = 0.4f;
        [SerializeField] [Min(0f)] private float guidingSpeed = 4f;
        [SerializeField] [Range(0f, 180f)] private float guidingAngle = 30f;
        [SerializeField] private float guidingMaxDist = 200f;
        [SerializeField] [Min(0f)] private float lifetime = 3f;
        [SerializeField] private ParticleSystem explodeEffect;
        [SerializeField] private bool dieAfterTargetDistance = false;
        [SerializeField] private bool explodeOnCollision = true;
        [SerializeField] private float pullForce = 0f;

        [Inject] private ITeamsManager teamsManager;

        private float damageAmount;
        private IHittable owner;
        private Transform target;
        private IHittable targetHittable;
        private TrailRenderer trailRenderer;

        private bool targetSelected = false;
        private float timeToDeath = 3f;
        private float timeToChangeTarget = 0f;
        private float damageModifier = 1f;
        private float distanceLeft;
        private Quaternion m_targetRot;
        private bool isLaunched;

        private float Damage => damageModifier * Random.Range(damageMin, damageMax);

        public GameObject GetGameObject()
        {
            return gameObject;
        }

        public string GetName()
        {
            return gameObject.name;
        }

        public void SetName(string name)
        {
            gameObject.name = name;
        }

        public void SetOwner(IHittable owner)
        {
            this.owner = owner;
        }

        public void Launch(IHittable target, float damageModifier = 1f)
        {
            targetHittable = target;
            if (targetHittable != null)
            {
                this.target = targetHittable.GetTransform();

                if (dieAfterTargetDistance)
                {
                    distanceLeft = Vector3.Distance(transform.position, this.target.position);
                }
            }
            targetSelected = true;
            isLaunched = true;
            this.damageModifier = damageModifier;
        }

        private void FindNewGuidingTarget()
        {
            targetHittable = teamsManager.GetNearestMemberOfOppositeTeams(owner, guidingMaxDist);

            if (targetHittable != null)
            {
                target = targetHittable.GetTransform();

                timeToChangeTarget = dynamicGuidingInterval;

                targetSelected = true;
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.transform != owner.GetTransform() && (other.CompareTag(PlayerTag) || other.CompareTag(EnemyTag)))
            {
                IHittable hittable = other.GetComponentInParent<IHittable>();

                if (hittable != null && !hittable.IsDead())
                {
                    hittable.GetDamage(Damage);

                    if (explodeOnCollision)
                    {
                        Explode();
                    }
                }
            }
            else if (explodeOnCollision && other.transform != owner.GetTransform() && !other.CompareTag(TriggerTag))
            {
                Explode();
            }
        }

        public void Explode()
        {
            if (explodeEffect)
            {
                Instantiate(explodeEffect, transform.position, Quaternion.identity);
            }

            timeToDeath = lifetime;
            target = null;
            targetSelected = false;
            timeToChangeTarget = 0f;

            PoolSystem.ReturnToPool(this);
        }

        private void Update()
        {
            if (!isLaunched)
            {
                return;
            }

            if (dieAfterTargetDistance)
            {
                distanceLeft -= speed * Time.deltaTime;

                if (distanceLeft <= 0f)
                {
                    Explode();
                    return;
                }
            }

            timeToDeath -= Time.deltaTime;

            if (timeToChangeTarget > 0f)
                timeToChangeTarget -= Time.deltaTime;

            if (timeToDeath <= 0f)
            {
                Explode();
                return;
            }

            if (isGuiding && guidingForce > 0f && (!isSingleGuiding || (isSingleGuiding && !targetSelected)))
            {
                if (timeToChangeTarget <= 0f || target == null)
                {
                    if (isDynamicGuiding)
                        FindNewGuidingTarget();
                    else if (!isDynamicGuiding && target == null && !targetSelected)
                        FindNewGuidingTarget();
                }
            }

            if (isGuiding && guidingForce > 0f && target != null && target.Distance(transform) < guidingMaxDist)
            {
                Vector3 dir = (target.position - transform.position).normalized;

                Vector3 localDir = transform.InverseTransformDirection(dir);

                if (localDir.z > 0f && Mathf.Atan2(localDir.z, localDir.x) < guidingAngle)
                {
                    Vector3 guidedDir = dir * guidingForce + transform.forward * (1f - guidingForce);
                    m_targetRot = Quaternion.LookRotation(guidedDir, Vector3.up);
                    transform.rotation = Quaternion.Lerp(transform.rotation, m_targetRot, guidingSpeed * Time.deltaTime);
                }
                else
                {
                    target = null;
                }
            }
            else
            {
                target = null;
            }

            transform.Translate(transform.forward * speed * Time.deltaTime, Space.World);
        }

        private void Start()
        {
            SystemsManager.InjectSystemsTo(this);
        }

        private void OnEnable()
        {
            timeToDeath = lifetime;

            target = null;
            targetSelected = false;
            timeToChangeTarget = 0f;
            isLaunched = false;
        }
    }
}