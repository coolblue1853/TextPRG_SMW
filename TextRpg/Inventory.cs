using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace TextRpg
{
    class EquipInventoryHandler : IGameStateHandler
    {
        bool isShowError = true;

        public void Handle(GameLoop context)
        {
            Utils.UpdateStringBuilder(Database.Instance.sceneDatas.Inventory.banner, false, true);
            Inventory.Instance.AddInventoryStringBuiler(true);

            Utils.UpdateStringBuilder(Database.Instance.sceneDatas.ETC.base_etc, !isShowError);

            if (isShowError)
            {
                Utils.UpdateStringBuilder(Database.Instance.sceneDatas.Error.input_error, isShowError, false);
                isShowError = false;
            }

            Utils.ReadLine(out string action);
            if (int.TryParse(action, out int num)) // 예외처리
            {
                if (num == 0)
                {
                    context.ChangeState(GameState.Inventory);
                }
                else if (num <= Inventory.Instance.GetInventorySize())
                {
                    Inventory.Instance.EquipItem(num);
                }
                else
                {
                    isShowError = true;
                }
            }
            else
            {
                isShowError = true;
            }
        }
    }
    class ShowInventoryHandler : IGameStateHandler
    {
        bool isShowError = false;
     
        public void Handle(GameLoop context)
        {
            Utils.UpdateStringBuilder(Database.Instance.sceneDatas.Inventory.banner, false, true);
            Inventory.Instance.AddInventoryStringBuiler(false);
            Utils.UpdateStringBuilder(Database.Instance.sceneDatas.Inventory.equip_mode);

            Utils.UpdateStringBuilder(Database.Instance.sceneDatas.ETC.base_etc, !isShowError);

            if (isShowError)
            {
                Utils.UpdateStringBuilder(Database.Instance.sceneDatas.Error.input_error, isShowError, false);
                isShowError = false;
            }

            Utils.ReadLine(out string action);
            if (int.TryParse(action, out int num)) // 예외처리
            {
                switch (num)
                {
                    case 0:
                        context.ChangeState(GameState.Town);
                        break;
                    case 1:
                        context.ChangeState(GameState.Equip);
                        break;
                    default:
                        isShowError = true;
                        break;
                }
            }
            else
            {
                isShowError = true;
            }
        }
    }

    internal class Inventory
    {
        public static Inventory Instance { get; private set; } = new Inventory();
        private  Dictionary<int, Item> invenDict = new Dictionary<int, Item>();
        private  Dictionary<ItemType, Item> equipDict = new Dictionary<ItemType, Item>();

        public  Item GetItem(int idx)
        {
            return invenDict.ElementAt(idx - 1).Value;
        }
        public  void AddItem(Item item)
        {
            invenDict.Add(item._id,item);
        }
        public  void DeleteItem(Item item, int num)
        {
            if (item._isEquip)
            {
                UnEquipItem(item);
            }
            invenDict.Remove(invenDict.ElementAt(num-1).Key);
        }
        public  int GetInventorySize() { return invenDict.Count; }
        public  void AddInventoryStringBuiler(bool isShowNum = false)
        {
            for (int i = 1; i < invenDict.Count + 1; i++)
            {
                Item item = invenDict.ElementAt(i - 1).Value;

                string equipText = "";
                if (item._isEquip)
                    equipText = "[E]";

                if (isShowNum)
                    equipText = ($"- {equipText} {i} ");
                else
                    equipText = ($"- {equipText} ");

                // 패딩 과정
                string name = Utils.PadRight(equipText + item._name, 25);
                string effect = Utils.PadRight(Utils.AddEffectText(item), 20);
                string desc = Utils.PadRight(item._description, 55);
                string price = Utils.PadRight(((int)(item._price * 0.85f)).ToString(), 6);

                Utils.UpdateStringBuilder($"{name} | {effect} | {desc} | {price} G\n");
            }
            Utils.UpdateStringBuilder("\n\n");
        }

        // 토글 방식으로 작동
        public  void EquipItem(int index)
        {
            Item item = invenDict.ElementAt(index - 1).Value;
            //토글
            // 장착 해제에 따른 효과 반영
            if (equipDict.ContainsKey(item._itemType)) // 해당하는 타입이 있다면 장착해제 후 장착
            {
                if(equipDict[item._itemType]._id != item._id) // 그게 지금 아이템과 다른경우
                {
                    var ChangeItem = equipDict[item._itemType];
                    ChangeItem.TogleEquipState();
                    ActiveItemEffect(ChangeItem);
                    equipDict[item._itemType] = item;
                }
                else // 그게 지금 아이템과 같은경우
                {
                    equipDict.Remove(item._itemType);
                }
            }
            else
            {
                equipDict.Add(item._itemType, item);
            }
            item.TogleEquipState();
            ActiveItemEffect(item);
        }
        //특정 아이템 장착 해제
        public  void UnEquipItem(Item item)
        {
            equipDict.Remove(item._itemType);
            item.TogleEquipState();
            ActiveItemEffect(item);
        }

        public  void ActiveItemEffect(Item item)
        {
            foreach (var value in item.GetEffect())
            {
                int effectPower = value.Value;
                if (!item._isEquip) // 효과 추가
                    effectPower = -effectPower;

                GameManager.gameLoop.myPlayer.SetAddtionalStat(value.Key, effectPower);
            }
        }

        public  Dictionary<ItemType, Item> GetEquipDict()
        {
            return equipDict;
        }
        public  Dictionary<int, Item> GetInvenDict()
        {
            return invenDict;
        }
        public  void SetInvenDict(Dictionary<int, Item>  changeDict)
        {
            invenDict = changeDict;
        }
    }
}
