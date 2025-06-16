// Decompiled with JetBrains decompiler
// Type: Engine.Source.Blueprints.FastTravelTestNode
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using Engine.Common;
using Engine.Common.Components.Regions;
using Engine.Source.Components;
using FlowCanvas;
using FlowCanvas.Nodes;
using ParadoxNotion.Design;
using PLVirtualMachine.Common.EngineAPI;
using System;

#nullable disable
namespace Engine.Source.Blueprints
{
  [Category("Engine")]
  public class FastTravelTestNode : FlowControlNode
  {
    [Port("Entity")]
    private ValueInput<IEntity> entity;
    [Port("Out")]
    private FlowOutput output;

    [Port("In")]
    private void In()
    {
      GameTime travelTime = new GameTime((ushort) 1, (byte) 0, (byte) 0, (byte) 0);
      if (this.entity != null)
        this.entity.value.GetComponent<FastTravelComponent>()?.FireTravelToPoint(FastTravelPointEnum.Zavodi, (TimeSpan) travelTime);
      this.output.Call();
    }
  }
}
