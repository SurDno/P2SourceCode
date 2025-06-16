using AmplifyColor;

[AddComponentMenu("")]
public class AmplifyColorVolumeBase : MonoBehaviour
{
  public Texture2D LutTexture;
  public float Exposure = 1f;
  public float EnterBlendTime = 1f;
  public int Priority = 0;
  public bool ShowInSceneView = true;
  [HideInInspector]
  public VolumeEffectContainer EffectContainer = new VolumeEffectContainer();

  private void OnDrawGizmos()
  {
    if (!ShowInSceneView)
      return;
    BoxCollider component = this.GetComponent<BoxCollider>();
    if ((Object) component != (Object) null)
    {
      Vector3 center = component.center;
      Vector3 size = component.size;
      Gizmos.color = Color.green;
      Gizmos.matrix = this.transform.localToWorldMatrix;
      Gizmos.DrawWireCube(center, size);
    }
  }

  private void OnDrawGizmosSelected()
  {
    BoxCollider component = this.GetComponent<BoxCollider>();
    if (!((Object) component != (Object) null))
      return;
    Gizmos.color = Color.green with { a = 0.2f };
    Gizmos.matrix = this.transform.localToWorldMatrix;
    Gizmos.DrawCube(component.center, component.size);
  }
}
