// Decompiled with JetBrains decompiler
// Type: PlagueWebCellId
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using System;
using UnityEngine;

#nullable disable
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
