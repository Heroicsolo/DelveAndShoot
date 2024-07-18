using Assets.FantasyInventory.Scripts.Enums;
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

        private List<LootInfo> lootInfos = new List<LootInfo>();

        private ItemsDropState itemsDropState;

        public GameObject GetGameObject()
        {
            return gameObject;
        }

        public List<LootInfo> GetLootInfos()
        {
            return lootInfos;
        }

        public void GenerateRandomDrop(LootInfo lootInfo, Vector3 worldPosition)
        {
            List<DropResult> randomDrop = lootInfo.GetRandomDrop(itemsDropState);

            //TODO: spawn loot in world

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