using System;
using UnityEngine;

namespace Engine.Source.UI.Menu.Protagonist.LockPicking
{
  [Serializable]
  public class LockPickingSettings
  {
    [Header("Gameplay")]
    public float GravityForce = 2f;
    public float MouseForce = 1f;
    public float MaxVelocity = 3f;
    public float SourSpotDrag = 6.5f;
    public float DurabilityCost = 0.025f;
    [Header("Layout")]
    public float LowerDeadZone = 0.25f;
    public float UpperDeadZone = 0.07f;
    public float MiddleDeadZones = 0.07f;
    public float SweetSpotSize = 0.12f;
    public float SourSpotMinSize = 0.05f;
    public float SourSpotMaxSize = 0.1f;
    public Pattern[] Patterns;

    [Serializable]
    public struct Pattern
    {
      public int SpotCount;
      public int SweetSpotPosition;
    }
  }
}
