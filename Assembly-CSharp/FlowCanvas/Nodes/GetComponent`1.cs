// Decompiled with JetBrains decompiler
// Type: FlowCanvas.Nodes.GetComponent`1
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using ParadoxNotion.Design;
using UnityEngine;

#nullable disable
namespace FlowCanvas.Nodes
{
  [Category("Functions/Utility")]
  public class GetComponent<T> : PureFunctionNode<T, GameObject> where T : Component
  {
    private T _component;

    public override T Invoke(GameObject gameObject)
    {
      if ((Object) gameObject == (Object) null)
        return default (T);
      if ((Object) this._component == (Object) null || (Object) this._component.gameObject != (Object) gameObject)
        this._component = gameObject.GetComponent<T>();
      return this._component;
    }
  }
}
