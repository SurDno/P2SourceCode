// Decompiled with JetBrains decompiler
// Type: FlowCanvas.Nodes.SetListItem`1
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using ParadoxNotion.Design;
using System.Collections.Generic;

#nullable disable
namespace FlowCanvas.Nodes
{
  [Category("Functions/Lists")]
  public class SetListItem<T> : CallableFunctionNode<IList<T>, IList<T>, int, T>
  {
    public override IList<T> Invoke(IList<T> list, int index, T item)
    {
      try
      {
        list[index] = item;
        return list;
      }
      catch
      {
        return (IList<T>) null;
      }
    }
  }
}
