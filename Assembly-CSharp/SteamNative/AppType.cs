﻿namespace SteamNative;

internal enum AppType {
	DepotOnly = -2147483648,
	Invalid = 0,
	Game = 1,
	Application = 2,
	Tool = 4,
	Demo = 8,
	Media_DEPRECATED = 16,
	DLC = 32,
	Guide = 64,
	Driver = 128,
	Config = 256,
	Hardware = 512,
	Franchise = 1024,
	Video = 2048,
	Plugin = 4096,
	Music = 8192,
	Series = 16384,
	Shortcut = 1073741824
}