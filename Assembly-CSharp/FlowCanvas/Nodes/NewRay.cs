// Decompiled with JetBrains decompiler
// Type: FlowCanvas.Nodes.NewRay
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using ParadoxNotion.Design;
using UnityEngine;

#nullable disable
namespace FlowCanvas.Nodes
{
  [Category("Utilities/Constructors")]
  public class NewRay : PureFunctionNode<Ray, Vector3, Vector3>
  {
    public override Ray Invoke(Vector3 origin, Vector3 direction) => new Ray(origin, direction);
  }
}
