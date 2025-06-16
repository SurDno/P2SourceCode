// Decompiled with JetBrains decompiler
// Type: FlowCanvas.Nodes.GetListItem`1
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using ParadoxNotion.Design;
using System.Collections.Generic;

#nullable disable
namespace FlowCanvas.Nodes
{
  [Category("Functions/Lists")]
  public class GetListItem<T> : PureFunctionNode<T, IList<T>, int>
  {
    public override T Invoke(IList<T> list, int index)
    {
      try
      {
        return list[index];
      }
      catch
      {
        return default (T);
      }
    }
  }
}
