using System;
using System.Collections.Generic;
using System.Text;

namespace TextRpg
{

    class Player
    {
        protected PlayerType _playerType;
        protected string _nickName;
        protected string _className;
        protected int _level;
        protected int _attack;
        protected int _defense;
        protected int _hp;
        protected int _gold;

        protected Player(PlayerType type)
        {
            _playerType = type;
        }

        public virtual void SetInfo(string name, JobData job,int level = 1,int gold = 1500)
        {
            _nickName = name;
            _className = job.Name;
            _level = level;
            _attack = job.Attack;
            _defense = job.Defense;
            _hp = job.MaxHP;
            _gold = gold;
        }

        public string GetNickName() { return _nickName; }
        public string GetClass() { return _className; }
        public int GetLevel() { return _level; }
        public int GetAttack() { return _attack; }
        public int GetDefense() { return _defense; }
        public int GetHP() { return _hp; }
        public int GetGold() { return _gold; }
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
