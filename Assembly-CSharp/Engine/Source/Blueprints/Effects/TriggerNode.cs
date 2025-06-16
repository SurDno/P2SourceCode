// Decompiled with JetBrains decompiler
// Type: Engine.Source.Blueprints.Effects.TriggerNode
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using FlowCanvas.Nodes;
using ParadoxNotion.Design;

#nullable disable
namespace Engine.Source.Blueprints.Effects
{
  [Category("Effects")]
  public class TriggerNode : FlowControlNode
  {
    private bool value;

    [Port("Set")]
    public void Set() => this.value = true;

    [Port("Reset")]
    public void Reset() => this.value = false;

    [Port("Value")]
    private bool Value() => this.value;
  }
}
