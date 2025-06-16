// Decompiled with JetBrains decompiler
// Type: Engine.Source.UI.Menu.Protagonist.LockPicking.LockPickingSettings
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using System;
using UnityEngine;

#nullable disable
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
    public LockPickingSettings.Pattern[] Patterns;

    [Serializable]
    public struct Pattern
    {
      public int SpotCount;
      public int SweetSpotPosition;
    }
  }
}
