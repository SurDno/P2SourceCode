// Decompiled with JetBrains decompiler
// Type: UnityHeapCrawler.TypeSizeModeEx
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using System;

#nullable disable
namespace UnityHeapCrawler
{
  public static class TypeSizeModeEx
  {
    public static long GetSize(this TypeSizeMode mode, TypeStats typeStats)
    {
      switch (mode)
      {
        case TypeSizeMode.Self:
          return typeStats.SelfSize;
        case TypeSizeMode.Total:
          return typeStats.TotalSize;
        case TypeSizeMode.Native:
          return typeStats.NativeSize;
        default:
          throw new ArgumentOutOfRangeException(nameof (mode), (object) mode, (string) null);
      }
    }
  }
}
