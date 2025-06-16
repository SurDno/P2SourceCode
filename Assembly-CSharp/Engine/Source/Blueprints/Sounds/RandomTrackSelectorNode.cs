// Decompiled with JetBrains decompiler
// Type: Engine.Source.Blueprints.Sounds.RandomTrackSelectorNode
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using FlowCanvas;
using FlowCanvas.Nodes;
using ParadoxNotion.Design;
using System.Collections.Generic;
using UnityEngine;

#nullable disable
namespace Engine.Source.Blueprints.Sounds
{
  [Category("Sounds")]
  [ContextDefinedInputs(new System.Type[] {typeof (AudioClip)})]
  public class RandomTrackSelectorNode : FlowControlNode, IMultiPortNode
  {
    [SerializeField]
    private int _portCount = 2;
    private List<ValueInput<AudioClip>> inputs = new List<ValueInput<AudioClip>>();

    public int portCount
    {
      get => this._portCount;
      set => this._portCount = value;
    }

    protected override void RegisterPorts()
    {
      base.RegisterPorts();
      this.inputs.Clear();
      for (int index = 0; index < this._portCount; ++index)
        this.inputs.Add(this.AddValueInput<AudioClip>((index + 1).ToString()));
    }

    [Port("Value")]
    private AudioClip Value() => this.inputs[UnityEngine.Random.Range(0, this.inputs.Count)].value;
  }
}
