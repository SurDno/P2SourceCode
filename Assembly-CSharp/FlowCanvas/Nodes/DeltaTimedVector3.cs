// Decompiled with JetBrains decompiler
// Type: FlowCanvas.Nodes.DeltaTimedVector3
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using ParadoxNotion.Design;
using UnityEngine;

#nullable disable
namespace FlowCanvas.Nodes
{
  [Category("Functions/Utility")]
  public class DeltaTimedVector3 : PureFunctionNode<Vector3, Vector3, float>
  {
    public override Vector3 Invoke(Vector3 value, float multiplier)
    {
      return value * multiplier * Time.deltaTime;
    }
  }
}
