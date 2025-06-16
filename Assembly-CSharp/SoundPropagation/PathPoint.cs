namespace SoundPropagation
{
  public struct PathPoint
  {
    public Vector3 Position;
    public Vector3 Direction;
    public float StepLength;
    public SPPortal Portal;
    public SPCell Cell;
  }
}
