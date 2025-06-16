namespace FlowCanvas.Nodes
{
  public class ExtractRaycastHit : ExtractorNode<RaycastHit, GameObject, float, Vector3, Vector3>
  {
    public override void Invoke(
      RaycastHit hit,
      out GameObject gameObject,
      out float distance,
      out Vector3 normal,
      out Vector3 point)
    {
      gameObject = hit.collider?.gameObject;
      distance = hit.distance;
      normal = hit.normal;
      point = hit.point;
    }
  }
}
