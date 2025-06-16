namespace SoundPropagation
{
  public static class Math
  {
    public static Vector3 DirectionalityToDirection(Vector3 directionality, Vector3 fallback)
    {
      float magnitude = directionality.magnitude;
      if (magnitude == 0.0)
        return fallback;
      directionality /= magnitude;
      return Vector3.Lerp(fallback, directionality, magnitude);
    }

    public static float Normalize(ref Vector3 vector)
    {
      float magnitude = vector.magnitude;
      if (magnitude > 0.0)
      {
        vector /= magnitude;
      }
      else
      {
        vector.x = 0.0f;
        vector.y = 0.0f;
        vector.z = 0.0f;
      }
      return magnitude;
    }
  }
}
