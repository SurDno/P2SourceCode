using System;
using System.Linq;
using System.Net;

namespace Facepunch.Steamworks
{
  internal static class Utility
  {
    internal static uint SwapBytes(uint x)
    {
      return (uint) ((((int) x & byte.MaxValue) << 24) + (((int) x & 65280) << 8)) + ((x & 16711680U) >> 8) + ((x & 4278190080U) >> 24);
    }

    internal static uint IpToInt32(this IPAddress ipAddress)
    {
      return BitConverter.ToUInt32(ipAddress.GetAddressBytes().Reverse().ToArray(), 0);
    }

    internal static IPAddress Int32ToIp(uint ipAddress)
    {
      return new IPAddress(BitConverter.GetBytes(ipAddress).Reverse().ToArray());
    }

    internal static class Epoch
    {
      private static readonly DateTime epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

      public static int Current => (int) DateTime.UtcNow.Subtract(epoch).TotalSeconds;

      public static DateTime ToDateTime(Decimal unixTime)
      {
        return epoch.AddSeconds((long) unixTime);
      }

      public static uint FromDateTime(DateTime dt)
      {
        return (uint) dt.Subtract(epoch).TotalSeconds;
      }
    }
  }
}
