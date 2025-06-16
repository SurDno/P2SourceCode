// Decompiled with JetBrains decompiler
// Type: FlowCanvas.Nodes.ExtractQuaternion
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
namespace FlowCanvas.Nodes
{
  public class ExtractQuaternion : ExtractorNode<Quaternion, float, float, float, float, Vector3>
  {
    public override void Invoke(
      Quaternion quaternion,
      out float x,
      out float y,
      out float z,
      out float w,
      out Vector3 eulerAngles)
    {
      x = quaternion.x;
      y = quaternion.y;
      z = quaternion.z;
      w = quaternion.w;
      eulerAngles = quaternion.eulerAngles;
    }
  }
}
