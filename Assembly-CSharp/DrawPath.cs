using Engine.Source.Components.Utilities;

public class DrawPath : MonoBehaviour
{
  private void Update() => NavMeshUtility.DrawPath(this.gameObject.GetComponent<NavMeshAgent>());
}
