using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace TextRpg
{
    public class Item
    {
        public ItemType _itemType { get; private set; }
        public int _id { get; private set; }
        public string _name { get; private set; }
        public string _description { get; private set; }
        public int _price { get; private set; }
        public bool _isEquip { get; private set; }

        protected Dictionary<AdditionalStat, int> _effectsDict = new Dictionary<AdditionalStat, int>();

        protected Item() { }

        protected virtual void SetInfo(ItemData item)
        {
            _id = item.Id;
            _itemType = (ItemType)Enum.Parse(typeof(ItemType), item.Type);
            _name = item.Name;
            var effectsData = item.Effect.Split(',');
            foreach (var value in effectsData)
            {
                var effect = value.Split(':');
                if (Enum.TryParse(effect[0], out AdditionalStat statKey))
                    _effectsDict.Add(statKey, int.Parse(effect[1]));
                else
                    Console.WriteLine($"알 수 없는 스탯 키: {effect[0]}");
            }
            _description = item.Description;
            _price = item.Price;
            _isEquip = false;
        }

        public static Item Create(int itemIdx)
        {
            var itemData = Database.itemData[itemIdx];
            ItemType type = (ItemType)Enum.Parse(typeof(ItemType), itemData.Type);


            switch (type)
            {
                case ItemType.Weapon:
                    return new Weapon(itemData);
                case ItemType.Armor:
                    return new Armor(itemData);
            }
            // 위의 케이스가 아니면 null 반환
            return null;
        }
        public Dictionary<AdditionalStat, int> GetEffect()
        {
            return _effectsDict;
        }
        public void TogleEquipState()
        {
            _isEquip = !_isEquip;
        }
    }

    class Weapon : Item
    {
        public Weapon(ItemData data)
        {
            SetInfo(data);
        }

        protected override void SetInfo(ItemData item)
        {
            base.SetInfo(item);
        }
    }

    class Armor : Item
    {
        public Armor(ItemData data)
        {
            SetInfo(data);
        }

        protected override void SetInfo(ItemData item)
        {
            base.SetInfo(item);
        }
    }
}
