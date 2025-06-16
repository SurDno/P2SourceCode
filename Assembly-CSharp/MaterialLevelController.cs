using UnityEngine;

[ExecuteInEditMode]
public class MaterialLevelController : MonoBehaviour
{
  private static MaterialPropertyBlock propertyBlock;
  [Range(0.0f, 1f)]
  public float Level = 0.0f;

  private void OnEnable() => this.UpdateMaterial();

  private void LateUpdate() => this.UpdateMaterial();

  private void UpdateMaterial()
  {
    Renderer component = this.GetComponent<Renderer>();
    if ((Object) component == (Object) null)
      return;
    if ((double) this.Level > 0.0)
    {
      if (MaterialLevelController.propertyBlock == null)
        MaterialLevelController.propertyBlock = new MaterialPropertyBlock();
      MaterialLevelController.propertyBlock.SetFloat("_Level", this.Level);
      component.SetPropertyBlock(MaterialLevelController.propertyBlock);
      component.enabled = true;
    }
    else
      component.enabled = false;
  }
}
