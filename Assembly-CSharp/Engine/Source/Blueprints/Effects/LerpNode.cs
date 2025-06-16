// Decompiled with JetBrains decompiler
// Type: Engine.Source.Blueprints.Effects.LerpNode
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using FlowCanvas;
using FlowCanvas.Nodes;
using ParadoxNotion.Design;
using UnityEngine;

#nullable disable
namespace Engine.Source.Blueprints.Effects
{
  [Category("Effects")]
  public class LerpNode : FlowControlNode
  {
    [Port("A")]
    private ValueInput<float> aInput;
    [Port("B")]
    private ValueInput<float> bInput;
    [Port("T")]
    private ValueInput<float> tInput;

    [Port("Value")]
    private float Value() => Mathf.Lerp(this.aInput.value, this.bInput.value, this.tInput.value);
  }
}
