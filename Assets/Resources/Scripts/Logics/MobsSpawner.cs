using Heroicsolo.Scripts.Utils;
using Heroicsolo.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Heroicsolo.Scripts.Logics
{
    public class MobsSpawner : MonoBehaviour
    {
        [SerializeField]
        private List<ValueWeight<Mob>> spawnList = new();
        [Min(0)]
        [SerializeField]
        private float spawnPeriod = 0;

        private WeightedChoser<Mob> spawnChoser;
        private Coroutine spawnCoroutine = null;

        private void Spawn()
        {
            Mob chosenMob = spawnChoser.Chose();
            Mob mobInstance = PoolSystem.GetInstanceAtPosition(chosenMob, chosenMob.GetName(), transform.position);
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

        void Start()
        {
            var spawnDict = new Dictionary<Mob, float>(spawnList.Select(i => new KeyValuePair<Mob, float>(i.Value, i.Weight)));
            spawnChoser = new WeightedChoser<Mob>(spawnDict);
            if (spawnPeriod <= 0)
                Spawn();
            else
                spawnCoroutine = StartCoroutine(SpawnCoroutine());
        }
    }
}