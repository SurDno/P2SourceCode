using UnityEngine;

namespace StateSetters
{
  [StateSetter("{8E333D23-032D-4598-89D7-25C6D94A20B4}")]
  public class ShowGameObjectItemController : IStateSetterItemController
  {
    public void Apply(StateSetterItem item, float value)
    {
      GameObject objectValue1 = item.ObjectValue1 as GameObject;
      if ((Object) objectValue1 == (Object) null)
        return;
      bool flag = item.BoolValue1 != ((double) value != 0.0);
      objectValue1.SetActive(flag);
    }
  }
}
