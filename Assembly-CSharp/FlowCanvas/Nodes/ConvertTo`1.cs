// Decompiled with JetBrains decompiler
// Type: FlowCanvas.Nodes.ConvertTo`1
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using ParadoxNotion.Design;
using System;

#nullable disable
namespace FlowCanvas.Nodes
{
  [Category("Utilities/Converters")]
  public class ConvertTo<T> : PureFunctionNode<T, IConvertible> where T : IConvertible
  {
    public override T Invoke(IConvertible obj) => (T) Convert.ChangeType((object) obj, typeof (T));
  }
}
