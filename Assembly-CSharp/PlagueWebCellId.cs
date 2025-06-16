using System;
using UnityEngine;

public struct PlagueWebCellId : IEquatable<PlagueWebCellId>
{
  public int X;
  public int Z;

  public PlagueWebCellId(int x, int z)
  {
    this.X = x;
    this.Z = z;
  }

  public PlagueWebCellId(Vector3 worldPosition, float cellSize)
  {
    this.X = Mathf.FloorToInt(worldPosition.x / cellSize);
    this.Z = Mathf.FloorToInt(worldPosition.z / cellSize);
  }

  public bool Equals(PlagueWebCellId other) => this.X == other.X && this.Z == other.Z;

  public override bool Equals(object obj) => obj is PlagueWebCellId other && this.Equals(other);

  public override int GetHashCode() => this.X ^ this.Z;

  public static bool operator ==(PlagueWebCellId a, PlagueWebCellId b) => a.Equals(b);

  public static bool operator !=(PlagueWebCellId a, PlagueWebCellId b) => !a.Equals(b);
}
