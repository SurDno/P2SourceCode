using System;

namespace Engine.Source.Utility;

[Flags]
public enum KeyModifficator {
	None = 0,
	Control = 1,
	Shift = 2,
	Alt = 4
}