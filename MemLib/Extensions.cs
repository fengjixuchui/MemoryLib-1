using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.Linq;

namespace MemLib {
    public static class LinqExtensions {

        public static string ToString<T>(this IEnumerable<T> array) where T : struct {
            return array.ToString("");
        }

        public static string ToString<T>(this IEnumerable<T> array, string format, string seperator = null) where T : struct {
            if (array == null)
                return string.Empty;
            var tostr = typeof(T).GetMethod("ToString", new[] {typeof(string)});
            return string.Join(seperator ?? string.Empty,
                array.Select(v => {
                    if (string.IsNullOrEmpty(format) || tostr == null)
                        return v.ToString();
                    return tostr.Invoke(v, new object[] {format});
                }));
        }
    }

    public static class IntPtrExtensions {
   		[Pure]
		[DebuggerStepThrough]
		public static IntPtr Add(this IntPtr left, IntPtr right) {
            if (IntPtr.Size > 4)
                return new IntPtr(left.ToInt64() + right.ToInt64());
			return new IntPtr(left.ToInt32() + right.ToInt32());
		}

		[Pure]
		[DebuggerStepThrough]
		public static IntPtr Sub(this IntPtr left, IntPtr right) {
            if (IntPtr.Size > 4)
                return new IntPtr(left.ToInt64() - right.ToInt64());
			return new IntPtr(left.ToInt32() - right.ToInt32());
		}

		[Pure]
		[DebuggerStepThrough]
		public static IntPtr Mul(this IntPtr left, IntPtr right) {
            if (IntPtr.Size > 4)
                return new IntPtr(left.ToInt64() * right.ToInt64());
			return new IntPtr(left.ToInt32() * right.ToInt32());
		}

		[Pure]
		[DebuggerStepThrough]
		public static IntPtr Div(this IntPtr left, IntPtr right) {
            if (IntPtr.Size > 4)
                return new IntPtr(left.ToInt64() / right.ToInt64());
			return new IntPtr(left.ToInt32() / right.ToInt32());
		}

		[Pure]
		[DebuggerStepThrough]
		public static int Mod(this IntPtr ptr, int mod) {
            if (IntPtr.Size > 4)
			    return (int)(ptr.ToInt64() % mod);
			return ptr.ToInt32() % mod;
		}

        [Pure]
        [DebuggerStepThrough]
        public static bool InRange(this IntPtr address, IntPtr start, IntPtr end) {
            if (IntPtr.Size > 4) {
                var val = (ulong) address.ToInt64();
                return (ulong) start.ToInt64() <= val && val <= (ulong) end.ToInt64();
            } else {
                var val = (uint)address.ToInt32();
                return (uint) start.ToInt32() <= val && val <= (uint) end.ToInt32();
            }
        }

		[Pure]
		[DebuggerStepThrough]
		public static int CompareTo(this IntPtr left, IntPtr right) {
            if (IntPtr.Size > 4)
                return ((ulong)left.ToInt64()).CompareTo((ulong)right.ToInt64());
			return ((uint)left.ToInt32()).CompareTo((uint)right.ToInt32());
		}

        [Pure]
		[DebuggerStepThrough]
		public static int CompareToRange(this IntPtr address, IntPtr start, IntPtr end) {
			if (InRange(address, start, end))
				return 0;
			return CompareTo(address, start);
		}

        [Pure]
        [DebuggerStepThrough]
        public static bool IsNull(this IntPtr ptr) {
            return ptr == IntPtr.Zero;
        }

        [Pure]
        [DebuggerStepThrough]
        public static bool MayBeValid(this IntPtr ptr) {
            return ptr.InRange((IntPtr) 0x10000, IntPtr.Size > 4 ? (IntPtr)long.MaxValue : (IntPtr)int.MaxValue);
        }
    }
}