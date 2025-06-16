// Decompiled with JetBrains decompiler
// Type: Engine.Source.Blueprints.Effects.VignetteOutputParameterNode
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using Engine.Common;
using Engine.Common.Services;
using Engine.Source.Commons;
using Engine.Source.Services;
using Engine.Source.VisualEffects;
using FlowCanvas;
using FlowCanvas.Nodes;
using ParadoxNotion.Design;
using UnityEngine;

#nullable disable
namespace Engine.Source.Blueprints.Effects
{
  [Category("Effects")]
  public class VignetteOutputParameterNode : 
    FlowControlNode,
    IUpdatable,
    IParameter<IntensityParameter<Color>>,
    IParameter
  {
    [Port("Intensity")]
    private ValueInput<float> intensityValueInput;
    [Port("Color")]
    private ValueInput<Color> colorValueInput;
    [Port("Name")]
    private ValueInput<string> nameInput;
    [FromLocator]
    private EffectsService effects;
    private float prevIntensityValue;
    private Color prevColorValue;
    private bool created;
    private bool destroed;
    private float accamulator;

    IntensityParameter<Color> IParameter<IntensityParameter<Color>>.Value
    {
      get
      {
        return new IntensityParameter<Color>()
        {
          Intensity = this.intensityValueInput.value,
          Value = this.colorValueInput.value
        };
      }
    }

    public override void OnGraphStarted()
    {
      base.OnGraphStarted();
      InstanceByRequest<UpdateService>.Instance.BlueprintEffectsUpdater.AddUpdatable((IUpdatable) this);
    }

    public override void OnGraphStoped()
    {
      InstanceByRequest<UpdateService>.Instance.BlueprintEffectsUpdater.RemoveUpdatable((IUpdatable) this);
      base.OnGraphStoped();
    }

    public void ComputeUpdate()
    {
      float num = this.intensityValueInput.value;
      Color color = this.colorValueInput.value;
      if ((double) this.prevIntensityValue == (double) num && !(this.prevColorValue != color))
        return;
      this.prevIntensityValue = num;
      this.prevColorValue = color;
      if ((double) num != 0.0)
        this.CreateEffect();
      else
        this.DestroyEffect();
    }

    public override void OnDestroy()
    {
      this.destroed = true;
      this.DestroyEffect();
      base.OnDestroy();
    }

    protected void CreateEffect()
    {
      if (this.destroed || this.created)
        return;
      this.created = true;
      this.effects.AddParameter(this.nameInput.value, (IParameter) this);
    }

    protected void DestroyEffect()
    {
      if (!this.created)
        return;
      this.created = false;
      this.effects.RemoveParameter(this.nameInput.value, (IParameter) this);
    }
  }
}
