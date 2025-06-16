// Decompiled with JetBrains decompiler
// Type: FlowCanvas.Nodes.EventNode
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using ParadoxNotion.Design;

#nullable disable
namespace FlowCanvas.Nodes
{
  [Category("Events")]
  [Color("5cdd5c")]
  public abstract class EventNode : FlowNode
  {
    public override string name => string.Format("{0}", (object) base.name.ToUpper());
  }
}
