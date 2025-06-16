// Decompiled with JetBrains decompiler
// Type: FlowCanvas.Nodes.GetRandomListItem`1
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using ParadoxNotion.Design;
using System.Collections.Generic;

#nullable disable
namespace FlowCanvas.Nodes
{
  [Category("Functions/Lists")]
  public class GetRandomListItem<T> : PureFunctionNode<T, IList<T>>
  {
    public override T Invoke(IList<T> list)
    {
      return list.Count > 0 ? list[UnityEngine.Random.Range(0, list.Count)] : default (T);
    }
  }
}
