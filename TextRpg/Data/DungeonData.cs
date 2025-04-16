using System;
using System.Collections.Generic;
using System.Text;

namespace TextRpg.Data
{
    public class DungeonData
    {
        public int Idx { get; set; }
        public string Name { get; set; }
        public int Defense { get; set; }
        public int Gold { get; set; }
        public float DefenseProbability { get; set; }
        public int FailHealthDivied { get; set; }
        public int MinUseHp { get; set; }
        public int MaxUseHp { get; set; }
        public int GoldRatio { get; set; }

    }
}
