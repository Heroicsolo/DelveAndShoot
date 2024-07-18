using Assets.FantasyInventory.Scripts.Enums;
using Heroicsolo.Utils;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Heroicsolo.Inventory
{
    [CreateAssetMenu(fileName = "New LootInfo", menuName = "Loot Info", order = 51)]
    public class LootInfo : ScriptableObject
    {
        [ScriptableObjectId][SerializeField] private string lootId;
        [SerializeField] private List<LootUnit> lootUnits = new List<LootUnit>();

        public string LootId => lootId;
        public List<LootUnit> LootUnits => lootUnits;

        public List<DropResult> GetRandomDrop(ItemsDropState itemsDropState)
        {
            List<DropResult> dropResults = new();

            foreach (var lootUnit in lootUnits)
            {
                int dropStateIdx = itemsDropState.DropStates.FindIndex(item => item.ItemId == lootUnit.ItemId);

                ItemDropState itemDropState = itemsDropState.DropStates[dropStateIdx];

                //Unluckiness/Luckiness protection
                float currDropChance = lootUnit.DropChance + itemDropState.DropChanceModifier;

                if (UnityEngine.Random.value <= currDropChance)
                {
                    int maxAmount = Mathf.Max(lootUnit.MinAmount, lootUnit.MaxAmount);

                    dropResults.Add(new DropResult
                    {
                        ItemId = lootUnit.ItemId,
                        Amount = UnityEngine.Random.Range(lootUnit.MinAmount, maxAmount)
                    });

                    //We reset unluckiness protection and do *luckiness* protection instead
                    itemDropState.DropChanceModifier = currDropChance - 1f;
                    itemsDropState.DropStates[dropStateIdx] = itemDropState;
                }
                else
                {
                    //Unluckiness protection
                    itemDropState.DropChanceModifier += lootUnit.DropChance;
                }
            }

            return dropResults;
        }
    }

    [Serializable]
    public struct LootUnit
    {
        public ItemId ItemId;
        [Min(1)] public int MinAmount;
        [Min(1)] public int MaxAmount;
        [Range(0f, 1f)] public float DropChance;
    }

    [Serializable]
    public struct DropResult
    {
        public ItemId ItemId;
        public int Amount;
    }
}