using System;
using UnityEngine;

namespace Engine.Assets.Objects
{
  [Serializable]
  public class StepsAction
  {
    public StepsEvent[] Events;
    public float MaxThesholdIntensity;
    public float MinThesholdIntensity;
    public PhysicMaterial Material;
  }
}
