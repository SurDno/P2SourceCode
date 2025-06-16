// Decompiled with JetBrains decompiler
// Type: Facepunch.Steamworks.Auth
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using SteamNative;
using System;
using System.Collections.Generic;
using System.Linq;

#nullable disable
namespace Facepunch.Steamworks
{
  public class Auth
  {
    internal Client client;

    public unsafe Auth.Ticket GetAuthSessionTicket()
    {
      byte[] source = new byte[1024];
      fixed (byte* pTicket = source)
      {
        uint pcbTicket = 0;
        uint authSessionTicket = (uint) this.client.native.user.GetAuthSessionTicket((IntPtr) (void*) pTicket, source.Length, out pcbTicket);
        if (authSessionTicket == 0U)
          return (Auth.Ticket) null;
        return new Auth.Ticket()
        {
          client = this.client,
          Data = ((IEnumerable<byte>) source).Take<byte>((int) pcbTicket).ToArray<byte>(),
          Handle = authSessionTicket
        };
      }
    }

    public class Ticket : IDisposable
    {
      internal Client client;
      public byte[] Data;
      public uint Handle;

      public void Cancel()
      {
        if (!this.client.IsValid || this.Handle <= 0U)
          return;
        this.client.native.user.CancelAuthTicket((HAuthTicket) this.Handle);
        this.Handle = 0U;
        this.Data = (byte[]) null;
      }

      public void Dispose() => this.Cancel();
    }
  }
}
