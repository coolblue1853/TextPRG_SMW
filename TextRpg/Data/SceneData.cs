using System;
using System.Collections.Generic;
using System.Text;

namespace TextRpg
{
    public class SceneTextData
    {
        public IntroText Intro { get; set; }
        public ErrorText Error { get; set; }
        public TownText Town { get; set; }
        public StatText Stat { get; set; }
        public InventoryText Inventory { get; set; }
        public ShopText Shop { get; set; }
        public ETCText ETC { get; set; }
    }
    public class IntroText
    {
        public string intro_welcome { get; set; }
        public string choose_job { get; set; }
    }
    public class TownText
    {
        public string town_welcome { get; set; }
        public string town_select { get; set; }
    }
    public class StatText
    {
        public string level_name { get; set; }
        public string attack { get; set; }
        public string defense { get; set; }
        public string etc { get; set; }
    }
    public class InventoryText
    {
        public string benner { get; set; }
        public string equip_mode { get; set; }
    }
    public class ShopText
    {
        public string benner { get; set; }
        public string buy { get; set; }
        public string buy_fail { get; set; }
        public string buy_Succ { get; set; }
        public string buy_already { get; set; }
    }
    public class ETCText
    {
        public string base_etc { get; set; }

    }
    public class ErrorText
    {
        public string input_error { get; set; }

    }
}
