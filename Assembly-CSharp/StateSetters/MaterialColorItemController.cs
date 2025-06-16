using UnityEngine;

namespace StateSetters
{
  [StateSetter("{B749D8EE-A73C-4CC1-B7A1-7FAF349AFF78}")]
  public class MaterialColorItemController : MaterialPropertyItemController
  {
    protected override void SetParameter(
      StateSetterItem item,
      Material material,
      string parameter,
      float value)
    {
      Color color = Color.Lerp(item.ColorValue1, item.ColorValue2, value);
      material.SetColor(parameter, color);
    }
  }
}
