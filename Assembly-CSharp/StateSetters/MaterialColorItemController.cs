// Decompiled with JetBrains decompiler
// Type: StateSetters.MaterialColorItemController
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
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
