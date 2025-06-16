// Decompiled with JetBrains decompiler
// Type: StateSetters.MaterialFloatItemController
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
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
