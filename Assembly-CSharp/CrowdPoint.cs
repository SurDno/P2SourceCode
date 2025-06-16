using Engine.Common.Components.Movable;

public class CrowdPoint : MonoBehaviour
{
  [SerializeField]
  [FormerlySerializedAs("Area")]
  private AreaEnum area;
  [SerializeField]
  private bool onNavMesh;

  public AreaEnum Area => area;

  public bool OnNavMesh => onNavMesh;
}
