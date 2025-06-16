public struct ZoneHit
{
  public float Opacity;
  public PlagueZone PlagueZone;
  public byte Importance;
  public float Level;

  public ZoneHit(PlagueZone plagueZone, float opacity, byte importance, float level)
  {
    this.PlagueZone = plagueZone;
    this.Opacity = opacity;
    this.Importance = importance;
    this.Level = level;
  }

  public static int Comparison(ZoneHit x, ZoneHit y) => x.Importance.CompareTo(y.Importance);
}
