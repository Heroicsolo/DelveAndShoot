using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Assets.FantasyInventory.Scripts.Data;
using Assets.FantasyInventory.Scripts.Enums;
using Heroicsolo.Scripts.Inventory;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.FantasyInventory.Scripts.Interface.Elements
{
    /// <summary>
    /// Represents item when it was selected. Displays icon, name, price and properties.
    /// </summary>
    public class ItemInfo : MonoBehaviour
    {
        private const float FrameOpacity = 0.3f;

        public Text Name;
        public Text Description;
        public Text Price;
        public Image Icon;
        public Image Frame;

        public void Reset()
        {
            Name.text = Description.text = Price.text = null;
            Icon.sprite = ImageCollection.Instance.DefaultItemIcon;
            Color color = Color.white;
            color.a = FrameOpacity;
            Frame.color = color;
        }

        public void Initialize(ItemId itemId, ItemParams itemParams, bool shop = false)
        {
            Icon.sprite = ImageCollection.Instance.GetIcon(itemId);
            Name.text = string.IsNullOrEmpty(itemParams.Title) ? SplitName(itemId.ToString()) : itemParams.Title;
            Description.text = $"Here will be {itemId} description soon...";

            Color color = ItemsCollection.GetRarityColor(ItemsCollection.ItemsParams[itemId].Rarity);
            color.a = FrameOpacity;
            Frame.color = color;

            if (itemParams.Tags.Contains(ItemTag.NotForSale))
            {
                Price.text = null;
            }
            else if (shop)
            {
                Price.text = $"Buy price: {itemParams.Price}G{Environment.NewLine}Sell price: {itemParams.Price / Shop.SellRatio}$";
            }
            else
            {
                Price.text = $"Sell price: {itemParams.Price / Shop.SellRatio}$";
            }

            var description = new List<string> {$"Type: {itemParams.Type}"};

            if (itemParams.Tags.Any())
            {
                description[description.Count - 1] += $" <color=grey>[{string.Join(", ", itemParams.Tags.Select(i => $"{i}").ToArray())}]</color>";
            }

            foreach (var attribute in itemParams.Properties)
            {
                description.Add($"{SplitName(attribute.Id.ToString())}: {attribute.Value}");
            }

            Description.text = string.Join(Environment.NewLine, description.ToArray());
        }
        
        public static string SplitName(string name)
        {
            return Regex.Replace(Regex.Replace(name, "[A-Z]", " $0"), "([a-z])([1-9])", "$1 $2").Trim();
        }
    }
}