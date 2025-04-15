using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TextRpg
{
    public class Shop
    {
        static Dictionary<Item, bool> shopItems = new Dictionary<Item, bool>();

        public static void Init()
        {
            foreach (var value in Database.items)
                shopItems.Add(value, false);
        }
        public static int GetShopSize()
        {
            return shopItems.Count;
        }
        public static void AddShopStringBuiler(bool isShowNum = false)
        {
            for (int i = 1; i < shopItems.Count+1; i++)
            {
                Item item = shopItems.ElementAt(i-1).Key;
                bool isBuy = shopItems.ElementAt(i-1).Value;


                if (isShowNum)
                    Utils.UpdateStringBuilder($"- {i} ");
                else
                    Utils.UpdateStringBuilder("- ");

                Utils.UpdateStringBuilder($"{item._name} | {Utils.AddEffectText(item)} | {item._description} | ");
                if(isBuy)
                    Utils.UpdateStringBuilder("[구매완료]\n");
                else
                    Utils.UpdateStringBuilder($"{item._price}G\n");

            }
            Utils.UpdateStringBuilder("\n\n");
        }

        public static void BuyItem(int index)
        {
            Item item = shopItems.ElementAt(index - 1).Key;
            // 안 산 물건이라면
            if (shopItems[item] == false)
            {
                if(GameManager.myPlayer._gold >= item._price)
                {
                    shopItems[item] = true;
                    Inventory.AddItem(item);
                    GameManager.myPlayer.SetGold(-item._price);
                    Utils.UpdateStringBuilder("구매를 완료했습니다.\n");
                }
                else
                    Utils.UpdateStringBuilder("Gold가 부족합니다.\n");
            }
            else
                Utils.UpdateStringBuilder("이미 구매한 아이템 입니다.\n");
        }
    }
}
