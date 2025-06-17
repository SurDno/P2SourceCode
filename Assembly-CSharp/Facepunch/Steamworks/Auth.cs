using System;
using System.Linq;

namespace Facepunch.Steamworks
{
  public class Auth
  {
    internal Client client;

    public unsafe Ticket GetAuthSessionTicket()
    {
      byte[] source = new byte[1024];
      fixed (byte* pTicket = source)
      {
        uint authSessionTicket = client.native.user.GetAuthSessionTicket((IntPtr) pTicket, source.Length, out uint pcbTicket);
        if (authSessionTicket == 0U)
          return null;
        return new Ticket {
          client = client,
          Data = source.Take((int) pcbTicket).ToArray(),
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
        if (!client.IsValid || Handle <= 0U)
          return;
        client.native.user.CancelAuthTicket(Handle);
        Handle = 0U;
        Data = null;
      }

      public void Dispose() => Cancel();
    }
  }
}
