﻿using System.Collections.Generic;
using UnityEngine.SceneManagement;

namespace Engine.Source.Comparers
{
  public class SceneComparer : IEqualityComparer<Scene>
  {
    public static readonly SceneComparer Instance = new();

    public bool Equals(Scene x, Scene y) => x == y;

    public int GetHashCode(Scene obj) => obj.GetHashCode();
  }
}
