// Decompiled with JetBrains decompiler
// Type: Engine.Source.Blueprints.Effects.ShakeCameraNode
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using FlowCanvas;
using FlowCanvas.Nodes;
using NodeCanvas.Framework;
using System.ComponentModel;
using UnityEngine;

#nullable disable
namespace Engine.Source.Blueprints.Effects
{
  [Category("Effects")]
  public class ShakeCameraNode : FlowControlNode, IUpdatable
  {
    [Port("Value")]
    private ValueInput<float> valueInput;

    public void Update()
    {
      GameCamera.Instance.Camera.transform.localPosition = Vector3.one * UnityEngine.Random.value * this.valueInput.value;
    }
  }
}
