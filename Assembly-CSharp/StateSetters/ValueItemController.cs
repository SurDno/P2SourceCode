// Decompiled with JetBrains decompiler
// Type: StateSetters.ValueItemController
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using Engine.Source.Behaviours;
using UnityEngine;

#nullable disable
namespace StateSetters
{
  [StateSetter("{5cdc9818-ebc3-48b6-9e66-821c9641d3bf}")]
  public class ValueItemController : IStateSetterItemController
  {
    public void Apply(StateSetterItem item, float value)
    {
      Object objectValue1 = item.ObjectValue1;
      if (objectValue1 == (Object) null || !(objectValue1 is IValueController valueController))
        return;
      valueController.Value = value;
    }
  }
}
