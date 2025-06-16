// Decompiled with JetBrains decompiler
// Type: RootMotion.Dynamics.SubBehaviourBase
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using System;
using UnityEngine;

#nullable disable
namespace RootMotion.Dynamics
{
  [Serializable]
  public abstract class SubBehaviourBase
  {
    protected BehaviourBase behaviour;

    protected static Vector2 XZ(Vector3 v) => new Vector2(v.x, v.z);

    protected static Vector3 XYZ(Vector2 v) => new Vector3(v.x, 0.0f, v.y);

    protected static Vector3 Flatten(Vector3 v) => new Vector3(v.x, 0.0f, v.z);

    protected static Vector3 SetY(Vector3 v, float y) => new Vector3(v.x, y, v.z);
  }
}
