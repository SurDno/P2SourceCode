using System;

namespace ClipperLib;

internal struct Int128 {
	private long hi;
	private ulong lo;

	public Int128(long _lo) {
		lo = (ulong)_lo;
		if (_lo < 0L)
			hi = -1L;
		else
			hi = 0L;
	}

	public Int128(long _hi, ulong _lo) {
		lo = _lo;
		hi = _hi;
	}

	public Int128(Int128 val) {
		hi = val.hi;
		lo = val.lo;
	}

	public bool IsNegative() {
		return hi < 0L;
	}

	public static bool operator ==(Int128 val1, Int128 val2) {
		if ((ValueType)val1 == (ValueType)val2)
			return true;
		return (ValueType)val1 != null && (ValueType)val2 != null && val1.hi == val2.hi &&
		       (long)val1.lo == (long)val2.lo;
	}

	public static bool operator !=(Int128 val1, Int128 val2) {
		return !(val1 == val2);
	}

	public override bool Equals(object obj) {
		if (obj == null || !(obj is Int128))
			return false;
		var int128 = (Int128)obj;
		return int128.hi == hi && (long)int128.lo == (long)lo;
	}

	public override int GetHashCode() {
		return hi.GetHashCode() ^ lo.GetHashCode();
	}

	public static bool operator >(Int128 val1, Int128 val2) {
		return val1.hi != val2.hi ? val1.hi > val2.hi : val1.lo > val2.lo;
	}

	public static bool operator <(Int128 val1, Int128 val2) {
		return val1.hi != val2.hi ? val1.hi < val2.hi : val1.lo < val2.lo;
	}

	public static Int128 operator +(Int128 lhs, Int128 rhs) {
		lhs.hi += rhs.hi;
		lhs.lo += rhs.lo;
		if (lhs.lo < rhs.lo)
			++lhs.hi;
		return lhs;
	}

	public static Int128 operator -(Int128 lhs, Int128 rhs) {
		return lhs + -rhs;
	}

	public static Int128 operator -(Int128 val) {
		return val.lo == 0UL ? new Int128(-val.hi, 0UL) : new Int128(~val.hi, ~val.lo + 1UL);
	}

	public static explicit operator double(Int128 val) {
		if (val.hi >= 0L)
			return val.lo + val.hi * 1.8446744073709552E+19;
		return val.lo == 0UL ? val.hi * 1.8446744073709552E+19 : -(~val.lo + ~val.hi * 1.8446744073709552E+19);
	}

	public static Int128 Int128Mul(long lhs, long rhs) {
		var flag = lhs < 0L != rhs < 0L;
		if (lhs < 0L)
			lhs = -lhs;
		if (rhs < 0L)
			rhs = -rhs;
		var num1 = (ulong)(lhs >>> 32);
		var num2 = (ulong)lhs & uint.MaxValue;
		var num3 = (ulong)(rhs >>> 32);
		var num4 = (ulong)rhs & uint.MaxValue;
		var num5 = num1 * num3;
		var num6 = num2 * num4;
		var num7 = (ulong)((long)num1 * (long)num4 + (long)num2 * (long)num3);
		var _hi = (long)num5 + (long)(num7 >> 32);
		var _lo = (num7 << 32) + num6;
		if (_lo < num6)
			++_hi;
		var int128 = new Int128(_hi, _lo);
		return flag ? -int128 : int128;
	}
}