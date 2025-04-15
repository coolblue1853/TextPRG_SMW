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

        }
    }
}
