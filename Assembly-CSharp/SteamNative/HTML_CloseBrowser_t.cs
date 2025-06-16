// Decompiled with JetBrains decompiler
// Type: SteamNative.HTML_CloseBrowser_t
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using System;
using System.Runtime.InteropServices;

#nullable disable
namespace SteamNative
{
  [StructLayout(LayoutKind.Sequential, Pack = 8)]
  internal struct HTML_CloseBrowser_t
  {
    public uint UnBrowserHandle;

    public static HTML_CloseBrowser_t FromPointer(IntPtr p)
    {
      return Platform.PackSmall ? (HTML_CloseBrowser_t) (HTML_CloseBrowser_t.PackSmall) Marshal.PtrToStructure(p, typeof (HTML_CloseBrowser_t.PackSmall)) : (HTML_CloseBrowser_t) Marshal.PtrToStructure(p, typeof (HTML_CloseBrowser_t));
    }

    [StructLayout(LayoutKind.Sequential, Pack = 4)]
    internal struct PackSmall
    {
      public uint UnBrowserHandle;

      public static implicit operator HTML_CloseBrowser_t(HTML_CloseBrowser_t.PackSmall d)
      {
        return new HTML_CloseBrowser_t()
        {
          UnBrowserHandle = d.UnBrowserHandle
        };
      }
    }
  }
}
