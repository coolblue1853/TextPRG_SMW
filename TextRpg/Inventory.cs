using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace TextRpg
{
    internal class Inventory
    {
        private static Dictionary<int, Item> inventory = new Dictionary<int, Item>();
        private static Dictionary<ItemType, Item> equipDict = new Dictionary<ItemType, Item>();

        public static Item GetItem(int idx)
        {
            return inventory.ElementAt(idx - 1).Value;
        }
        public static void AddItem(Item item)
        {
            inventory.Add(item._id,item);
        }
        public static void DeleteItem(Item item)
        {
            if (item._isEquip)
                EquipItem(item._id);
            inventory.Remove(item._id);
        }
        public static int GetInventorySize() { return inventory.Count; }
        public static void AddInventoryStringBuiler(bool isShowNum = false)
        {
            for(int i = 1; i < inventory.Count+1; i++)
            {
                Item item = inventory.ElementAt(i - 1).Value;

                string equipText = "";
                if (item._isEquip)
                    equipText = "[E]";

                if (isShowNum)
                    Utils.UpdateStringBuilder($"- {equipText} {i} ");
                else
                    Utils.UpdateStringBuilder($"- {equipText}");

                Utils.UpdateStringBuilder($"{item._name} | {Utils.AddEffectText(item)} | {item._description} | {(int)(item._price *0.85f)} G\n");
            }
            Utils.UpdateStringBuilder("\n\n");
        }

        // 토글 방식으로 작동
        public static void EquipItem(int index)
        {
            Item item = inventory.ElementAt(index - 1).Value;
            //토글
            item.TogleEquipState();

            // 장착 해제에 따른 효과 반영
            if (equipDict.ContainsKey(item._itemType)) // 해당하는 타입이 있다면 장착해제 후 장착
            {
                if(equipDict[item._itemType]._id != item._id)
                {
                    var ChangeItem = equipDict[item._itemType];
                    ChangeItem.TogleEquipState();
                    ActiveItemEffect(ChangeItem);
                    equipDict[item._itemType] = item;
                }
            }
            else
            {
                equipDict.Add(item._itemType, item);
            }
            ActiveItemEffect(item);
        }

        public static void ActiveItemEffect(Item item)
        {
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
