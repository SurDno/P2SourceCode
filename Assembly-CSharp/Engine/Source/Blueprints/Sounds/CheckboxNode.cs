// Decompiled with JetBrains decompiler
// Type: Engine.Source.Blueprints.Sounds.CheckboxNode
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using FlowCanvas;
using FlowCanvas.Nodes;
using ParadoxNotion.Design;

#nullable disable
namespace Engine.Source.Blueprints.Sounds
{
  [Category("Sounds")]
  public class CheckboxNode : FlowControlNode
  {
    [Port("Value")]
    private ValueInput<bool> valueInput;

    [Port("Value")]
    private float Value() => this.valueInput.value ? 1f : 0.0f;
  }
}
