// Decompiled with JetBrains decompiler
// Type: Engine.Source.Blueprints.Effects.VignetteInputParametersNode
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using Engine.Common.Services;
using Engine.Source.Services;
using Engine.Source.VisualEffects;
using FlowCanvas;
using FlowCanvas.Nodes;
using ParadoxNotion.Design;
using System.Collections.Generic;
using UnityEngine;

#nullable disable
namespace Engine.Source.Blueprints.Effects
{
  [Category("Effects")]
  public class VignetteInputParametersNode : FlowControlNode
  {
    [Port("Name")]
    private ValueInput<string> nameInput;
    [FromLocator]
    private EffectsService effects;
    private List<IParameter<IntensityParameter<Color>>> result = new List<IParameter<IntensityParameter<Color>>>();

    [Port("Value")]
    private IList<IParameter<IntensityParameter<Color>>> Value()
    {
      this.effects.GetParameters<IntensityParameter<Color>>(this.nameInput.value, this.result);
      return (IList<IParameter<IntensityParameter<Color>>>) this.result;
    }
  }
}
