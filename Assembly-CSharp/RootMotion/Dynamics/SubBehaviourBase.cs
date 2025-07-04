﻿using System;
using UnityEngine;

namespace RootMotion.Dynamics
{
  [Serializable]
  public abstract class SubBehaviourBase
  {
    protected BehaviourBase behaviour;

    protected static Vector2 XZ(Vector3 v) => new(v.x, v.z);

    protected static Vector3 XYZ(Vector2 v) => new(v.x, 0.0f, v.y);

    protected static Vector3 Flatten(Vector3 v) => new(v.x, 0.0f, v.z);

    protected static Vector3 SetY(Vector3 v, float y) => new(v.x, y, v.z);
  }
}
