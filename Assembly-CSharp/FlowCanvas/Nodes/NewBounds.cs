// Decompiled with JetBrains decompiler
// Type: FlowCanvas.Nodes.NewBounds
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using ParadoxNotion.Design;
using UnityEngine;

#nullable disable
namespace FlowCanvas.Nodes
{
  [Category("Utilities/Constructors")]
  public class NewBounds : PureFunctionNode<Bounds, Vector3, Vector3>
  {
    public override Bounds Invoke(Vector3 center, Vector3 size) => new Bounds(center, size);
  }
}
