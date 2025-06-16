// Decompiled with JetBrains decompiler
// Type: Engine.Source.Blueprints.Effects.FloatCeilToIntNode
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using FlowCanvas.Nodes;
using ParadoxNotion.Design;
using UnityEngine;

#nullable disable
namespace Engine.Source.Blueprints.Effects
{
  [Category("Engine")]
  public class FloatCeilToIntNode : PureFunctionNode<int, float>
  {
    public override int Invoke(float a) => Mathf.CeilToInt(a);
  }
}
