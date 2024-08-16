using Heroicsolo.DI;
using Heroicsolo.Utils;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

namespace Heroicsolo.Logics.Mobs
{
    public class MobsSpawner : MonoBehaviour
    {
        [SerializeField] private List<ValueWeight<Mob>> spawnList = new();
        [SerializeField] private bool autoSpawn = true;
        [SerializeField] [Min(0)] private float spawnPeriod = 0;
        [SerializeField] private float spawnYOffset = 0f;
        [SerializeField] private ParticleSystem spawnEffect;
        [SerializeField] private bool followPlayerInstantly = false;
        [SerializeField] private bool isMinionsSpawn = false;

        [Inject] private RuntimeDungeonGenerator runtimeDungeonGenerator;

        private WeightedChoser<Mob> spawnChoser;
        private Coroutine spawnCoroutine = null;
        private NavMeshPath testPath = null;
        private NavMeshHit navMeshHit;

        public void SpawnWithDelay(float delay = 1f)
        {
            if (spawnEffect != null)
            {
                spawnEffect.Play();
            }

            Invoke(nameof(Spawn), delay);
        }

        public void Spawn()
        {
            Vector3 spawnPos = transform.position + Vector3.up * spawnYOffset;

            Mob chosenMob = spawnChoser.Chose();

            bool isStationary = chosenMob.GetDefaultStrategy() is StationaryMobStrategy;

            Mob mobInstance = isStationary ? 
                PoolSystem.GetInstanceAtPosition(chosenMob, chosenMob.GetName(), spawnPos) : 
                PoolSystem.GetInstance(chosenMob, chosenMob.GetName());

            if (!isStationary)
            {
                if (NavMesh.SamplePosition(spawnPos, out navMeshHit, 10f, NavMesh.AllAreas))
                {
                    spawnPos = navMeshHit.position;
                }

                chosenMob.GetComponent<NavMeshAgent>().Warp(spawnPos);
            }

            mobInstance.Activate();

            if (followPlayerInstantly && mobInstance is GenericMob genericMob)
            {
                genericMob.FollowPlayer();
            }
        }

        private IEnumerator SpawnCoroutine()
        {
            while (true)
            {
                Spawn();
                yield return new WaitForSeconds(spawnPeriod);
            }
        }

        private IEnumerator BeginSpawningProccess()
        {
            yield return new WaitUntil(() => runtimeDungeonGenerator.IsDungeonReady());

            if (spawnPeriod <= 0)
                Spawn();
            else
                spawnCoroutine = StartCoroutine(SpawnCoroutine());
        }

        void Start()
        {
            SystemsManager.InjectSystemsTo(this);

            testPath = new();

            var spawnDict = new Dictionary<Mob, float>(spawnList.Select(i => new KeyValuePair<Mob, float>(i.Value, i.Weight)));
            spawnChoser = new WeightedChoser<Mob>(spawnDict);

            if (autoSpawn)
            {
                StartCoroutine(BeginSpawningProccess());
            }
        }
    }
}