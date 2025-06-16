// Decompiled with JetBrains decompiler
// Type: FlowCanvas.Nodes.Vector3NotEqual
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using ParadoxNotion.Design;
using UnityEngine;

#nullable disable
namespace FlowCanvas.Nodes
{
  [Category("Functions/Math Operators/Vector3")]
  [Name("!=")]
  public class Vector3NotEqual : PureFunctionNode<bool, Vector3, Vector3>
  {
    public override bool Invoke(Vector3 a, Vector3 b) => a != b;
  }
}
