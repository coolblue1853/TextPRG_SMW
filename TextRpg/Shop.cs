using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TextRpg
{
    class SellShopHandler : IGameStateHandler
    {
        bool isShowError = false;
        string shopText = "";
        public void Handle(GameLoop context)
        {
            DataLoader dataLoader = new DataLoader();
            Utils.UpdateStringBuilder(dataLoader.FormatText(Database.Instance.sceneDatas.Shop.banner,context.myPlayer.GetFormattedStats()), false, true);
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
                    context.ChangeState(GameState.Shop);
                }
                else if (num <= Inventory.Instance.GetInventorySize())
                {
                    Shop.Instance.SellItem(num);
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
    class BuyShopHandler : IGameStateHandler
    {
        bool isShowError = false;
        string shopText = "";
        public void Handle(GameLoop context)
        {
            DataLoader dataLoader = new DataLoader();
            Utils.UpdateStringBuilder(dataLoader.FormatText(Database.Instance.sceneDatas.Shop.banner,context.myPlayer.GetFormattedStats()), false, true);
            Shop.Instance.AddShopStringBuiler(true);

            if (shopText != "")
            {
                Utils.UpdateStringBuilder(Database.Instance.sceneDatas.ETC.base_etc);
                Utils.UpdateStringBuilder(shopText, true, false);
                shopText = "";
            }
            else
            {
                Utils.UpdateStringBuilder(Database.Instance.sceneDatas.ETC.base_etc, !isShowError);
            }

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
                    context.ChangeState(GameState.Shop);
                }
                else if (num <= Shop.Instance.GetShopSize())
                {
                    shopText = Shop.Instance.BuyItem(num);
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

    class ShowShopHandler : IGameStateHandler
    {
        bool isShowError = false;
        string shopText = "";
        public void Handle(GameLoop context)
        {
            DataLoader dataLoader = new DataLoader();
            Utils.UpdateStringBuilder(dataLoader.FormatText(Database.Instance.sceneDatas.Shop.banner, context.myPlayer.GetFormattedStats()), false, true);
            Shop.Instance.AddShopStringBuiler(false);
            Utils.UpdateStringBuilder(Database.Instance.sceneDatas.Shop.buy);

            if (shopText != "")
            {
                Utils.UpdateStringBuilder(Database.Instance.sceneDatas.ETC.base_etc);
                Utils.UpdateStringBuilder(shopText, true, false);
                shopText = "";
            }
            else
            {
                Utils.UpdateStringBuilder(Database.Instance.sceneDatas.ETC.base_etc, !isShowError);
            }

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
                        context.ChangeState(GameState.Buy);
                        break;
                    case 2:
                        context.ChangeState(GameState.Sell);
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
    public class Shop
    {
        public static Shop Instance { get; private set; } = new Shop();
        Dictionary<int, (Item, bool)> shopItems = new Dictionary<int, (Item, bool)>();

        public void Init() // static을 제거
        {
            foreach (var value in Database.Instance.items)
                shopItems.Add(value._id, (value, false));
        }
        public  int GetShopSize()
        {
            return shopItems.Count;
        }
        public  void AddShopStringBuiler(bool isShowNum = false)
        {
            for (int i = 1; i < shopItems.Count+1; i++)
            {
                Item item = shopItems.ElementAt(i-1).Value.Item1;
                bool isBuy = shopItems.ElementAt(i-1).Value.Item2;


                string equipText = "";

                if (isShowNum)
                    equipText = ($"- {i} ");
                else
                    equipText = ($"- ");

                string name = Utils.PadRight(equipText + item._name, 25);
                string effect = Utils.PadRight(Utils.AddEffectText(item), 20);
                string desc = Utils.PadRight(item._description, 55);
                string price = Utils.PadRight(((int)(item._price)).ToString(), 6);

                Utils.UpdateStringBuilder($"{name} | {effect} | {desc} | ");


                if(isBuy)
                    Utils.UpdateStringBuilder("[구매완료]\n");
                else
                    Utils.UpdateStringBuilder($"{price}G\n");

            }
            Utils.UpdateStringBuilder("\n\n");
        }

        public string BuyItem(int index)
        {
            Item item = shopItems.ElementAt(index - 1).Value.Item1;
            // 안 산 물건이라면
            if (shopItems[item._id].Item2 == false)
            {
                if(GameManager.gameLoop.myPlayer._gold >= item._price)
                {
                    shopItems[item._id] = (item,true);
                    Inventory.Instance.AddItem(item);
                    GameManager.gameLoop.myPlayer.ChangeGold(-item._price);
                    return Database.Instance.sceneDatas.Shop.buy_Succ;
                }
                else
                    return Database.Instance.sceneDatas.Shop.buy_fail;
            }
            else
                return Database.Instance.sceneDatas.Shop.buy_already;
        }

        public  void SellItem(int num)
        {
            Item sellItem = Inventory.Instance.GetItem(num);
            shopItems[sellItem._id] = (sellItem, false);
            int sellPrice = (int)(sellItem._price * 0.85f);
            GameManager.gameLoop.myPlayer.ChangeGold(sellPrice);
            Inventory.Instance.DeleteItem(sellItem, num);
        }
        public  void SetItemBuy(int idx)
        {
            shopItems[idx] = (shopItems[idx].Item1, true);
        }
    }
}
