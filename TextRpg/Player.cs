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
        public int _attack { get; private set; }
        public int _defense { get; private set; }
        public int _hp { get; private set; }
        public int _gold { get; private set; }

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
