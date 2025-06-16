namespace SteamNative;

internal struct ClientUnifiedMessageHandle {
	public ulong Value;

	public static implicit operator ClientUnifiedMessageHandle(ulong value) {
		return new ClientUnifiedMessageHandle {
			Value = value
		};
	}

	public static implicit operator ulong(ClientUnifiedMessageHandle value) {
		return value.Value;
	}
}