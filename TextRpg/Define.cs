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
        Inventory = 4
    }

    public enum PlayerType
    {
        Unknown = 0,
        Warrior = 1
    }
}
