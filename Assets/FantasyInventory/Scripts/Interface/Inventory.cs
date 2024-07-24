using System.Collections.Generic;
using System.Linq;
using Assets.FantasyInventory.Scripts.Data;
using Assets.FantasyInventory.Scripts.Enums;
using Assets.FantasyInventory.Scripts.Interface.Elements;
using Heroicsolo.DI;
using Heroicsolo.Inventory;
using Heroicsolo.Scripts.Inventory;
using Heroicsolo.Scripts.Logics;
using Heroicsolo.Scripts.Player;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.FantasyInventory.Scripts.Interface
{
    /// <summary>
    /// High-level inventory inverface.
    /// </summary>
    public class Inventory : ItemWorkspace
    {
        public Equipment Equipment;
        public ScrollInventory Bag;
        public Button EquipButton;
        public Button RemoveButton;
        public AudioSource AudioSource;
        public AudioClip EquipSound;
        public AudioClip RemoveSound;
        public Text StatsLabel;
        public Text StatsValuesLabel;

        [Inject] private ICharacterStatsManager characterStatsManager;
        [Inject] private IInventoryManager inventoryManager;

        private PlayerController playerController;

        public void OnEnable()
        {
            SystemsManager.InjectSystemsTo(this);

            FillInventory();
        }

        protected void Start()
        {
            SystemsManager.InjectSystemsTo(this);

            playerController ??= FindObjectOfType<PlayerController>();

            Reset();
            EquipButton.interactable = RemoveButton.interactable = false;

            // TODO: Assigning static callbacks. Don't forget to set null values when UI will be closed. You can also use events instead.
            InventoryItem.OnItemSelected = SelectItem;
            InventoryItem.OnDragStarted = SelectItem;
            InventoryItem.OnDragCompleted = InventoryItem.OnDoubleClick = item => { if (Bag.Items.Contains(item)) Equip(); else Unequip(); };
        }

        public void SelectItem(Item item)
        {
            SelectItem(item.Id);
        }

        public void SelectItem(ItemId itemId)
        {
            SelectedItem = itemId;
            SelectedItemParams = ItemsCollection.ItemsParams[itemId];
            ItemInfo.Initialize(SelectedItem, SelectedItemParams);
            Refresh();
        }

        public void Equip()
        {
            var equipped = Equipment.Items.LastOrDefault(i => i.Params.Type == SelectedItemParams.Type);

            if (equipped != null)
            {
                AutoRemove(SelectedItemParams.Type, Equipment.Slots.Count(i => i.ItemType == SelectedItemParams.Type));
            }

            MoveItem(SelectedItem, Bag, Equipment);

            inventoryManager.EquipItem(SelectedItem);

            AudioSource.PlayOneShot(EquipSound);

            if (SelectedItemParams.Type == ItemType.Weapon)
            {
                playerController.EquipWeapon(SelectedItem);
            }
        }

        public void Unequip()
        {
            MoveItem(SelectedItem, Equipment, Bag);

            inventoryManager.UnequipItem(SelectedItem);

            SelectItem(Equipment.Items.FirstOrDefault(i => i.Id == SelectedItem) ?? Bag.Items.Single(i => i.Id == SelectedItem));
            AudioSource.PlayOneShot(RemoveSound);
        }

        public override void Refresh()
        {
            if (CanEquip())
            {
                EquipButton.interactable = Bag.Items.Any(i => i.Id == SelectedItem)
                    && Equipment.Slots.Count(i => i.ItemType == SelectedItemParams.Type) > Equipment.Items.Count(i => i.Id == SelectedItem);
                RemoveButton.interactable = Equipment.Items.Any(i => i.Id == SelectedItem);
                ItemInfo.Price.enabled = !SelectedItemParams.Tags.Contains(ItemTag.NotForSale);
            }
            else
            {
                EquipButton.interactable = RemoveButton.interactable = false;
            }

            FillCharacterStats();
        }

        private void FillInventory()
        {
            List<Item> inventoryItems = inventoryManager.GetInventoryItems();
            List<Item> equippedItems = inventoryManager.GetEquippedItems();

            Bag.Initialize(ref inventoryItems);
            Equipment.Initialize(ref equippedItems);
        }

        private void FillCharacterStats()
        {
            string statsNames = string.Empty;
            string statsValues = string.Empty;

            foreach (var stat in playerController.GetCharacterStats())
            {
                if (stat.MaxValue > 0f)
                {
                    statsNames += ItemInfo.SplitName(stat.StatType.ToString()) + "\n";
                    statsValues += stat.MaxValue.ToString() + "\n";
                }
            }
        }

        private bool CanEquip()
        {
            return Equipment.Slots.Any(i => i.ItemType == SelectedItemParams.Type && i.ItemTags.All(j => SelectedItemParams.Tags.Contains(j)));
        }

        /// <summary>
        /// Automatically removes items if target slot is busy.
        /// </summary>
        private void AutoRemove(ItemType itemType, int max)
        {
            var items = Equipment.Items.Where(i => i.Params.Type == itemType).ToList();
            long sum = 0;

            foreach (var p in items)
            {
                sum += p.Count;
            }

            if (sum == max)
            {
                MoveItem(items.LastOrDefault(i => i.Id != SelectedItem) ?? items.Last(), Equipment, Bag);
            }
        }
    }
}