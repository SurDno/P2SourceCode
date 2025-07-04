﻿using System;
using UnityEngine;

public struct PlagueWebCellId(int x, int z) : IEquatable<PlagueWebCellId> {
  public int X = x;
  public int Z = z;

  public PlagueWebCellId(Vector3 worldPosition, float cellSize) : this(Mathf.FloorToInt(worldPosition.x / cellSize), Mathf.FloorToInt(worldPosition.z / cellSize)) { }

  public bool Equals(PlagueWebCellId other) => X == other.X && Z == other.Z;

  public override bool Equals(object obj) => obj is PlagueWebCellId other && Equals(other);

  public override int GetHashCode() => X ^ Z;

  public static bool operator ==(PlagueWebCellId a, PlagueWebCellId b) => a.Equals(b);

  public static bool operator !=(PlagueWebCellId a, PlagueWebCellId b) => !a.Equals(b);
}
