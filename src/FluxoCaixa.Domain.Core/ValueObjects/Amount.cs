using System;

namespace FluxoCaixa.Domain.Core.ValueObjects
{
    /// <summary>
    /// Value Object para representar valores monetários
    /// </summary>
    public sealed class Amount
    {
        private readonly decimal _value;

        public Amount(decimal value)
        {
            _value = value;
        }

        public override string ToString()
        {
            return _value.ToString("C");
        }

        public static implicit operator decimal(Amount value)
        {
            return value._value;
        }

        public static Amount operator -(Amount value)
        {
            return new Amount(Math.Abs(value._value) * -1);
        }

        public static implicit operator Amount(decimal value)
        {
            return new Amount(value);
        }

        public static Amount operator +(Amount amount1, Amount amount2)
        {
            return new Amount(amount1._value + amount2._value);
        }

        public static Amount operator -(Amount amount1, Amount amount2)
        {
            return new Amount(amount1._value - amount2._value);
        }

        public static bool operator <(Amount amount1, Amount amount2)
        {
            return amount1._value < amount2._value;
        }

        public static bool operator >(Amount amount1, Amount amount2)
        {
            return amount1._value > amount2._value;
        }

        public static bool operator <=(Amount amount1, Amount amount2)
        {
            return amount1._value <= amount2._value;
        }

        public static bool operator >=(Amount amount1, Amount amount2)
        {
            return amount1._value >= amount2._value;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
            {
                return false;
            }

            if (ReferenceEquals(this, obj))
            {
                return true;
            }

            if (obj is decimal)
            {
                return (decimal)obj == _value;
            }

            return ((Amount)obj)._value == _value;
        }

        public override int GetHashCode()
        {
            return _value.GetHashCode();
        }
    }
} 