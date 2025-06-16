using UnityEngine;
using UnityEngine.Rendering;

namespace StateSetters
{
  [StateSetter("{FAB40A49-AD2A-423C-8776-A0DBA9B2B9CD}")]
  public class CastShadowsItemController : IStateSetterItemController
  {
    public void Apply(StateSetterItem item, float value)
    {
      MeshRenderer objectValue1 = item.ObjectValue1 as MeshRenderer;
      if (objectValue1 == null)
        return;
      if (value != 0.0)
        objectValue1.shadowCastingMode = (ShadowCastingMode) item.IntValue1;
      else
        objectValue1.shadowCastingMode = (ShadowCastingMode) item.IntValue2;
    }
  }
}
