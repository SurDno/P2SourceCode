// Decompiled with JetBrains decompiler
// Type: FlowCanvas.Nodes.SendEvent
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using ParadoxNotion;
using ParadoxNotion.Design;

#nullable disable
namespace FlowCanvas.Nodes
{
  [Category("Functions/Utility")]
  public class SendEvent : CallableActionNode<FlowScriptController, string>
  {
    public override void Invoke(FlowScriptController target, string eventName)
    {
      target.SendEvent(new EventData(eventName));
    }
  }
}
