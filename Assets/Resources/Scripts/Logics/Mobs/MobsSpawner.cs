using Heroicsolo.DI;
using Heroicsolo.Utils;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

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
            Mob chosenMob = spawnChoser.Chose();
            Mob mobInstance = PoolSystem.GetInstanceAtPosition(chosenMob, chosenMob.GetName(), transform.position + Vector3.up * spawnYOffset);
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

            var spawnDict = new Dictionary<Mob, float>(spawnList.Select(i => new KeyValuePair<Mob, float>(i.Value, i.Weight)));
            spawnChoser = new WeightedChoser<Mob>(spawnDict);

            if (autoSpawn)
            {
                StartCoroutine(BeginSpawningProccess());
            }
        }
    }
}