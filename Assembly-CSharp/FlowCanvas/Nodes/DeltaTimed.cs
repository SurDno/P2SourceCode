// Decompiled with JetBrains decompiler
// Type: FlowCanvas.Nodes.DeltaTimed
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using ParadoxNotion.Design;
using UnityEngine;

#nullable disable
namespace FlowCanvas.Nodes
{
  [Name("Delta Timed Float")]
  [Category("Functions/Utility")]
  public class DeltaTimed : PureFunctionNode<float, float, float>
  {
    public override float Invoke(float value, float multiplier)
    {
      return value * multiplier * Time.deltaTime;
    }
  }
}
