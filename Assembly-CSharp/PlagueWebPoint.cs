// Decompiled with JetBrains decompiler
// Type: PlagueWebPoint
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
public class PlagueWebPoint : IPlagueWebPoint
{
  public PlagueWebCell Cell;
  private Vector3 position;

  public Vector3 Directionality { get; set; }

  public float Strength { get; set; }

  public Vector3 Position
  {
    get => this.position;
    set
    {
      if (!(this.position != value))
        return;
      this.position = value;
      if (this.Cell != null)
        this.Cell.PlacePoint(this);
    }
  }
}
