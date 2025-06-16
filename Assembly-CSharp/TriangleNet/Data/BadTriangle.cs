// Decompiled with JetBrains decompiler
// Type: TriangleNet.Data.BadTriangle
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

#nullable disable
namespace TriangleNet.Data
{
  internal class BadTriangle
  {
    public static int OTID;
    public int ID;
    public double key;
    public BadTriangle nexttriang;
    public Otri poortri;
    public Vertex triangorg;
    public Vertex triangdest;
    public Vertex triangapex;

    public BadTriangle() => this.ID = BadTriangle.OTID++;

    public override string ToString()
    {
      return string.Format("B-TID {0}", (object) this.poortri.triangle.hash);
    }
  }
}
