// Decompiled with JetBrains decompiler
// Type: Engine.Source.Blueprints.Effects.FocusModeShaderEffectNode
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using FlowCanvas;
using FlowCanvas.Nodes;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using UnityEngine;

#nullable disable
namespace Engine.Source.Blueprints.Effects
{
  [Category("Effects")]
  public class FocusModeShaderEffectNode : FlowControlNode, IUpdatable
  {
    [Port("Intensity")]
    private ValueInput<float> intensityInput;
    [Port("Color")]
    private ValueInput<Color> colorInput;
    private bool initialized = false;
    private int propertyId;

    public void Update()
    {
      if (!this.initialized)
      {
        this.propertyId = Shader.PropertyToID("_FocusEffectColor");
        this.initialized = true;
      }
      Shader.SetGlobalColor(this.propertyId, this.colorInput.value * this.intensityInput.value);
    }

    public override void OnDestroy()
    {
      base.OnDestroy();
      if (!this.initialized)
        return;
      Shader.SetGlobalColor(this.propertyId, Color.black);
    }
  }
}
