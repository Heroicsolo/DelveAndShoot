using Assets.FantasyInventory.Scripts.Data;
using Assets.FantasyInventory.Scripts.Enums;
using Heroicsolo.Scripts.Inventory;
using Heroicsolo.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace Heroicsolo.Inventory
{
    public class LootManager : MonoBehaviour, ILootManager
    {
        private const string LootStatePrefsKey = "LootSystemState";

        [SerializeField] [Min(0f)] private float dropRadius = 3f;
        [SerializeField] private PooledParticleSystem commonLootEffect;
        [SerializeField] private PooledParticleSystem rareLootEffect;
        [SerializeField] private PooledParticleSystem epicLootEffect;
        [SerializeField] private PooledParticleSystem legendaryLootEffect;

        private List<LootInfo> lootInfos = new();
        private Dictionary<string, LootInfo> lootInfosByIds = new();

        private ItemsDropState itemsDropState;

        public GameObject GetGameObject()
        {
            return gameObject;
        }

        public List<LootInfo> GetLootInfos()
        {
            return lootInfos;
        }

        public LootInfo GetLootInfo(string lootId)
        {
            if (lootInfosByIds.ContainsKey(lootId))
            {
                return lootInfosByIds[lootId];
            }

            return null;
        }

        public void GenerateRandomDrop(string lootId, Vector3 worldPosition)
        {
            if (string.IsNullOrEmpty(lootId))
            {
                return;
            }

            GenerateRandomDrop(GetLootInfo(lootId), worldPosition);
        }    

        public void GenerateRandomDrop(LootInfo lootInfo, Vector3 worldPosition)
        {
            if (lootInfo == null)
            {
                return;
            }

            List<DropResult> randomDrop = lootInfo.GetRandomDrop(itemsDropState);

            foreach (var dropUnit in randomDrop)
            {
                PickupItem itemPrefab = ItemsCollection.ItemsParams[dropUnit.ItemId].PickupItem;

                Vector2 spread = UnityEngine.Random.insideUnitCircle * dropRadius;
                Vector3 spread3D = new(spread.x, 0f, spread.y);
                Vector3 dropPosition = worldPosition + spread3D;

                if (itemPrefab != null)
                {
                    PickupItem itemInstance = PoolSystem.GetInstanceAtPosition(itemPrefab, itemPrefab.GetName(), worldPosition);
                    
                    PooledParticleSystem rarityEffect = commonLootEffect;

                    switch (ItemsCollection.ItemsParams[dropUnit.ItemId].Rarity)
                    {
                        case ItemRarity.Rare:
                            rarityEffect = rareLootEffect; break;
                        case ItemRarity.Epic:
                            rarityEffect = epicLootEffect; break;
                        case ItemRarity.Legendary:
                            rarityEffect = legendaryLootEffect; break;
                    }

                    PoolSystem.GetInstanceAtPosition(rarityEffect, rarityEffect.GetName(), itemInstance.transform.position, itemInstance.transform);

                    itemInstance.FlyToPoint(dropPosition);
                }
            }

            SaveState();
        }

        private void LoadState()
        {
            string savesString = PlayerPrefs.GetString(LootStatePrefsKey, "");

            if (!string.IsNullOrEmpty(savesString))
            {
                itemsDropState = JsonUtility.FromJson<ItemsDropState>(savesString);
            }
            else
            {
                itemsDropState = new ItemsDropState();
            }
        }

        private void SaveState()
        {
            string savesString = JsonUtility.ToJson(itemsDropState);
            PlayerPrefs.SetString(LootStatePrefsKey, savesString);
        }

        private void Awake()
        {
            lootInfos.AddRange(Resources.LoadAll("Data/Loot", typeof(LootInfo)));

            lootInfos.ForEach(lootInfo => lootInfosByIds.Add(lootInfo.LootId, lootInfo));

            LoadState();
        }
    }

    [Serializable]
    public struct ItemDropState
    {
        public ItemId ItemId;
        public float DropChanceModifier;
    }

    [Serializable]
    public class ItemsDropState
    {
        public List<ItemDropState> DropStates;

        public ItemsDropState()
        {
            DropStates = new List<ItemDropState>();

            foreach (ItemId itemId in Enum.GetValues(typeof(ItemId)))
            {
                DropStates.Add(new ItemDropState { ItemId = itemId, DropChanceModifier = 0f });
            }
        }
    }
}