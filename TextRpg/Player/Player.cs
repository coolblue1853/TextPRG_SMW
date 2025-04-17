using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Numerics;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Xml.Serialization;

namespace TextRpg
{
    public class Player
    {
        public PlayerType _playerType { get; private set; }
        public string _nickName { get; private set; }
        public string _className { get; private set; }
        public Level _level { get; private set; }
        public int _exp { get; private set; }
        public Attack _attack { get; private set; }
        public Defense _defense { get; private set; }
        public int _hp { get; private set; }
        public int _gold { get; private set; }

        private float attackUpPoint = 0.5f;
        private float defenseUpPoint = 1.0f;

        private Dictionary<AdditionalStat, Action<int>> _statAdders;
        private Dictionary<AdditionalStat, int> _additionalStat;
        public Player()
        {
        }
        protected Player(PlayerType type)
        {
            _playerType = type;
        }
        protected Player(PlayerSaveData data)
        {
            _playerType = (PlayerType)data.PlayerType;
            _nickName = data.NickName;
            _className = data.ClassName;
            _level = (Level)data.Level;
            _exp = data.Exp;
            _attack = new Attack(data.AttackBase);
            _attack.SetAddValue(new StatFloat(data.AttackAdd));
            _defense = new Defense(data.DefenseBase);
            _defense.SetAddValue(new StatFloat(data.DefenseAdd));
            _hp = data.HP;
            _gold = data.Gold;

            InitStatDelegates();
        }

        public virtual void SetInfo(string name, JobData job, Level level = Level.LV_1, int exp = 0, int gold = 1500)
        {
            _nickName = name;
            _className = job.Name;
            _level = level;
            _exp = exp;
            _attack = new Attack(job.Attack);
            _defense = new Defense(job.Defense);
            _hp = job.MaxHP;
            _gold = gold;

            InitStatDelegates();
        }

        private void InitStatDelegates()
        {
            _statAdders = new Dictionary<AdditionalStat, Action<int>>()
        {
            { AdditionalStat.ATK, v => _attack.SetAddValue(new StatFloat(v)) },
            { AdditionalStat.DEF, v => _defense.SetAddValue(new StatFloat(v)) }
        };
            _additionalStat = new Dictionary<AdditionalStat, int>();
        }

        public void SetAddtionalStat(AdditionalStat key, int value)
        {
            if (_statAdders.TryGetValue(key, out var action))
            {
                action(value);

                if (_additionalStat.ContainsKey(key))
                {
                    _additionalStat[key] += value;

                    if (_additionalStat[key] == 0)
                        _additionalStat.Remove(key);
                }
                else
                    _additionalStat.Add(key, value);
            }
            else
                Console.WriteLine($"알 수 없는 추가 스탯: {key}");
        }

        public Dictionary<AdditionalStat, int> GetAdditionalStat()
        {
            return _additionalStat;
        }


        public Dictionary<string, string> GetFormattedStats()
        {
            return new Dictionary<string, string>
        {
            { "level", ((int)_level).ToString() },
            { "nickName", _nickName },
            { "className", _className },
            { "attack", _attack.GetBaseValue().Value.ToString() },
            { "defense", _defense.GetBaseValue().Value.ToString() },
            { "hp", _hp.ToString() },
            { "gold", _gold.ToString() }
        };
        }
        public void ChangeGold(int value)
        {
            _gold += value;
        }

        public void ChangeHp(int value)
        {
            _hp += value;
        }

        public void SetHp(int value)
        {
            _hp = value;
        }
        public void ChangeExp(int value)
        {
            _exp += value;

            if ((int)_level < _exp && _level < Level.LV_4)
            {
                _level++;
                _attack.SetBaseValue(_attack._baseValue.Add(new StatFloat(attackUpPoint)));
                _defense.SetBaseValue(_defense._baseValue.Add(new StatFloat(defenseUpPoint)));
            }
        }
    }

    class Warrior : Player
    {
        public Warrior() : base(PlayerType.Warrior)
        {
        }

        public Warrior(PlayerSaveData data) : base(data)
        {

        }


        public override void SetInfo(string name, JobData job, Level level = Level.LV_1, int exp = 0, int gold = 1500)
        {
            base.SetInfo(name, job, level, gold);
        }
    }


}
