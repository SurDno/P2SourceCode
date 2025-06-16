// Decompiled with JetBrains decompiler
// Type: SteamNative.SteamItemDetails_t
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using System;
using System.Runtime.InteropServices;

#nullable disable
namespace SteamNative
{
  [StructLayout(LayoutKind.Sequential, Pack = 8)]
  internal struct SteamItemDetails_t
  {
    public ulong ItemId;
    public int Definition;
    public ushort Quantity;
    public ushort Flags;

    public static SteamItemDetails_t FromPointer(IntPtr p)
    {
      return Platform.PackSmall ? (SteamItemDetails_t) (SteamItemDetails_t.PackSmall) Marshal.PtrToStructure(p, typeof (SteamItemDetails_t.PackSmall)) : (SteamItemDetails_t) Marshal.PtrToStructure(p, typeof (SteamItemDetails_t));
    }

    [StructLayout(LayoutKind.Sequential, Pack = 4)]
    internal struct PackSmall
    {
      public ulong ItemId;
      public int Definition;
      public ushort Quantity;
      public ushort Flags;

      public static implicit operator SteamItemDetails_t(SteamItemDetails_t.PackSmall d)
      {
        return new SteamItemDetails_t()
        {
          ItemId = d.ItemId,
          Definition = d.Definition,
          Quantity = d.Quantity,
          Flags = d.Flags
        };
      }
    }
  }
}
