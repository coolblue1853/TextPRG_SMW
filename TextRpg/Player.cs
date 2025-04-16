using System;
using System.Collections.Generic;
using System.Text;

namespace TextRpg
{
    class Player
    {
        public PlayerType _playerType {  get; private set; }
        public string _nickName { get; private set; }
        public string _className { get; private set; }
        public int _level { get; private set; }
        public Attack _attack { get; private set; }
        public Defense _defense { get; private set; }
        public int _hp { get; private set; }
        public int _gold { get; private set; }

        private Dictionary<AdditionalStat, Action<int>> _statAdders;
        private Dictionary<AdditionalStat, int> _additionalStat;
        protected Player(PlayerType type)
        {
            _playerType = type;
        }

        public virtual void SetInfo(string name, JobData job,int level = 1,int gold = 1500)
        {
            _nickName = name;
            _className = job.Name;
            _level = level;
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
            { AdditionalStat.ATK, v => _attack.SetAddValue(v) },
            { AdditionalStat.DEF, v => _defense.SetAddValue(v) }
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

        public void SetGold(int value)
        {
            _gold += value;
        }

        public Dictionary<string, string> GetFormattedStats()
        {
            return new Dictionary<string, string>
        {
            { "level", _level.ToString() },
            { "nickName", _nickName },
            { "className", _className },
            { "attack", _attack.GetBaselValue().ToString() },
            { "defense", _defense.GetBaselValue().ToString() },
            { "hp", _hp.ToString() },
            { "gold", _gold.ToString() }
        };
        }
    }

    class Warrior : Player
    {
        public Warrior() : base(PlayerType.Warrior)
        {
        }

        public override void SetInfo(string name, JobData job, int level = 1, int gold = 1500)
        {
            base.SetInfo(name, job, level, gold);
        }

    }
}
