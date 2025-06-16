// Decompiled with JetBrains decompiler
// Type: Engine.Source.Blueprints.IsPlayer2Node
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using Engine.Common;
using Engine.Common.Services;
using FlowCanvas;
using FlowCanvas.Nodes;
using ParadoxNotion.Design;

#nullable disable
namespace Engine.Source.Blueprints
{
  [Category("Engine")]
  public class IsPlayer2Node : FlowControlNode
  {
    [Port("Target")]
    private ValueInput<IEntity> inputValue;
    [Port("True")]
    private FlowOutput trueOutput;
    [Port("False")]
    private FlowOutput falseOutput;

    [Port("In")]
    private void In()
    {
      IEntity entity = this.inputValue.value;
      if (entity != null && entity == ServiceLocator.GetService<ISimulation>().Player)
        this.trueOutput.Call();
      else
        this.falseOutput.Call();
    }
  }
}
