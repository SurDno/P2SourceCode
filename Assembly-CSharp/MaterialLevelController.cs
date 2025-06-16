[ExecuteInEditMode]
public class MaterialLevelController : MonoBehaviour
{
  private static MaterialPropertyBlock propertyBlock;
  [Range(0.0f, 1f)]
  public float Level = 0.0f;

  private void OnEnable() => UpdateMaterial();

  private void LateUpdate() => UpdateMaterial();

  private void UpdateMaterial()
  {
    Renderer component = this.GetComponent<Renderer>();
    if ((Object) component == (Object) null)
      return;
    if (Level > 0.0)
    {
      if (propertyBlock == null)
        propertyBlock = new MaterialPropertyBlock();
      propertyBlock.SetFloat("_Level", Level);
      component.SetPropertyBlock(propertyBlock);
      component.enabled = true;
    }
    else
      component.enabled = false;
  }
}
