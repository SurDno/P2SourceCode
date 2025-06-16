// Decompiled with JetBrains decompiler
// Type: FlowCanvas.Nodes.NewQuaternion
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using ParadoxNotion.Design;
using UnityEngine;

#nullable disable
namespace FlowCanvas.Nodes
{
  [Category("Utilities/Constructors")]
  public class NewQuaternion : PureFunctionNode<Quaternion, float, float, float, float>
  {
    public override Quaternion Invoke(float x, float y, float z, float w)
    {
      return new Quaternion(x, y, z, w);
    }
  }
}
