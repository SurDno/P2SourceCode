using System;
using Inspectors;
using UnityEngine;

[Serializable]
public class IdlePresetObject : MonoBehaviour
{
  [SerializeField]
  [HideInInspector]
  private IdlePresetEnum presetAnimation;
  public bool MakeObstacle;
  public bool RandomAnimationIndex = true;
  public int AnimationIndex;

  [Inspected(Mutable = true, Mode = ExecuteMode.EditAndRuntime)]
  public IdlePresetEnum PresetAnimation
  {
    get => presetAnimation;
    set => presetAnimation = value;
  }

  public POIAnimationEnum GetPOIAnimationEnum()
  {
    switch (presetAnimation)
    {
      case IdlePresetEnum.SitAtDesk:
        return POIAnimationEnum.S_SitAtDesk;
      case IdlePresetEnum.SitOnBench:
        return POIAnimationEnum.S_SitOnBench;
      case IdlePresetEnum.LeanOnWall:
        return POIAnimationEnum.S_LeanOnWall;
      case IdlePresetEnum.LeanOnTable:
        return POIAnimationEnum.S_LeanOnTable;
      case IdlePresetEnum.SitNearWall:
        return POIAnimationEnum.S_SitNearWall;
      case IdlePresetEnum.LieOnBed:
        return POIAnimationEnum.S_LieOnBed;
      case IdlePresetEnum.NearFire:
        return POIAnimationEnum.S_NearFire;
      case IdlePresetEnum.ActionOnFloor:
        return POIAnimationEnum.S_ActionOnFloor;
      case IdlePresetEnum.SitAtDeskRight:
        return POIAnimationEnum.S_SitAtDeskRight;
      case IdlePresetEnum.Dialog:
        return POIAnimationEnum.S_Dialog;
      case IdlePresetEnum.GroundSitting:
        return POIAnimationEnum.S_PlaygroundSandbox;
      default:
        return POIAnimationEnum.Unknown;
    }
  }
}
