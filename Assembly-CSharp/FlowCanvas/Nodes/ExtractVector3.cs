namespace FlowCanvas.Nodes
{
  public class ExtractVector3 : ExtractorNode<Vector3, float, float, float>
  {
    public override void Invoke(Vector3 vector, out float x, out float y, out float z)
    {
      x = vector.x;
      y = vector.y;
      z = vector.z;
    }
  }
}
