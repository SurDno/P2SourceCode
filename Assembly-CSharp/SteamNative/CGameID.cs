namespace SteamNative;

internal struct CGameID {
	public ulong Value;

	public static implicit operator CGameID(ulong value) {
		return new CGameID { Value = value };
	}

	public static implicit operator ulong(CGameID value) {
		return value.Value;
	}
}