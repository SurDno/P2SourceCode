namespace SteamNative;

internal struct UGCUpdateHandle_t {
	public ulong Value;

	public static implicit operator UGCUpdateHandle_t(ulong value) {
		return new UGCUpdateHandle_t { Value = value };
	}

	public static implicit operator ulong(UGCUpdateHandle_t value) {
		return value.Value;
	}
}