using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace TextRpg
{
    class Item
    {
        public ItemType _itemType { get; private set; }
        public int _id { get; private set; }
        public string _name { get; private set; }
        public string _description { get; private set; }
        public int _price { get; private set; }
        public bool _isEquip { get; private set; }

        protected Dictionary<string, int> _effectsDict = new Dictionary<string, int>();

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
                _effectsDict.Add(effect[0], int.Parse(effect[1]));
            }
            _description = item.Description;
            _price = item.Price;
            _isEquip = false;
        }

        public static Item Create(int itemIdx)
        {
            var itemData = Database.items[itemIdx];
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
        public Dictionary<string, int> GetEffect()
        {
            return _effectsDict;
        }
    }

    class Weapon : Item
    {
        protected int attack;

        public Weapon(ItemData data)
        {
            SetInfo(data);
        }

        protected override void SetInfo(ItemData item)
        {
            base.SetInfo(item);
            attack = _effectsDict["ATK"];
        }
    }

    class Armor : Item
    {
        protected int defence;

        public Armor(ItemData data)
        {
            SetInfo(data);
        }

        protected override void SetInfo(ItemData item)
        {
            base.SetInfo(item);
            defence = _effectsDict["DEF"];
        }
    }
}
