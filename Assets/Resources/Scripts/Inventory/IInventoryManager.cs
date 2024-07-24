using Assets.FantasyInventory.Scripts.Data;
using Assets.FantasyInventory.Scripts.Enums;
using Heroicsolo.DI;
using System.Collections.Generic;

namespace Heroicsolo.Inventory
{
    public interface IInventoryManager : ISystem
    {
        List<Item> GetInventoryItems();
        List<Item> GetEquippedItems();
        void AddItem(ItemId itemId, int amount);
        void RemoveItem(ItemId itemId, int amount);
        int GetItemCount(ItemId itemId);
        float GetItemsWeight();
        void EquipItem(ItemId itemId);
        void UnequipItem(ItemId itemId);
    }
}