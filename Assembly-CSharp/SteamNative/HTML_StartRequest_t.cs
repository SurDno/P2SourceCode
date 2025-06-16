// Decompiled with JetBrains decompiler
// Type: SteamNative.HTML_StartRequest_t
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using System;
using System.Runtime.InteropServices;

#nullable disable
namespace SteamNative
{
  [StructLayout(LayoutKind.Sequential, Pack = 8)]
  internal struct HTML_StartRequest_t
  {
    public uint UnBrowserHandle;
    public string PchURL;
    public string PchTarget;
    public string PchPostData;
    [MarshalAs(UnmanagedType.I1)]
    public bool BIsRedirect;

    public static HTML_StartRequest_t FromPointer(IntPtr p)
    {
      return Platform.PackSmall ? (HTML_StartRequest_t) (HTML_StartRequest_t.PackSmall) Marshal.PtrToStructure(p, typeof (HTML_StartRequest_t.PackSmall)) : (HTML_StartRequest_t) Marshal.PtrToStructure(p, typeof (HTML_StartRequest_t));
    }

    [StructLayout(LayoutKind.Sequential, Pack = 4)]
    internal struct PackSmall
    {
      public uint UnBrowserHandle;
      public string PchURL;
      public string PchTarget;
      public string PchPostData;
      [MarshalAs(UnmanagedType.I1)]
      public bool BIsRedirect;

      public static implicit operator HTML_StartRequest_t(HTML_StartRequest_t.PackSmall d)
      {
        return new HTML_StartRequest_t()
        {
          UnBrowserHandle = d.UnBrowserHandle,
          PchURL = d.PchURL,
          PchTarget = d.PchTarget,
          PchPostData = d.PchPostData,
          BIsRedirect = d.BIsRedirect
        };
      }
    }
  }
}
