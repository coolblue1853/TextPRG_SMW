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
        public static int GetInventorySize() { return inventory.Count; }
        public static void AddInventoryStringBuiler(bool isShowNum = false)
        {
            for(int i = 1; i < inventory.Count+1; i++)
            {
                Item item = inventory[i-1];

                string equipText = "";
                if (item._isEquip)
                    equipText = "[E]";

                if (isShowNum)
                    Utils.UpdateStringBuilder($"- {equipText} {i} ");
                else
                    Utils.UpdateStringBuilder($"- {equipText}");

                Utils.UpdateStringBuilder($"{item._name} | {Utils.AddEffectText(item)} | {item._description}\n");
            }
            Utils.UpdateStringBuilder("\n\n");
        }



        // 토글 방식으로 작동
        public static void EquipItem(int index)
        {
            Item item = inventory[index - 1];
            //토글
            item.TogleEquipState();

            // 장착 해제에 따른 효과 반영

      
            foreach (var value in item.GetEffect())
            {
                int effectPower = value.Value;
                if (!item._isEquip) // 효과 추가
                    effectPower = -effectPower;

                GameManager.myPlayer.SetAddtionalStat(value.Key, effectPower);
            }
    
        }
    }
}
