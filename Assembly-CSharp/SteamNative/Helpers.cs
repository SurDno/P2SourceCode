// Decompiled with JetBrains decompiler
// Type: SteamNative.Helpers
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using System.Text;

#nullable disable
namespace SteamNative
{
  internal static class Helpers
  {
    private static StringBuilder[] StringBuilderPool;
    private static int StringBuilderPoolIndex;

    public static StringBuilder TakeStringBuilder()
    {
      if (Helpers.StringBuilderPool == null)
      {
        Helpers.StringBuilderPool = new StringBuilder[8];
        for (int index = 0; index < Helpers.StringBuilderPool.Length; ++index)
          Helpers.StringBuilderPool[index] = new StringBuilder(4096);
      }
      ++Helpers.StringBuilderPoolIndex;
      if (Helpers.StringBuilderPoolIndex >= Helpers.StringBuilderPool.Length)
        Helpers.StringBuilderPoolIndex = 0;
      Helpers.StringBuilderPool[Helpers.StringBuilderPoolIndex].Capacity = 4096;
      Helpers.StringBuilderPool[Helpers.StringBuilderPoolIndex].Length = 0;
      return Helpers.StringBuilderPool[Helpers.StringBuilderPoolIndex];
    }
  }
}
