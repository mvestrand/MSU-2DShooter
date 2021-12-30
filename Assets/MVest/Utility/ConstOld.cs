using System;

using UnityEngine;

namespace MVest {

    namespace ConstOld {

        public abstract class IntConstant : ConstantOld {
            public abstract int GetValue();
            public override bool PreventOverflow { get { return false; } }
            public override decimal ValueDecimal() { return GetValue(); }
            public override double ValueDouble() { return GetValue(); }
            public override long ValueInt64() { return GetValue(); }
            public override float ValueSingle() { return GetValue(); }
        }

        public abstract class FloatConstant : ConstantOld {
            public abstract float GetValue();
            public override bool PreventOverflow { get { return false; } }
            public override decimal ValueDecimal() { throw new NotSupportedException(); }
            public override double ValueDouble() { return GetValue(); }
            public override long ValueInt64() { throw new NotSupportedException(); }
            public override float ValueSingle() { return GetValue(); }
        }

        public class MaxValue : ConstantOld
        {
            public override bool PreventOverflow { get { return true; } }
            public override decimal ValueDecimal() { return decimal.MaxValue; }
            public override double ValueDouble() { return double.MaxValue; }
            public override long ValueInt64() { return long.MaxValue; }
            public override float ValueSingle() { return float.MaxValue; }
        }

        public class MinValue : ConstantOld
        {
            public override bool PreventOverflow { get { return true; } }
            public override decimal ValueDecimal() { return decimal.MinValue; }
            public override double ValueDouble() { return double.MinValue; }
            public override long ValueInt64() { return long.MinValue; }
            public override float ValueSingle() { return float.MinValue; }
        }

        public class PositiveInfinity : ConstantOld
        {
            public override bool PreventOverflow { get { return false; } }
            public override decimal ValueDecimal() { throw new NotSupportedException(); }
            public override double ValueDouble() { return double.PositiveInfinity; }
            public override long ValueInt64() { throw new NotSupportedException(); }
            public override float ValueSingle() { return float.PositiveInfinity; }
        }

        public class NegativeInfinity : ConstantOld
        {
            public override bool PreventOverflow { get { return false; } }
            public override decimal ValueDecimal() { throw new NotSupportedException(); }
            public override double ValueDouble() { return double.NegativeInfinity; }
            public override long ValueInt64() { throw new NotSupportedException(); }
            public override float ValueSingle() { return float.NegativeInfinity; }
        }


        public class NaN : ConstantOld
        {
            public override bool PreventOverflow { get { return false; } }
            public override decimal ValueDecimal() { throw new NotSupportedException(); }
            public override double ValueDouble() { return double.NaN; }
            public override long ValueInt64() { throw new NotSupportedException(); }
            public override float ValueSingle() { return float.NaN; }
        }

        public class Zero : ConstantOld
        {
            public override bool PreventOverflow { get { return false; } }
            public override decimal ValueDecimal() { return decimal.Zero; }
            public override double ValueDouble() { return 0; }
            public override long ValueInt64() { return 0; }
            public override float ValueSingle() { return 0f; }
        }

        public class One : ConstantOld
        {
            public override bool PreventOverflow { get { return false; } }
            public override decimal ValueDecimal() { return decimal.One; }
            public override double ValueDouble() { return 1; }
            public override long ValueInt64() { return 1; }
            public override float ValueSingle() { return 1; }
        }

        public class MinusOne : ConstantOld
        {
            public override bool PreventOverflow { get { return false; } }
            public override decimal ValueDecimal() { return decimal.MinusOne; }
            public override double ValueDouble() { return -1; }
            public override long ValueInt64() { return -1; }
            public override float ValueSingle() { return -1; }
        }

        public abstract class ConstantOld {
            public virtual bool PreventOverflow { get; }
            public virtual T Value<T>() {
                // Signed Integers
                if (typeof(T) == typeof(long)) {
                    return (T)(object)ValueInt64();
                } else if (typeof(T) == typeof(int)) {
                    return (T)(object)ValueInt32();
                } else if (typeof(T) == typeof(short)) {
                    return (T)(object)ValueInt16();
                } else if (typeof(T) == typeof(sbyte)) {
                    return (T)(object)ValueSByte();
                }
                // Unsigned Integers
                else if (typeof(T) == typeof(ulong)) {
                    return (T)(object)ValueUInt64();
                } else if (typeof(T) == typeof(uint)) {
                    return (T)(object)ValueUInt32();
                } else if (typeof(T) == typeof(ushort)) {
                    return (T)(object)ValueUInt16();
                } else if (typeof(T) == typeof(byte)) {
                    return (T)(object)ValueByte();
                }

                else if (typeof(T) == typeof(decimal)) {
                    return (T)(object)ValueDecimal();
                } else if (typeof(T) == typeof(double)) {
                    return (T)(object)ValueDouble();
                } else if (typeof(T) == typeof(float)) {
                    return (T)(object)ValueSingle();
                }
                throw new NotSupportedException();
            }

        

            // Signed integers
            public abstract long ValueInt64();
            public virtual int ValueInt32() {
                return (int)HandleOverflow(ValueInt64(), int.MaxValue, int.MinValue, typeof(int));
            }
            public virtual short ValueInt16() {
                return (short)HandleOverflow(ValueInt64(), short.MaxValue, short.MinValue, typeof(short));
            }
            public virtual sbyte ValueSByte() {
                return (sbyte)HandleOverflow(ValueInt64(), sbyte.MaxValue, sbyte.MinValue, typeof(sbyte));
            }

            // Unsigned integers
            public virtual ulong ValueUInt64() { throw new NotSupportedException(); } // I can't be bothered to handle this edge case right now
            public virtual uint ValueUInt32() {
                return (uint)HandleOverflow(ValueInt64(), uint.MaxValue, uint.MinValue, typeof(uint));
            }
            public virtual ushort ValueUInt16() {
                return (ushort)HandleOverflow(ValueInt64(), ushort.MaxValue, ushort.MinValue, typeof(ushort));
            }
            public virtual byte ValueByte() {
                return (byte)HandleOverflow(ValueInt64(), byte.MaxValue, byte.MinValue, typeof(byte));
            }

            // Floating point values
            public abstract decimal ValueDecimal();
            public abstract double ValueDouble();
            public abstract float ValueSingle();

            // Unity Vectors
            public virtual Vector2Int ValueVector2Int() { int i = ValueInt32(); return new Vector2Int(i, i); }
            public virtual Vector3Int ValueVector3Int() { int i = ValueInt32(); return new Vector3Int(i, i, i); }
            public virtual Vector2 ValueVector2() { float f = ValueSingle(); return new Vector2(f, f); }
            public virtual Vector3 ValueVector3() { float f = ValueSingle(); return new Vector3(f, f, f); }
            public virtual Vector4 ValueVector4() { float f = ValueSingle(); return new Vector4(f, f, f, f); }

            private long HandleOverflow(long value, long maxValue, long minValue, Type type) {
                if (value > maxValue) {
                    if (PreventOverflow)
                        return maxValue;
                    else
                        throw new System.OverflowException("Constant value "+value+" is too big to be cast to type: "+type.Name);
                } else if (value < minValue) {
                    if (PreventOverflow)
                        return minValue;
                    else
                        throw new System.OverflowException("Constant value "+value+" is too small to be cast to type: "+type.Name);
                } else {
                    return value;
                }
            }

        }

    }
}
