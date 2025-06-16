public struct ZoneHit
{
  public float Opacity;
  public PlagueZone PlagueZone;
  public byte Importance;
  public float Level;

  public ZoneHit(PlagueZone plagueZone, float opacity, byte importance, float level)
  {
    PlagueZone = plagueZone;
    Opacity = opacity;
    Importance = importance;
    Level = level;
  }

  public static int Comparison(ZoneHit x, ZoneHit y) => x.Importance.CompareTo(y.Importance);
}
