using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MobsSpawner : MonoBehaviour
{
    [SerializeField]
    private List<MobSpawnInfo> spawnList = new();
    private List<int> comulateWeight;
    private bool spawnListSorted = false;
    [Min(0)]
    [SerializeField]
    private float spawnPeriod = 0;
    
    private MobSpawnInfo ChooseMob()
    {
        if (!spawnListSorted)
            Prepare();
        int target = Random.Range(0, comulateWeight.Last());
        int guess = 0;
        while (true)
        {
            if (comulateWeight[guess] > target)
                return spawnList[guess];
            int hopDist = target - comulateWeight[guess];
            guess += 1 + hopDist / spawnList[guess].Weight;
        }
    }

    private void Spawn()
    {
        MobSpawnInfo chosenInfo = ChooseMob();
        var chosenMob = chosenInfo.Mob;
        Instantiate(chosenMob, this.transform, true);
    }

    private IEnumerator SpawnCoroutine()
    {
        Spawn();
        yield return new WaitForSeconds(spawnPeriod);
    }

    private void Prepare()
    {
        spawnList = spawnList.OrderByDescending(i => i.Weight).ToList();
        comulateWeight = spawnList.Select(i => i.Weight).ComulativeSum().ToList();
        spawnListSorted = true;
    }

    void Start()
    {
        Prepare();
        if (spawnPeriod <= 0)
            Spawn();
        else
            StartCoroutine(SpawnCoroutine());
    }

    struct MobSpawnInfo
    {
        public GameObject Mob;
        public int Weight;
    }
}
