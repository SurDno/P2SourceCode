using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class POISetup : MonoBehaviour
{
  private static HashSet<string> clipNames = new HashSet<string>();
  [EnumFlag(typeof (POIAnimationEnum))]
  [SerializeField]
  [FormerlySerializedAs("SupportedAnimations")]
  private POIAnimationEnum _supportedAnimations;
  [SerializeField]
  [FormerlySerializedAs("S_SitAtDesk_Setup")]
  private POIAnimationSetupSlow _s_SitAtDesk_Setup = new POIAnimationSetupSlow();
  [SerializeField]
  private POIAnimationSetupSlow s_SitAtDesk_Setup = new POIAnimationSetupSlow();
  [SerializeField]
  [FormerlySerializedAs("S_SitOnBench_Setup")]
  private POIAnimationSetupSlow _s_SitOnBench_Setup = new POIAnimationSetupSlow();
  [SerializeField]
  private POIAnimationSetupSlow s_SitOnBench_Setup = new POIAnimationSetupSlow();
  [SerializeField]
  [FormerlySerializedAs("S_LeanOnWall_Setup")]
  private POIAnimationSetupSlow _s_LeanOnWall_Setup = new POIAnimationSetupSlow();
  [SerializeField]
  private POIAnimationSetupSlow s_LeanOnWall_Setup = new POIAnimationSetupSlow();
  [SerializeField]
  [FormerlySerializedAs("S_LeanOnTable_Setup")]
  private POIAnimationSetupSlow _s_LeanOnTable_Setup = new POIAnimationSetupSlow();
  [SerializeField]
  private POIAnimationSetupSlow s_LeanOnTable_Setup = new POIAnimationSetupSlow();
  [SerializeField]
  [FormerlySerializedAs("S_SitNearWall_Setup")]
  private POIAnimationSetupSlow _s_SitNearWall_Setup = new POIAnimationSetupSlow();
  [SerializeField]
  private POIAnimationSetupSlow s_SitNearWall_Setup = new POIAnimationSetupSlow();
  [SerializeField]
  [FormerlySerializedAs("S_LieOnBed_Setup")]
  private POIAnimationSetupSlow _s_LieOnBed_Setup = new POIAnimationSetupSlow();
  [SerializeField]
  private POIAnimationSetupSlow s_LieOnBed_Setup = new POIAnimationSetupSlow();
  [SerializeField]
  [FormerlySerializedAs("S_NearFire_Setup")]
  private POIAnimationSetupSlow _s_NearFire_Setup = new POIAnimationSetupSlow();
  [SerializeField]
  private POIAnimationSetupSlow s_NearFire_Setup = new POIAnimationSetupSlow();
  [SerializeField]
  [FormerlySerializedAs("Q_ViewPoster_Setup")]
  private POIAnimationSetupBase _q_ViewPoster_Setup = new POIAnimationSetupBase();
  [SerializeField]
  private POIAnimationSetupQuick q_ViewPoster_Setup = new POIAnimationSetupQuick();
  [SerializeField]
  [FormerlySerializedAs("Q_LookOutOfTheWindow_Setup")]
  private POIAnimationSetupBase _q_LookOutOfTheWindow_Setup = new POIAnimationSetupBase();
  [SerializeField]
  private POIAnimationSetupQuick q_LookOutOfTheWindow_Setup = new POIAnimationSetupQuick();
  [SerializeField]
  [FormerlySerializedAs("Q_LookUnder_Setup")]
  private POIAnimationSetupBase _q_LookUnder_Setup = new POIAnimationSetupBase();
  [SerializeField]
  private POIAnimationSetupQuick q_LookUnder_Setup = new POIAnimationSetupQuick();
  [SerializeField]
  [FormerlySerializedAs("Q_LookIntoTheWindow_Setup")]
  private POIAnimationSetupBase _q_LookIntoTheWindow_Setup = new POIAnimationSetupBase();
  [SerializeField]
  private POIAnimationSetupQuick q_LookIntoTheWindow_Setup = new POIAnimationSetupQuick();
  [SerializeField]
  [FormerlySerializedAs("Q_ActionWithWall_Setup")]
  private POIAnimationSetupBase _q_ActionWithWall_Setup = new POIAnimationSetupBase();
  [SerializeField]
  private POIAnimationSetupQuick q_ActionWithWall_Setup = new POIAnimationSetupQuick();
  [SerializeField]
  [FormerlySerializedAs("Q_ActionWithTable_Setup")]
  private POIAnimationSetupBase _q_ActionWithTable_Setup = new POIAnimationSetupBase();
  [SerializeField]
  private POIAnimationSetupQuick q_ActionWithTable_Setup = new POIAnimationSetupQuick();
  [SerializeField]
  [FormerlySerializedAs("Q_ActionWithWardrobe_Setup")]
  private POIAnimationSetupBase _q_ActionWithWardrobe_Setup = new POIAnimationSetupBase();
  [SerializeField]
  private POIAnimationSetupQuick q_ActionWithWardrobe_Setup = new POIAnimationSetupQuick();
  [SerializeField]
  [FormerlySerializedAs("Q_ActionWithShelves_Setup")]
  private POIAnimationSetupBase _q_ActionWithShelves_Setup = new POIAnimationSetupBase();
  [SerializeField]
  private POIAnimationSetupQuick q_ActionWithShelves_Setup = new POIAnimationSetupQuick();
  [SerializeField]
  [FormerlySerializedAs("Q_ActionWithNightstand_Setup")]
  private POIAnimationSetupBase _q_ActionWithNightstand_Setup = new POIAnimationSetupBase();
  [SerializeField]
  private POIAnimationSetupQuick q_ActionWithNightstand_Setup = new POIAnimationSetupQuick();
  [SerializeField]
  [FormerlySerializedAs("Q_ActionOnFloor_Setup")]
  private POIAnimationSetupBase _q_ActionOnFloor_Setup = new POIAnimationSetupBase();
  [SerializeField]
  private POIAnimationSetupQuick q_ActionOnFloor_Setup = new POIAnimationSetupQuick();
  [SerializeField]
  private POIAnimationSetupSlow _s_ActionOnFloor_Setup = new POIAnimationSetupSlow();
  [SerializeField]
  private POIAnimationSetupSlow s_ActionOnFloor_Setup = new POIAnimationSetupSlow();
  [SerializeField]
  private POIAnimationSetupBase _q_Idle_Setup = new POIAnimationSetupBase();
  [SerializeField]
  private POIAnimationSetupQuick q_Idle_Setup = new POIAnimationSetupQuick();
  [SerializeField]
  private POIAnimationSetupQuick q_NearFire_Setup = new POIAnimationSetupQuick();
  [SerializeField]
  private POIAnimationSetupSlow s_Dialog_Setup = new POIAnimationSetupSlow();
  [SerializeField]
  private POIAnimationSetupSlow s_Loot_Setup = new POIAnimationSetupSlow();
  [SerializeField]
  private POIAnimationSetupQuick q_PlaygroundPlay_Setup = new POIAnimationSetupQuick();
  [SerializeField]
  private POIAnimationSetupSlow s_PlaygroundSandbox_Setup = new POIAnimationSetupSlow();
  [SerializeField]
  private POIAnimationSetupSlow s_PlaygroundCooperative_Setup = new POIAnimationSetupSlow();
  [SerializeField]
  private POIAnimationSetupQuick q_PlaygroundCooperative_Setup = new POIAnimationSetupQuick();
  [SerializeField]
  private POIAnimationSetupSlow s_SitAtDeskRight_Setup = new POIAnimationSetupSlow();

  public POIAnimationEnum SupportedAnimations
  {
    get => this._supportedAnimations;
    set => this._supportedAnimations = value;
  }

  private void Start()
  {
  }

  public POIAnimationSetupBase GetAnimationSetupOld(POIAnimationEnum animation)
  {
    switch (animation)
    {
      case POIAnimationEnum.S_SitAtDesk:
        return (POIAnimationSetupBase) this._s_SitAtDesk_Setup;
      case POIAnimationEnum.S_SitOnBench:
        return (POIAnimationSetupBase) this._s_SitOnBench_Setup;
      case POIAnimationEnum.S_LeanOnWall:
        return (POIAnimationSetupBase) this._s_LeanOnWall_Setup;
      case POIAnimationEnum.S_LeanOnTable:
        return (POIAnimationSetupBase) this._s_LeanOnTable_Setup;
      case POIAnimationEnum.S_SitNearWall:
        return (POIAnimationSetupBase) this._s_SitNearWall_Setup;
      case POIAnimationEnum.S_LieOnBed:
        return (POIAnimationSetupBase) this._s_LieOnBed_Setup;
      case POIAnimationEnum.S_NearFire:
        return (POIAnimationSetupBase) this._s_NearFire_Setup;
      case POIAnimationEnum.Q_ViewPoster:
        return this._q_ViewPoster_Setup;
      case POIAnimationEnum.Q_LookOutOfTheWindow:
        return this._q_LookOutOfTheWindow_Setup;
      case POIAnimationEnum.Q_LookUnder:
        return this._q_LookUnder_Setup;
      case POIAnimationEnum.Q_LookIntoTheWindow:
        return this._q_LookIntoTheWindow_Setup;
      case POIAnimationEnum.Q_ActionWithWall:
        return this._q_ActionWithWall_Setup;
      case POIAnimationEnum.Q_ActionWithTable:
        return this._q_ActionWithTable_Setup;
      case POIAnimationEnum.Q_ActionWithWardrobe:
        return this._q_ActionWithWardrobe_Setup;
      case POIAnimationEnum.Q_ActionWithShelves:
        return this._q_ActionWithShelves_Setup;
      case POIAnimationEnum.Q_ActionWithNightstand:
        return this._q_ActionWithNightstand_Setup;
      case POIAnimationEnum.Q_ActionOnFloor:
        return this._q_ActionOnFloor_Setup;
      case POIAnimationEnum.S_ActionOnFloor:
        return (POIAnimationSetupBase) this._s_ActionOnFloor_Setup;
      case POIAnimationEnum.Q_Idle:
        return this._q_Idle_Setup;
      case POIAnimationEnum.Q_NearFire:
        return this._q_Idle_Setup;
      case POIAnimationEnum.S_Dialog:
        return this._q_Idle_Setup;
      case POIAnimationEnum.S_Loot:
        return this._q_Idle_Setup;
      case POIAnimationEnum.Q_PlaygroundPlay:
        return (POIAnimationSetupBase) this.q_PlaygroundPlay_Setup;
      case POIAnimationEnum.S_PlaygroundSandbox:
        return (POIAnimationSetupBase) this.s_PlaygroundSandbox_Setup;
      case POIAnimationEnum.S_PlaygroundCooperative:
        return (POIAnimationSetupBase) this.s_PlaygroundCooperative_Setup;
      case POIAnimationEnum.Q_PlaygroundCooperative:
        return (POIAnimationSetupBase) this.q_PlaygroundCooperative_Setup;
      case POIAnimationEnum.S_SitAtDeskRight:
        return (POIAnimationSetupBase) this.s_SitAtDeskRight_Setup;
      default:
        return (POIAnimationSetupBase) null;
    }
  }

  public POIAnimationSetupBase GetAnimationSetup(POIAnimationEnum animation)
  {
    switch (animation)
    {
      case POIAnimationEnum.S_SitAtDesk:
        return (POIAnimationSetupBase) this.s_SitAtDesk_Setup;
      case POIAnimationEnum.S_SitOnBench:
        return (POIAnimationSetupBase) this.s_SitOnBench_Setup;
      case POIAnimationEnum.S_LeanOnWall:
        return (POIAnimationSetupBase) this.s_LeanOnWall_Setup;
      case POIAnimationEnum.S_LeanOnTable:
        return (POIAnimationSetupBase) this.s_LeanOnTable_Setup;
      case POIAnimationEnum.S_SitNearWall:
        return (POIAnimationSetupBase) this.s_SitNearWall_Setup;
      case POIAnimationEnum.S_LieOnBed:
        return (POIAnimationSetupBase) this.s_LieOnBed_Setup;
      case POIAnimationEnum.S_NearFire:
        return (POIAnimationSetupBase) this.s_NearFire_Setup;
      case POIAnimationEnum.Q_ViewPoster:
        return (POIAnimationSetupBase) this.q_ViewPoster_Setup;
      case POIAnimationEnum.Q_LookOutOfTheWindow:
        return (POIAnimationSetupBase) this.q_LookOutOfTheWindow_Setup;
      case POIAnimationEnum.Q_LookUnder:
        return (POIAnimationSetupBase) this.q_LookUnder_Setup;
      case POIAnimationEnum.Q_LookIntoTheWindow:
        return (POIAnimationSetupBase) this.q_LookIntoTheWindow_Setup;
      case POIAnimationEnum.Q_ActionWithWall:
        return (POIAnimationSetupBase) this.q_ActionWithWall_Setup;
      case POIAnimationEnum.Q_ActionWithTable:
        return (POIAnimationSetupBase) this.q_ActionWithTable_Setup;
      case POIAnimationEnum.Q_ActionWithWardrobe:
        return (POIAnimationSetupBase) this.q_ActionWithWardrobe_Setup;
      case POIAnimationEnum.Q_ActionWithShelves:
        return (POIAnimationSetupBase) this.q_ActionWithShelves_Setup;
      case POIAnimationEnum.Q_ActionWithNightstand:
        return (POIAnimationSetupBase) this.q_ActionWithNightstand_Setup;
      case POIAnimationEnum.Q_ActionOnFloor:
        return (POIAnimationSetupBase) this.q_ActionOnFloor_Setup;
      case POIAnimationEnum.S_ActionOnFloor:
        return (POIAnimationSetupBase) this.s_ActionOnFloor_Setup;
      case POIAnimationEnum.Q_Idle:
        return (POIAnimationSetupBase) this.q_Idle_Setup;
      case POIAnimationEnum.Q_NearFire:
        return (POIAnimationSetupBase) this.q_NearFire_Setup;
      case POIAnimationEnum.S_Dialog:
        return (POIAnimationSetupBase) this.s_Dialog_Setup;
      case POIAnimationEnum.S_Loot:
        return (POIAnimationSetupBase) this.s_Loot_Setup;
      case POIAnimationEnum.Q_PlaygroundPlay:
        return (POIAnimationSetupBase) this.q_PlaygroundPlay_Setup;
      case POIAnimationEnum.S_PlaygroundSandbox:
        return (POIAnimationSetupBase) this.s_PlaygroundSandbox_Setup;
      case POIAnimationEnum.S_PlaygroundCooperative:
        return (POIAnimationSetupBase) this.s_PlaygroundCooperative_Setup;
      case POIAnimationEnum.Q_PlaygroundCooperative:
        return (POIAnimationSetupBase) this.q_PlaygroundCooperative_Setup;
      case POIAnimationEnum.S_SitAtDeskRight:
        return (POIAnimationSetupBase) this.s_SitAtDeskRight_Setup;
      default:
        return (POIAnimationSetupBase) null;
    }
  }

  public Vector3 GetAnimationOffset(POIAnimationEnum animation, int animationIndex)
  {
    POIAnimationSetupBase animationSetup = this.GetAnimationSetup(animation);
    return animationSetup != null && animationSetup.Elements.Count > animationIndex && animationSetup.Elements[animationIndex] != null ? animationSetup.Elements[animationIndex].Offset : Vector3.zero;
  }

  public bool GetAnimationPlayOnce(POIAnimationEnum animation, int animationIndex)
  {
    POIAnimationSetupBase animationSetup = this.GetAnimationSetup(animation);
    if (animationSetup == null || animationSetup.Elements.Count <= animationIndex || animationSetup.Elements[animationIndex] == null)
      return false;
    return animationSetup is POIAnimationSetupQuick || animationSetup.Elements[animationIndex].PlayOnce;
  }

  public bool AnimationIsQuick(POIAnimationEnum animation)
  {
    return this.GetAnimationSetup(animation) is POIAnimationSetupQuick;
  }

  public bool GetNeedSynchronizeAnimation(POIAnimationEnum animation, int animationIndex)
  {
    POIAnimationSetupBase animationSetup = this.GetAnimationSetup(animation);
    if (animationSetup == null || animationSetup.Elements.Count <= animationIndex || animationSetup.Elements[animationIndex] == null)
      return false;
    return animationSetup.Elements[animationIndex].Offset != Vector3.zero || animationSetup.Elements[animationIndex].NeedSynchroniseAnimation;
  }

  public float GetAnimationSyncDelay(POIAnimationEnum animation, int animationIndex)
  {
    POIAnimationSetupBase animationSetup = this.GetAnimationSetup(animation);
    return animationSetup != null && animationSetup.Elements.Count > animationIndex && animationSetup.Elements[animationIndex] != null ? animationSetup.Elements[animationIndex].AnimationSyncDelay : 0.0f;
  }

  private void SetNewClip(AnimationClipOverrides clips, string name, AnimationClip newClip)
  {
    clips[name] = newClip;
  }
}
