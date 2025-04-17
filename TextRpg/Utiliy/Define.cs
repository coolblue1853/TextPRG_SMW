using System;
using System.Collections.Generic;
using System.Text;

namespace TextRpg
{
    public enum GameState
    {
        None = 0,
        SetChar = 1,
        Town = 2,
        CheckStat = 3,
        Inventory = 4,
        Equip = 5,
        Shop = 6,
        Buy = 7,
        Sell = 8,
        Dungeon = 9,
        DungeonResult = 10,
        Restore = 11,
    }

    public enum PlayerType
    {
        None = 0,
        Warrior = 1
    }
    public enum ItemType
    {
        None = 0,
        Weapon = 1,
        Armor = 2
    }
    public enum AdditionalStat
    {
        None = 0,
        ATK = 1,
        DEF = 2,
    }
    public enum Level
    {
        LV_1 = 1,
        LV_2 = 2,
        LV_3 = 3,
        LV_4 = 4,
    }
}
