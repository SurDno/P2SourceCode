using System;

namespace Engine.Common.Generator;

[Flags]
public enum TypeEnum {
	None = 0,
	Cloneable = 1,
	Copyable = 2,
	EngineCloneable = 4,
	DataRead = 8,
	DataWrite = 16,
	StateSave = 32,
	StateLoad = 64,
	NeedSave = 128
}