// Decompiled with JetBrains decompiler
// Type: FlowCanvas.Nodes.AnyGreaterEqualThan
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using ParadoxNotion.Design;
using System;

#nullable disable
namespace FlowCanvas.Nodes
{
  [Category("Functions/Math Operators/Any")]
  [Name(">=")]
  public class AnyGreaterEqualThan : PureFunctionNode<bool, IComparable, IComparable>
  {
    public override bool Invoke(IComparable a, IComparable b)
    {
      return a.CompareTo((object) b) == 1 || object.Equals((object) a, (object) b);
    }
  }
}
