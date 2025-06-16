public class DialogIndicationView : MonoBehaviour
{
  [SerializeField]
  private ParticleSystem particleSystem;

  public static DialogIndicationView Create(Transform parent)
  {
    DialogIndicationView indicationPrefab = ScriptableObjectInstance<ResourceFromCodeData>.Instance.DialogIndicationPrefab;
    return (Object) indicationPrefab == (Object) null ? (DialogIndicationView) null : Object.Instantiate<DialogIndicationView>(indicationPrefab, parent, false);
  }

  public void SetVisibility(bool value)
  {
    if (!((Object) particleSystem != (Object) null))
      return;
    particleSystem.emission.enabled = value;
  }

  public void SetShape(SkinnedMeshRenderer renderer)
  {
    if (!((Object) particleSystem != (Object) null))
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
    if (!((Object) particleSystem != (Object) null))
      return;
    ParticleSystem.ShapeModule shape = particleSystem.shape with
    {
      shapeType = ParticleSystemShapeType.MeshRenderer,
      meshShapeType = ParticleSystemMeshShapeType.Triangle,
      meshRenderer = renderer
    };
    if ((double) renderer.transform.lossyScale.x < 0.0)
      particleSystem.GetComponent<ParticleSystemRenderer>().minParticleSize /= 100000f;
  }
}
