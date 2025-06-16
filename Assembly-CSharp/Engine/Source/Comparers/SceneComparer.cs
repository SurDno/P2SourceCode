using System.Collections.Generic;

namespace Engine.Source.Comparers
{
  public class SceneComparer : IEqualityComparer<Scene>
  {
    public static readonly SceneComparer Instance = new SceneComparer();

    public bool Equals(Scene x, Scene y) => x == y;

    public int GetHashCode(Scene obj) => obj.GetHashCode();
  }
}
