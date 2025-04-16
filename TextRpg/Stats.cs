using System;
using System.Collections.Generic;
using System.Text;

namespace TextRpg
{
    abstract class Stat
    {
        public Stat(int value)
        {
            _baseValue = value;
        }

        public int _baseValue { get; protected set; }
        public int _addValue { get; protected set; }

        public int _finalValue => _baseValue + _addValue;
        public int GetBaselValue() => _baseValue;
        public int GetAddValue() => _addValue;
        public int GetFinalValue() => _finalValue;

        public void SetBaseValue(int value)
        {
            _baseValue = value;
        }

        public void SetAddValue(int value)
        {
            _addValue += value;
        }
    }

    class Defense : Stat
    {
        public Defense(int value) : base(value)
        {
        }
    }
    class Attack : Stat
    {
        public Attack(int value) : base(value)
        {
        }
    }


}
