using System;

namespace Facepunch.Steamworks
{
  public class Screenshots
  {
    internal Client client;

    internal Screenshots(Client c) => client = c;

    public void Trigger() => client.native.screenshots.TriggerScreenshot();

    public unsafe void Write(byte[] rgbData, int width, int height)
    {
      if (rgbData == null)
        throw new ArgumentNullException(nameof (rgbData));
      if (width < 1)
        throw new ArgumentOutOfRangeException(nameof (width), width, "Expected width to be at least 1.");
      if (height < 1)
        throw new ArgumentOutOfRangeException(nameof (height), height, "Expected height to be at least 1.");
      int num = width * height * 3;
      if (rgbData.Length < num)
        throw new ArgumentException(nameof (rgbData), string.Format("Expected {0} to contain at least {1} elements (actual size: {2}).", nameof (rgbData), num, rgbData.Length));
      fixed (byte* pubRGB = rgbData)
        client.native.screenshots.WriteScreenshot((IntPtr) pubRGB, (uint) rgbData.Length, width, height);
    }
  }
}
