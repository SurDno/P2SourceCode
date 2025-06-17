using Engine.Common.Types;
using UnityEngine;

public static class EngineExtension
{
  public static Vector2 To(this Position vector) => new(vector.X, vector.Y);

  public static Position To(this Vector2 vector) => new(vector.x, vector.y);
}
