// Decompiled with JetBrains decompiler
// Type: FlowCanvas.Nodes.ToList`1
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using ParadoxNotion.Design;
using System.Collections.Generic;
using System.Linq;

#nullable disable
namespace FlowCanvas.Nodes
{
  [Category("Utilities/Converters")]
  public class ToList<T> : PureFunctionNode<List<T>, IList<T>>
  {
    public override List<T> Invoke(IList<T> list) => list.ToList<T>();
  }
}
