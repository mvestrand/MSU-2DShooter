
using UnityEngine;



namespace MVest {

    public static class Const {

        public struct MaxValue : IConstant<long>, IConstant<int>, IConstant<short>, IConstant<sbyte>, 
                            IConstant<ulong>, IConstant<uint>, IConstant<ushort>, IConstant<byte>,
                            IConstant<double>, IConstant<float>, 
                            IConstant<Vector2>, IConstant<Vector3>, IConstant<Vector4>,
                            IConstant<Vector2Int>, IConstant<Vector3Int>{
            public void Value(out long value) { value = long.MaxValue; }
            public void Value(out int value) { value = int.MaxValue; }
            public void Value(out short value) { value = short.MaxValue; }
            public void Value(out sbyte value) { value = sbyte.MaxValue; }
            public void Value(out ulong value) { value = ulong.MaxValue; }
            public void Value(out uint value) { value = uint.MaxValue; }
            public void Value(out ushort value) { value = ushort.MaxValue; }
            public void Value(out byte value) { value = byte.MaxValue; }

            public void Value(out double value) { value = double.MaxValue; }
            public void Value(out float value) { value = float.MaxValue; }
            public void Value(out Vector2 value) { Value(out float f); value = new Vector2(f, f); }
            public void Value(out Vector3 value) { Value(out float f); value = new Vector3(f, f, f); }
            public void Value(out Vector4 value) { Value(out float f); value = new Vector4(f, f, f, f); }
            public void Value(out Vector2Int value) { Value(out int i); value = new Vector2Int(i, i); }
            public void Value(out Vector3Int value) { Value(out int i); value = new Vector3Int(i, i, i); }

        }

        public struct MinValue : IConstant<long>, IConstant<int>, IConstant<short>, IConstant<sbyte>, 
                            IConstant<ulong>, IConstant<uint>, IConstant<ushort>, IConstant<byte>,
                            IConstant<double>, IConstant<float> {
            public void Value(out long value) { value = long.MinValue; }
            public void Value(out int value) { value = int.MinValue; }
            public void Value(out short value) { value = short.MinValue; }
            public void Value(out sbyte value) { value = sbyte.MinValue; }
            public void Value(out ulong value) { value = ulong.MinValue; }
            public void Value(out uint value) { value = uint.MinValue; }
            public void Value(out ushort value) { value = ushort.MinValue; }
            public void Value(out byte value) { value = byte.MinValue; }

            public void Value(out double value) { value = double.MinValue; }
            public void Value(out float value) { value = float.MinValue; }
            public void Value(out Vector2 value) { Value(out float f); value = new Vector2(f, f); }
            public void Value(out Vector3 value) { Value(out float f); value = new Vector3(f, f, f); }
            public void Value(out Vector4 value) { Value(out float f); value = new Vector4(f, f, f, f); }
            public void Value(out Vector2Int value) { Value(out int i); value = new Vector2Int(i, i); }
            public void Value(out Vector3Int value) { Value(out int i); value = new Vector3Int(i, i, i); }
        }

        public struct Zero : IConstant<long>, IConstant<int>, IConstant<short>, IConstant<sbyte>, 
                            IConstant<ulong>, IConstant<uint>, IConstant<ushort>, IConstant<byte>,
                            IConstant<double>, IConstant<float> {
            public void Value(out long value) { value = 0; }
            public void Value(out int value) { value = 0; }
            public void Value(out short value) { value = 0; }
            public void Value(out sbyte value) { value = 0; }
            public void Value(out ulong value) { value = 0; }
            public void Value(out uint value) { value = 0; }
            public void Value(out ushort value) { value = 0; }
            public void Value(out byte value) { value = 0; }

            public void Value(out double value) { value = 0; }
            public void Value(out float value) { value = 0; }
            public void Value(out Vector2 value) { Value(out float f); value = new Vector2(f, f); }
            public void Value(out Vector3 value) { Value(out float f); value = new Vector3(f, f, f); }
            public void Value(out Vector4 value) { Value(out float f); value = new Vector4(f, f, f, f); }
            public void Value(out Vector2Int value) { Value(out int i); value = new Vector2Int(i, i); }
            public void Value(out Vector3Int value) { Value(out int i); value = new Vector3Int(i, i, i); }
        }

        public struct One : IConstant<long>, IConstant<int>, IConstant<short>, IConstant<sbyte>, 
                            IConstant<ulong>, IConstant<uint>, IConstant<ushort>, IConstant<byte>,
                            IConstant<double>, IConstant<float> {
            public void Value(out long value) { value = 1; }
            public void Value(out int value) { value = 1; }
            public void Value(out short value) { value = 1; }
            public void Value(out sbyte value) { value = 1; }
            public void Value(out ulong value) { value = 1; }
            public void Value(out uint value) { value = 1; }
            public void Value(out ushort value) { value = 1; }
            public void Value(out byte value) { value = 1; }

            public void Value(out double value) { value = 1; }
            public void Value(out float value) { value = 1; }
            public void Value(out Vector2 value) { Value(out float f); value = new Vector2(f, f); }
            public void Value(out Vector3 value) { Value(out float f); value = new Vector3(f, f, f); }
            public void Value(out Vector4 value) { Value(out float f); value = new Vector4(f, f, f, f); }
            public void Value(out Vector2Int value) { Value(out int i); value = new Vector2Int(i, i); }
            public void Value(out Vector3Int value) { Value(out int i); value = new Vector3Int(i, i, i); }
        }

        public struct MinusOne : IConstant<long>, IConstant<int>, IConstant<short>, IConstant<sbyte>, 
                            IConstant<double>, IConstant<float> {
            public void Value(out long value) { value = -1; }
            public void Value(out int value) { value = -1; }
            public void Value(out short value) { value = -1; }
            public void Value(out sbyte value) { value = -1; }
            public void Value(out double value) { value = -1; }
            public void Value(out float value) { value = -1; }
            public void Value(out Vector2 value) { Value(out float f); value = new Vector2(f, f); }
            public void Value(out Vector3 value) { Value(out float f); value = new Vector3(f, f, f); }
            public void Value(out Vector4 value) { Value(out float f); value = new Vector4(f, f, f, f); }
            public void Value(out Vector2Int value) { Value(out int i); value = new Vector2Int(i, i); }
            public void Value(out Vector3Int value) { Value(out int i); value = new Vector3Int(i, i, i); }
        }

        public struct Infinity : IConstant<double>, IConstant<float> {
            public void Value(out double value) { value = double.PositiveInfinity; }
            public void Value(out float value) { value = float.PositiveInfinity; }
            public void Value(out Vector2 value) { Value(out float f); value = new Vector2(f, f); }
            public void Value(out Vector3 value) { Value(out float f); value = new Vector3(f, f, f); }
            public void Value(out Vector4 value) { Value(out float f); value = new Vector4(f, f, f, f); }
        }

        public struct PositiveInfinity : IConstant<double>, IConstant<float> {
            public void Value(out double value) { value = double.PositiveInfinity; }
            public void Value(out float value) { value = float.PositiveInfinity; }
            public void Value(out Vector2 value) { Value(out float f); value = new Vector2(f, f); }
            public void Value(out Vector3 value) { Value(out float f); value = new Vector3(f, f, f); }
            public void Value(out Vector4 value) { Value(out float f); value = new Vector4(f, f, f, f); }
        }

        public struct NegativeInfinity : IConstant<double>, IConstant<float> {
            public void Value(out double value) { value = double.NegativeInfinity; }
            public void Value(out float value) { value = float.NegativeInfinity; }
            public void Value(out Vector2 value) { Value(out float f); value = new Vector2(f, f); }
            public void Value(out Vector3 value) { Value(out float f); value = new Vector3(f, f, f); }
            public void Value(out Vector4 value) { Value(out float f); value = new Vector4(f, f, f, f); }
        }

        public struct NaN : IConstant<double>, IConstant<float> {
            public void Value(out double value) { value = double.NaN; }
            public void Value(out float value) { value = float.NaN; }
            public void Value(out Vector2 value) { Value(out float f); value = new Vector2(f, f); }
            public void Value(out Vector3 value) { Value(out float f); value = new Vector3(f, f, f); }
            public void Value(out Vector4 value) { Value(out float f); value = new Vector4(f, f, f, f); }
        }

        public struct Epsilon : IConstant<double>, IConstant<float> {
            public void Value(out double value) { value = double.Epsilon; }
            public void Value(out float value) { value = float.Epsilon; }
            public void Value(out Vector2 value) { Value(out float f); value = new Vector2(f, f); }
            public void Value(out Vector3 value) { Value(out float f); value = new Vector3(f, f, f); }
            public void Value(out Vector4 value) { Value(out float f); value = new Vector4(f, f, f, f); }
        }

        public struct True : IConstant<bool> {
            public void Value(out bool value) { value = true; }
        }

        public struct False : IConstant<bool> {
            public void Value(out bool value) { value = false; }
        }

    }

    public interface IConstant<T> {
        void Value(out T value);
    }

}