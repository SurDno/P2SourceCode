// Decompiled with JetBrains decompiler
// Type: StateSetters.CastShadowsItemController
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using UnityEngine;
using UnityEngine.Rendering;

#nullable disable
namespace StateSetters
{
  [StateSetter("{FAB40A49-AD2A-423C-8776-A0DBA9B2B9CD}")]
  public class CastShadowsItemController : IStateSetterItemController
  {
    public void Apply(StateSetterItem item, float value)
    {
      MeshRenderer objectValue1 = item.ObjectValue1 as MeshRenderer;
      if ((Object) objectValue1 == (Object) null)
        return;
      if ((double) value != 0.0)
        objectValue1.shadowCastingMode = (ShadowCastingMode) item.IntValue1;
      else
        objectValue1.shadowCastingMode = (ShadowCastingMode) item.IntValue2;
    }
  }
}
