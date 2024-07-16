using System;
using Assets.FantasyInventory.Scripts.Enums;
using Heroicsolo.Scripts.Inventory;

namespace Assets.FantasyInventory.Scripts.Data
{
    /// <summary>
    /// Represents item object for storing with game profile (please note, that item params are stored separately in params database).
    /// </summary>
    [Serializable]
    public class Item
    {
        public ItemId Id;
        public int Count;

        public ItemParams Params => ItemsCollection.ItemsParams[Id];

        public Item()
        {
        }

        public Item(ItemId id, int count)
        {
            Id = id;
            Count = count;
        }
    }
}