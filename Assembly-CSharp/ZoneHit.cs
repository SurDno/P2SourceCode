public struct ZoneHit(PlagueZone plagueZone, float opacity, byte importance, float level) {
  public float Opacity = opacity;
  public PlagueZone PlagueZone = plagueZone;
  public byte Importance = importance;
  public float Level = level;

  public static int Comparison(ZoneHit x, ZoneHit y) => x.Importance.CompareTo(y.Importance);
}
