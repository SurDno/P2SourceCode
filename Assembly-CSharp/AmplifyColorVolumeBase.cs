using AmplifyColor;
using UnityEngine;

[AddComponentMenu("")]
public class AmplifyColorVolumeBase : MonoBehaviour
{
  public Texture2D LutTexture;
  public float Exposure = 1f;
  public float EnterBlendTime = 1f;
  public int Priority;
  public bool ShowInSceneView = true;
  [HideInInspector]
  public VolumeEffectContainer EffectContainer = new();

  private void OnDrawGizmos()
  {
    if (!ShowInSceneView)
      return;
    BoxCollider component = GetComponent<BoxCollider>();
    if (component != null)
    {
      Vector3 center = component.center;
      Vector3 size = component.size;
      Gizmos.color = Color.green;
      Gizmos.matrix = transform.localToWorldMatrix;
      Gizmos.DrawWireCube(center, size);
    }
  }

  private void OnDrawGizmosSelected()
  {
    BoxCollider component = GetComponent<BoxCollider>();
    if (!(component != null))
      return;
    Gizmos.color = Color.green with { a = 0.2f };
    Gizmos.matrix = transform.localToWorldMatrix;
    Gizmos.DrawCube(component.center, component.size);
  }
}
