using UnityEngine;

public class DialogIndicationView : MonoBehaviour
{
  [SerializeField]
  private ParticleSystem particleSystem;

  public static DialogIndicationView Create(Transform parent)
  {
    DialogIndicationView indicationPrefab = ScriptableObjectInstance<ResourceFromCodeData>.Instance.DialogIndicationPrefab;
    return indicationPrefab == null ? null : Instantiate(indicationPrefab, parent, false);
  }

  public void SetVisibility(bool value)
  {
    if (!(particleSystem != null))
      return;
    particleSystem.emission.enabled = value;
  }

  public void SetShape(SkinnedMeshRenderer renderer)
  {
    if (!(particleSystem != null))
      return;
    ParticleSystem.ShapeModule shape = particleSystem.shape with
    {
      shapeType = ParticleSystemShapeType.SkinnedMeshRenderer,
      meshShapeType = ParticleSystemMeshShapeType.Vertex,
      skinnedMeshRenderer = renderer
    };
  }

  public void SetShape(MeshRenderer renderer)
  {
    if (!(particleSystem != null))
      return;
    ParticleSystem.ShapeModule shape = particleSystem.shape with
    {
      shapeType = ParticleSystemShapeType.MeshRenderer,
      meshShapeType = ParticleSystemMeshShapeType.Triangle,
      meshRenderer = renderer
    };
    if (renderer.transform.lossyScale.x < 0.0)
      particleSystem.GetComponent<ParticleSystemRenderer>().minParticleSize /= 100000f;
  }
}
