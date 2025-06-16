using UnityEngine;

namespace StateSetters
{
  [StateSetter("{1EC03228-53CC-4E8C-B2BC-FBFB513323CC}")]
  public class MaterialFloatItemController : MaterialPropertyItemController
  {
    protected override void SetParameter(
      StateSetterItem item,
      Material material,
      string parameter,
      float value)
    {
      float num = Mathf.Lerp(item.FloatValue1, item.FloatValue2, value);
      material.SetFloat(parameter, num);
    }
  }
}
