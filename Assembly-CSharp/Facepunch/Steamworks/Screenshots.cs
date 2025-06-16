// Decompiled with JetBrains decompiler
// Type: Facepunch.Steamworks.Screenshots
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using System;

#nullable disable
namespace Facepunch.Steamworks
{
  public class Screenshots
  {
    internal Client client;

    internal Screenshots(Client c) => this.client = c;

    public void Trigger() => this.client.native.screenshots.TriggerScreenshot();

    public unsafe void Write(byte[] rgbData, int width, int height)
    {
      if (rgbData == null)
        throw new ArgumentNullException(nameof (rgbData));
      if (width < 1)
        throw new ArgumentOutOfRangeException(nameof (width), (object) width, "Expected width to be at least 1.");
      if (height < 1)
        throw new ArgumentOutOfRangeException(nameof (height), (object) height, "Expected height to be at least 1.");
      int num = width * height * 3;
      if (rgbData.Length < num)
        throw new ArgumentException(nameof (rgbData), string.Format("Expected {0} to contain at least {1} elements (actual size: {2}).", (object) nameof (rgbData), (object) num, (object) rgbData.Length));
      fixed (byte* pubRGB = rgbData)
        this.client.native.screenshots.WriteScreenshot((IntPtr) (void*) pubRGB, (uint) rgbData.Length, width, height);
    }
  }
}
