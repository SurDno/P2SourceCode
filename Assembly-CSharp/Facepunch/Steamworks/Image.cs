// Decompiled with JetBrains decompiler
// Type: Facepunch.Steamworks.Image
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using SteamNative;
using System;

#nullable disable
namespace Facepunch.Steamworks
{
  public class Image
  {
    public int Id { get; internal set; }

    public int Width { get; internal set; }

    public int Height { get; internal set; }

    public byte[] Data { get; internal set; }

    public bool IsLoaded { get; internal set; }

    public bool IsError { get; internal set; }

    internal unsafe bool TryLoad(SteamUtils utils)
    {
      if (this.IsLoaded)
        return true;
      uint pnWidth = 0;
      uint pnHeight = 0;
      if (!utils.GetImageSize(this.Id, out pnWidth, out pnHeight))
      {
        this.IsError = true;
        return true;
      }
      byte[] numArray = new byte[(int) pnWidth * (int) pnHeight * 4];
      fixed (byte* pubDest = numArray)
      {
        if (!utils.GetImageRGBA(this.Id, (IntPtr) (void*) pubDest, numArray.Length))
        {
          this.IsError = true;
          return true;
        }
        // ISSUE: __unpin statement
        __unpin(pubDest);
        this.Width = (int) pnWidth;
        this.Height = (int) pnHeight;
        this.Data = numArray;
        this.IsLoaded = true;
        this.IsError = false;
        return true;
      }
    }

    public Color GetPixel(int x, int y)
    {
      if (!this.IsLoaded)
        throw new Exception("Image not loaded");
      if (x < 0 || x >= this.Width)
        throw new Exception("x out of bounds");
      if (y < 0 || y >= this.Height)
        throw new Exception("y out of bounds");
      Color pixel = new Color();
      int index = (y * this.Width + x) * 4;
      pixel.r = this.Data[index];
      pixel.g = this.Data[index + 1];
      pixel.b = this.Data[index + 2];
      pixel.a = this.Data[index + 3];
      return pixel;
    }
  }
}
