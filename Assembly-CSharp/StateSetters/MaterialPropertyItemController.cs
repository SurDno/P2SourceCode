using UnityEngine;

namespace StateSetters
{
  public abstract class MaterialPropertyItemController : IStateSetterItemController
  {
    public void Apply(StateSetterItem item, float value)
    {
      MeshRenderer objectValue1 = item.ObjectValue1 as MeshRenderer;
      if ((Object) objectValue1 == (Object) null)
        return;
      Material[] materialArray = Application.isPlaying ? objectValue1.materials : objectValue1.sharedMaterials;
      int intValue1 = item.IntValue1;
      if (intValue1 < 0 || intValue1 >= materialArray.Length)
        return;
      int intValue2 = item.IntValue2;
      if (intValue2 < 0 || intValue2 >= materialArray.Length)
        return;
      string stringValue1 = item.StringValue1;
      for (int index = intValue1; index <= intValue2; ++index)
      {
        Material material = materialArray[index];
        this.SetParameter(item, material, stringValue1, value);
      }
    }

    protected abstract void SetParameter(
      StateSetterItem item,
      Material material,
      string parameter,
      float value);
  }
}
