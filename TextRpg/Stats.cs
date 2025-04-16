using System;
using System.Collections.Generic;
using System.Text;
using System.Numerics;

namespace TextRpg
{
    interface IAddable<T>
    {
        T Add(T other);
    }
    class StatInt : IAddable<StatInt>
    {
        public int Value { get; }

        public StatInt()
        {
            Value = 0;
        }
        public StatInt(int value)
        {
            Value = value;
        }

        public StatInt Add(StatInt other)
        {
            return new StatInt(this.Value + other.Value);
        }
    }
    class StatFloat : IAddable<StatFloat>
    {
        public float Value { get; }

        public StatFloat()
        {
            Value = 0f;
        }
        public StatFloat(float value)
        {
            Value = value;
        }

        public StatFloat Add(StatFloat other)
        {
            return new StatFloat(this.Value + other.Value);
        }
    }

    abstract class Stat<T> where T : IAddable<T>, new()
    {
        public T _baseValue { get; protected set; }
        public T _addValue { get; protected set; }

        // 생성자에서 기본값을 설정
        public Stat(T value)
        {
            _baseValue = value;
            _addValue = new T(); 
        }

        // 최종 값을 구하는 프로퍼티
        public T FinalValue => _baseValue.Add(_addValue);

        // 기본값과 추가값을 반환하는 메서드
        public T GetBaseValue() => _baseValue;
        public T GetAddValue() => _addValue;
        public T GetFinalValue() => FinalValue;

        // 기본값 설정
        public void SetBaseValue(T value)
        {
            _baseValue = value;
        }

        // 추가값 설정
        public void SetAddValue(T value)
        {
            _addValue = _addValue.Add(value); // Add 연산을 호출
        }

    }

    class Defense : Stat<StatFloat>
    {
        public Defense(int value) : base(new StatFloat(value))  // StatFloat로 값 래핑
        {
        }
    }

    class Attack : Stat<StatFloat>
    {
        public Attack(int value) : base(new StatFloat(value))  // StatFloat로 값 래핑
        {
        }
    }


}
