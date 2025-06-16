namespace SoundPropagation
{
  public class SPPortal : MonoBehaviour
  {
    public SPCell CellA;
    public SPCell CellB;
    public float Occlusion = 0.0f;
    private bool initialized;
    private Shape[] shapes = null;

    private void Check()
    {
      if (initialized)
        return;
      shapes = this.GetComponentsInChildren<Shape>();
      initialized = true;
    }

    public bool ClosestPointToSegment(Vector3 pointA, Vector3 pointB, out Vector3 output)
    {
      Check();
      if (shapes.Length < 1)
      {
        output = Vector3.zero;
        return false;
      }
      if (shapes.Length == 1)
        return shapes[0].ClosestPointToSegment(pointA, pointB, out output);
      float num1 = float.MaxValue;
      Vector3 vector3 = Vector3.zero;
      bool segment = false;
      for (int index = 0; index < shapes.Length; ++index)
      {
        Vector3 output1;
        if (shapes[0].ClosestPointToSegment(pointA, pointB, out output1))
        {
          float num2 = Vector3.Distance(pointA, output1) + Vector3.Distance(output1, pointB);
          if (num2 < (double) num1)
          {
            vector3 = output1;
            num1 = num2;
            segment = true;
          }
        }
      }
      output = vector3;
      return segment;
    }

    public float Loss => Occlusion;
  }
}
