namespace SteamNative;

internal struct AccountID_t {
	public uint Value;

	public static implicit operator AccountID_t(uint value) {
		return new AccountID_t { Value = value };
	}

	public static implicit operator uint(AccountID_t value) {
		return value.Value;
	}
}