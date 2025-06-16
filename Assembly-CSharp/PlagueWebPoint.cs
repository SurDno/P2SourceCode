using UnityEngine;

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
