public class lerper
{
  public static float lerp_safe(float l, float p1, float p2)
  {
    if (p1 <= (double) p2)
      return p1 + (p2 - p1) * l;
    l = 1f - l;
    return p2 + (p1 - p2) * l;
  }
}
