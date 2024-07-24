using Assets.FantasyInventory.Scripts.Data;
using Assets.FantasyInventory.Scripts.Enums;
using Heroicsolo.Scripts.Inventory;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Heroicsolo.Inventory
{
    public class InventoryManager : MonoBehaviour, IInventoryManager
    {
        private const string InventoryStatePrefsKey = "InventoryState";
        private const string EquipmentStatePrefsKey = "EquipmentState";

        [SerializeField] private List<Item> defaultInventoryItems = new();
        [SerializeField] private List<Item> defaultEquippedItems = new();

        private InventoryState inventoryState;

        public GameObject GetGameObject()
        {
            return gameObject;
        }

        public List<Item> GetInventoryItems()
        {
            return inventoryState.InventorySlots;
        }

        public List<Item> GetEquippedItems()
        {
            return inventoryState.EquipmentSlots;
        }

        public void AddItem(ItemId itemId, int amount)
        {
            inventoryState.AddItem(itemId, amount);
            SaveState();
        }

        public void EquipItem(ItemId itemId)
        {
            inventoryState.EquipItem(itemId);
            SaveState();
        }

        public void UnequipItem(ItemId itemId)
        {
            inventoryState.UnequipItem(itemId);
            SaveState();
        }

        public void RemoveItem(ItemId itemId, int amount)
        {
            inventoryState.RemoveItem(itemId, amount);
            SaveState();
        }

        public int GetItemCount(ItemId itemId)
        {
            return inventoryState.GetItemAmount(itemId);
        }

        public float GetItemsWeight()
        {
            return inventoryState.GetItemsWeight();
        }

        private void SaveState()
        {
            string stateString = JsonUtility.ToJson(inventoryState);
            PlayerPrefs.SetString(InventoryStatePrefsKey, stateString);
        }

        private void LoadState()
        {
            string stateString = PlayerPrefs.GetString(InventoryStatePrefsKey, string.Empty);

            if (string.IsNullOrEmpty(stateString))
            {
                inventoryState = new InventoryState(defaultInventoryItems, defaultEquippedItems);
            }
            else
            {
                inventoryState = JsonUtility.FromJson<InventoryState>(stateString);
            }
        }

        private void Awake()
        {
            LoadState();
        }
    }

    [Serializable]
    public class InventoryState
    {
        public List<Item> InventorySlots;
        public List<Item> EquipmentSlots;

        public InventoryState()
        {
            InventorySlots = new List<Item>();
            EquipmentSlots = new List<Item>();
        }

        public InventoryState(List<Item> defaultInventoryItems, List<Item> defaultEquippedItems)
        {
            InventorySlots = new List<Item>(defaultInventoryItems);
            EquipmentSlots = new List<Item>(defaultEquippedItems);
        }

        public void AddItem(ItemId itemId, int amount)
        {
            var target = InventorySlots.SingleOrDefault(i => i.Id == itemId);

            if (target == null)
            {
                InventorySlots.Add(new Item(itemId, amount));
            }
            else
            {
                target.Count += amount;
            }
        }

        public void RemoveItem(ItemId itemId, int amount)
        {
            var target = InventorySlots.Single(i => i.Id == itemId);

            if (target.Count > amount)
            {
                target.Count -= amount;
            }
            else
            {
                InventorySlots.Remove(target);
            }
        }

        public void EquipItem(ItemId itemId)
        {
            EquipmentSlots.Add(new Item
            {
                Id = itemId,
                Count = 1
            });

            RemoveItem(itemId, 1);
        }

        public void UnequipItem(ItemId itemId)
        {
            var target = EquipmentSlots.Single(i => i.Id == itemId);

            EquipmentSlots.Remove(target);

            AddItem(itemId, 1);
        }

        public int GetItemAmount(ItemId itemId)
        {
            int amount = 0;

            InventorySlots.ForEach(slot =>
            {
                if (slot.Id == itemId)
                {
                    amount += slot.Count;
                }
            });

            return amount;
        }

        public float GetItemsWeight()
        {
            float weight = 0f;

            InventorySlots.ForEach(slot => 
            {
                weight += ItemsCollection.ItemsParams[slot.Id].Weight;
            });

            return weight;
        }
    }
}