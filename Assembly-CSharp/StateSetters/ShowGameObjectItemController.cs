// Decompiled with JetBrains decompiler
// Type: StateSetters.ShowGameObjectItemController
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
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
