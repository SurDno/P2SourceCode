// Decompiled with JetBrains decompiler
// Type: ClipperLib.IntRect
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

#nullable disable
namespace ClipperLib
{
  public struct IntRect
  {
    public long left;
    public long top;
    public long right;
    public long bottom;

    public IntRect(long l, long t, long r, long b)
    {
      this.left = l;
      this.top = t;
      this.right = r;
      this.bottom = b;
    }

    public IntRect(IntRect ir)
    {
      this.left = ir.left;
      this.top = ir.top;
      this.right = ir.right;
      this.bottom = ir.bottom;
    }
  }
}
