using System;

namespace Engine.Common.Generator
{
  [Flags]
  public enum TypeEnum
  {
    None = 0,
    Cloneable = 1,
    Copyable = 2,
    EngineCloneable = 4,
    DataRead = 8,
    DataWrite = 16, // 0x00000010
    StateSave = 32, // 0x00000020
    StateLoad = 64, // 0x00000040
    NeedSave = 128, // 0x00000080
  }
}
