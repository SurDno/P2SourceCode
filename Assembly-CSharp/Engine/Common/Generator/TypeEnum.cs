// Decompiled with JetBrains decompiler
// Type: Engine.Common.Generator.TypeEnum
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

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
