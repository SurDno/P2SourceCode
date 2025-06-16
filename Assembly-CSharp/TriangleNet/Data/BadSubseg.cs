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
