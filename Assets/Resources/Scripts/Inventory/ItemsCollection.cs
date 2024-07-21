using Assets.FantasyInventory.Scripts.Data;
using Assets.FantasyInventory.Scripts.Enums;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Heroicsolo.Scripts.Inventory
{
    public class ItemsCollection : MonoBehaviour
    {
        public List<ItemParams> AvailableItems = new List<ItemParams>();

        public static Dictionary<ItemId, ItemParams> ItemsParams = new Dictionary<ItemId, ItemParams>();

        private static ItemsCollection instance;

        public ItemsCollection GetInstance()
        {
            return instance;
        }

        public static Color GetRarityColor(ItemRarity itemRarity)
        {
            return itemRarity switch
            {
                ItemRarity.Common => Color.white,
                ItemRarity.Rare => Color.blue,
                ItemRarity.Epic => Color.magenta,
                ItemRarity.Legendary => Color.yellow,
                _ => Color.white,
            };
        }

        private void Awake()
        {
            if (instance == null)
            {
                instance = this;
                AvailableItems.ForEach(item => ItemsParams.Add(item.ID, item));
            }
        }
    }
}