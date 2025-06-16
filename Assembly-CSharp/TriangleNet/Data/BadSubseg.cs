// Decompiled with JetBrains decompiler
// Type: TriangleNet.Data.BadSubseg
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

#nullable disable
namespace TriangleNet.Data
{
  internal class BadSubseg
  {
    private static int hashSeed;
    public Osub encsubseg;
    internal int Hash;
    public Vertex subsegorg;
    public Vertex subsegdest;

    public BadSubseg() => this.Hash = BadSubseg.hashSeed++;

    public override int GetHashCode() => this.Hash;

    public override string ToString()
    {
      return string.Format("B-SID {0}", (object) this.encsubseg.seg.hash);
    }
  }
}
