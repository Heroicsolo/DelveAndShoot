using System;
using System.Collections.Generic;
using Assets.FantasyInventory.Scripts.Enums;
using Heroicsolo.Inventory;
using Heroicsolo.Logics;

namespace Assets.FantasyInventory.Scripts.Data
{
    /// <summary>
    /// Represents generic item params (common for all items).
    /// </summary>
    [Serializable]
    public class ItemParams
    {
        public ItemId ID;
        public ItemType Type;
        public string Title;
        public ItemRarity Rarity;
        public List<ItemTag> Tags = new List<ItemTag>();
        public List<CharacterStatModifier> StatModifiers = new List<CharacterStatModifier>();
        public List<Property> Properties = new List<Property>();
        public int Price;
        public float Weight;
        public PickupItem PickupItem;

        public bool TryGetProperty(PropertyId propertyId, out Property property)
        {
            property = Properties.Find(p => p.Id == propertyId);

            return property != null;
        }
    }

    public enum ItemRarity
    {
        Common = 0,
        Rare = 1,
        Epic = 2,
        Legendary = 3
    }
}