public class PlagueWebPoint : IPlagueWebPoint
{
  public PlagueWebCell Cell;
  private Vector3 position;

  public Vector3 Directionality { get; set; }

  public float Strength { get; set; }

  public Vector3 Position
  {
    get => position;
    set
    {
      if (!(position != value))
        return;
      position = value;
      if (Cell != null)
        Cell.PlacePoint(this);
    }
  }
}
