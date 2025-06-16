// Decompiled with JetBrains decompiler
// Type: FlowCanvas.Nodes.AND
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using ParadoxNotion.Design;

#nullable disable
namespace FlowCanvas.Nodes
{
  [Category("Functions/Math Operators/Boolean")]
  public class AND : PureFunctionNode<bool, bool, bool>
  {
    public override bool Invoke(bool a, bool b) => a & b;
  }
}
