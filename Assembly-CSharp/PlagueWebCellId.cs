using System;
using UnityEngine;

public struct PlagueWebCellId : IEquatable<PlagueWebCellId>
{
  public int X;
  public int Z;

  public PlagueWebCellId(int x, int z)
  {
    X = x;
    Z = z;
  }

  public PlagueWebCellId(Vector3 worldPosition, float cellSize)
  {
    X = Mathf.FloorToInt(worldPosition.x / cellSize);
    Z = Mathf.FloorToInt(worldPosition.z / cellSize);
  }

  public bool Equals(PlagueWebCellId other) => X == other.X && Z == other.Z;

  public override bool Equals(object obj) => obj is PlagueWebCellId other && Equals(other);

  public override int GetHashCode() => X ^ Z;

  public static bool operator ==(PlagueWebCellId a, PlagueWebCellId b) => a.Equals(b);

  public static bool operator !=(PlagueWebCellId a, PlagueWebCellId b) => !a.Equals(b);
}
