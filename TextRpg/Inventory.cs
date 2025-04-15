using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Text;

namespace TextRpg
{
    internal class Inventory
    {
        private static List<Item> inventory = new List<Item>();

        public static void AddItem(Item item)
        {
            inventory.Add(item);
        }
        public static void ShowInventory()
        {
            for(int i = 1; i < inventory.Count+1; i++)
            {
                Item item = inventory[i-1];
                Utils.UpdateStringBuilder($"- {i}  {item._name} | {AddEffectText(item)} | {item._description}");
            }
            Utils.ShowStringBuilder();
        }

        public static string AddEffectText(Item item)
        {
            StringBuilder effectSB = new StringBuilder();
            foreach(var value in item.GetEffect())
            {
                effectSB.Append($"{value.Key} + {value.Value} ");
            }

            return effectSB.ToString();
        }
    }
}
