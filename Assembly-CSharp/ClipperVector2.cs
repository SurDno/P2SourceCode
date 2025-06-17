using System.Collections.Generic;
using ClipperLib;
using UnityEngine;

public static class ClipperVector2
{
  private const int precision = 1000;

  public static Vector2[][] Offset(Vector2[] path, float offset, float arcTolerance)
  {
    ClipperOffset clipperOffset = new ClipperOffset(arcTolerance: arcTolerance * 1000.0);
    List<IntPoint> path1 = new List<IntPoint>(path.Length);
    for (int index = 0; index < path.Length; ++index)
      path1.Add(Vector2ToIntPoint(path[index]));
    clipperOffset.AddPath(path1, JoinType.jtRound, EndType.etClosedPolygon);
    List<List<IntPoint>> solution = [];
    clipperOffset.Execute(ref solution, offset * 1000.0);
    Vector2[][] vector2Array = new Vector2[solution.Count][];
    for (int index1 = 0; index1 < vector2Array.Length; ++index1)
    {
      vector2Array[index1] = new Vector2[solution[index1].Count];
      for (int index2 = 0; index2 < vector2Array[index1].Length; ++index2)
        vector2Array[index1][index2] = IntPointToVector2(solution[index1][index2]);
    }
    return vector2Array;
  }

  private static Vector2 IntPointToVector2(IntPoint input)
  {
    return new Vector2(input.X, input.Y) / 1000f;
  }

  private static IntPoint Vector2ToIntPoint(Vector2 input)
  {
    return new IntPoint(input.x * 1000.0, input.y * 1000.0);
  }
}
