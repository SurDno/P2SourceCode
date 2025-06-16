// Decompiled with JetBrains decompiler
// Type: FlowCanvas.Nodes.PureFunctionNode`1
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

#nullable disable
namespace FlowCanvas.Nodes
{
  public abstract class PureFunctionNode<TResult> : PureFunctionNodeBase
  {
    public abstract TResult Invoke();

    protected override sealed void OnRegisterPorts(FlowNode node)
    {
      node.AddValueOutput<TResult>("Value", (ValueHandler<TResult>) (() => this.Invoke()));
    }
  }
}
