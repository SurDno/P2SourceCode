﻿namespace SteamNative;

internal struct ControllerActionSetHandle_t {
	public ulong Value;

	public static implicit operator ControllerActionSetHandle_t(ulong value) {
		return new ControllerActionSetHandle_t {
			Value = value
		};
	}

	public static implicit operator ulong(ControllerActionSetHandle_t value) {
		return value.Value;
	}
}