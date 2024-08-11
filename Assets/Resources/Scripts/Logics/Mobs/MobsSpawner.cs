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
        [SerializeField] [Min(0)] private float spawnPeriod = 0;
        [SerializeField] private float spawnYOffset = 0f;

        [Inject] private RuntimeDungeonGenerator runtimeDungeonGenerator;

        private WeightedChoser<Mob> spawnChoser;
        private Coroutine spawnCoroutine = null;

        private void Spawn()
        {
            Mob chosenMob = spawnChoser.Chose();
            Mob mobInstance = PoolSystem.GetInstanceAtPosition(chosenMob, chosenMob.GetName(), transform.position + Vector3.up * spawnYOffset);
            mobInstance.Activate();
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

            var spawnDict = new Dictionary<Mob, float>(spawnList.Select(i => new KeyValuePair<Mob, float>(i.Value, i.Weight)));
            spawnChoser = new WeightedChoser<Mob>(spawnDict);
            if (spawnPeriod <= 0)
                Spawn();
            else
                spawnCoroutine = StartCoroutine(SpawnCoroutine());
        }

        void Start()
        {
            SystemsManager.InjectSystemsTo(this);

            StartCoroutine(BeginSpawningProccess());
        }
    }
}