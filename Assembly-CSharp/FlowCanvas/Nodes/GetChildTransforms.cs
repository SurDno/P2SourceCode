// Decompiled with JetBrains decompiler
// Type: FlowCanvas.Nodes.GetChildTransforms
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using ParadoxNotion.Design;
using System.Collections;
using System.Linq;
using UnityEngine;

#nullable disable
namespace FlowCanvas.Nodes
{
  [Category("Functions/Utility")]
  public class GetChildTransforms : PureFunctionNode<Transform[], Transform>
  {
    public override Transform[] Invoke(Transform parent)
    {
      return ((IEnumerable) parent.transform).Cast<Transform>().ToArray<Transform>();
    }
  }
}
