using System;

namespace Engine.Common.DateTime;

[Flags]
public enum TimesOfDay {
	None = 0,
	Morning = 1,
	Day = 2,
	Evening = 4,
	Night = 8
}