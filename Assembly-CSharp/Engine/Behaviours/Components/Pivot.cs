using Engine.Behaviours.Engines.Controllers;
using Engine.Behaviours.Unity.Mecanim;
using Engine.Common;
using Engine.Common.Commons;
using Engine.Common.Components;
using Engine.Source.Audio;
using Engine.Source.Commons;
using Engine.Source.Components;
using Inspectors;
using RootMotion.Dynamics;
using System;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Audio;
using UnityEngine.Serialization;

namespace Engine.Behaviours.Components
{
  [DisallowMultipleComponent]
  public class Pivot : MonoBehaviour, IEntityAttachable
  {
    public Pivot.HierarhyStructureEnum HierarhyStructure = Pivot.HierarhyStructureEnum.Ordinary;
    public GameObject AnchorCameraFPS;
    [Tooltip("Camera прикрпленная к голове с правильно расположенными осями (z-вперед, y-вверх")]
    public GameObject AnchorCameraOnHead;
    public DialogLightingProfile DialogLightingProfile;
    public GameObject DialogDOFTarget;
    public Transform RootBone;
    [Header("Body parts")]
    public GameObject Head;
    public GameObject Chest;
    public GameObject Belly;
    public GameObject Foot;
    public GameObject Hands;
    [Header("Weapon Aim")]
    public Pivot.AimWeaponType AimType = Pivot.AimWeaponType.Head;
    [Tooltip("Этим объектом происходит прицеливание")]
    public GameObject AimTransform;
    [Tooltip("из этого объекта расчитывается направление выстрела")]
    public GameObject ShootStart;
    [Tooltip("Aim Axis")]
    public Vector3 AimAxis = Vector3.forward;
    [Tooltip("Pole Axis")]
    public Vector3 PoleAxis = Vector3.up;
    [Header("Weapon transform")]
    [Tooltip("Эта трансформация копируется в WeaponTransform")]
    public GameObject WeaponDummyTransform;
    [Tooltip("Оружие")]
    public GameObject WeaponTransform;
    [Header("Ragdoll")]
    [SerializeField]
    [FormerlySerializedAs("Ragdoll_Pelvis")]
    private GameObject ragdollPelvis;
    [SerializeField]
    [FormerlySerializedAs("Ragdoll_LeftHips")]
    private GameObject ragdollLeftHips;
    [SerializeField]
    [FormerlySerializedAs("Ragdoll_LeftKnee")]
    private GameObject ragdollLeftKnee;
    [SerializeField]
    [FormerlySerializedAs("Ragdoll_RightHips")]
    private GameObject ragdollRightHips;
    [SerializeField]
    [FormerlySerializedAs("Ragdoll_RightKnee")]
    private GameObject ragdollRightKnee;
    [SerializeField]
    [FormerlySerializedAs("Ragdoll_LeftArm")]
    private GameObject ragdollLeftArm;
    [SerializeField]
    [FormerlySerializedAs("Ragdoll_LeftElbow")]
    private GameObject ragdollLeftElbow;
    [SerializeField]
    [FormerlySerializedAs("Ragdoll_RightArm")]
    private GameObject ragdollRightArm;
    [SerializeField]
    [FormerlySerializedAs("Ragdoll_RightElbow")]
    private GameObject ragdollRightElbow;
    [SerializeField]
    [FormerlySerializedAs("Ragdoll_MiddleSpine")]
    private GameObject ragdollMiddleSpine;
    [SerializeField]
    [FormerlySerializedAs("Ragdoll_Head")]
    private GameObject ragdollHead;
    [SerializeField]
    [FormerlySerializedAs("Ragdoll_Weapon")]
    private GameObject ragdollWeapon;
    [Header("Sound")]
    [SerializeField]
    private NPCSoundBankDescription soundBank;
    [Header("You can use ComputeNpc to fill this!")]
    [SerializeField]
    private Animator animator;
    [SerializeField]
    private AnimatorEventProxy animatorEventProxy;
    [SerializeField]
    private NavMeshAgent agent;
    [SerializeField]
    private PuppetMaster puppet;
    [SerializeField]
    private NavMeshObstacle obstacle;
    [SerializeField]
    private Rigidbody rigidbody;
    [SerializeField]
    private NPCEnemy npcEnemy;
    [SerializeField]
    private NPCWeaponService npcWeaponService;
    [SerializeField]
    private EngineBehavior behavior;
    [SerializeField]
    public int SecondaryIdleAnimationCount;
    [SerializeField]
    public int SecondaryLowIdleAnimationCount;
    [SerializeField]
    public int DialogIdleAnimationCount;
    [SerializeField]
    public CombatStyleEnum DefaultCombatStyle;
    [SerializeField]
    public GameObject HidingOuterWeapon;
    private Vector3 headImpulseValue;
    private float ragdollWeight;
    private float actualRagdollWeight;
    private bool ragdollInternalCollisions = true;
    [EnumFlag(typeof (POIAnimationEnum))]
    [SerializeField]
    private POIAnimationEnum supportedFastPOIExits;
    [SerializeField]
    private bool supportsFastPOIExits;
    [SerializeField]
    public bool CollidersSetForSharpshooting;

    [Inspected]
    public IEntity Owner { get; private set; }

    [Inspected]
    public bool Indoor { get; private set; }

    public event Action<bool> IndoorChangedEvent;

    public Animator GetAnimator() => this.animator;

    public AnimatorEventProxy GetAnimatorEventProxy() => this.animatorEventProxy;

    public PuppetMaster GetPuppet() => this.puppet;

    public NavMeshObstacle GetObstacle() => this.obstacle;

    public NavMeshAgent GetAgent() => this.agent;

    public Rigidbody GetRigidbody() => this.rigidbody;

    public NPCEnemy GetNpcEnemy() => this.npcEnemy;

    public NPCWeaponService GetNpcWeaponService() => this.npcWeaponService;

    public EngineBehavior GetBehavior() => this.behavior;

    public NPCSoundBankDescription SoundBank => this.soundBank;

    private void Awake()
    {
      if ((UnityEngine.Object) this.animator == (UnityEngine.Object) null)
        Debug.LogWarning((object) (this.gameObject.name + " doesn't contain " + typeof (Animator).Name + " unity component."));
      if (!((UnityEngine.Object) this.agent == (UnityEngine.Object) null))
        return;
      Debug.LogWarning((object) (this.gameObject.name + " doesn't contain " + typeof (NavMeshAgent).Name + " unity component."));
    }

    private void Start()
    {
      this.RagdollWeight = 0.0f;
      this.ActualRagdollWeight = 0.0f;
    }

    public void ResetMovable() => AnimatorState45.GetAnimatorState(this.animator).ResetMovable();

    private bool PlayRandomSound(
      AudioMixerGroup mixer,
      AudioClip[] sounds,
      Transform transform,
      float volume,
      float spatialBlend)
    {
      if (sounds == null || sounds.Length == 0 || (UnityEngine.Object) transform == (UnityEngine.Object) null)
        return false;
      AudioClip sound = sounds[UnityEngine.Random.Range(0, sounds.Length)];
      if ((double) spatialBlend > 0.5)
        SoundUtility.PlayAudioClip3D(this.Chest.transform, sound, mixer, volume, 1f, 2.5f, true, 0.0f, context: this.gameObject.GetFullName());
      else
        SoundUtility.PlayAudioClip2D(sound, mixer, volume, 0.0f, context: this.gameObject.GetFullName());
      return true;
    }

    public void PlaySound(Pivot.SoundEnum soundEnum, float volumeScale = 1f, bool protagonist = false)
    {
      if ((UnityEngine.Object) this.soundBank == (UnityEngine.Object) null)
        return;
      AudioMixerGroup mixer;
      float spatialBlend;
      if (protagonist)
      {
        mixer = ScriptableObjectInstance<GameSettingsData>.Instance.ProtagonistHitsMixer;
        spatialBlend = 0.0f;
      }
      else
      {
        mixer = this.Indoor ? ScriptableObjectInstance<GameSettingsData>.Instance.NpcClothesIndoorMixer : ScriptableObjectInstance<GameSettingsData>.Instance.NpcClothesOutdoorMixer;
        spatialBlend = 1f;
      }
      switch (soundEnum)
      {
        case Pivot.SoundEnum.RagdollFall:
          this.PlayRandomSound(mixer, this.soundBank.RagdollFallSounds, this.Chest.transform, volumeScale, spatialBlend);
          break;
        case Pivot.SoundEnum.HittedVocal:
          this.PlayRandomSound(mixer, this.soundBank.HittedVocalSounds, this.Head.transform, volumeScale, spatialBlend);
          break;
        case Pivot.SoundEnum.HittedLowStaminaVocal:
          this.PlayRandomSound(mixer, this.soundBank.HittedLowStaminaVocalSounds, this.Head.transform, volumeScale, spatialBlend);
          break;
        case Pivot.SoundEnum.HittedDodgeVocal:
          this.PlayRandomSound(mixer, this.soundBank.HittedDodgeVocalSounds, this.Head.transform, volumeScale, spatialBlend);
          break;
        case Pivot.SoundEnum.StaggerVocal:
          this.PlayRandomSound(mixer, this.soundBank.StaggerVocalSounds, this.Head.transform, volumeScale, spatialBlend);
          break;
        case Pivot.SoundEnum.StaggerNonVocal:
          this.PlayRandomSound(mixer, this.soundBank.StaggerNonVocalSounds, this.Head.transform, volumeScale, spatialBlend);
          break;
        case Pivot.SoundEnum.StrafeLeft:
          this.PlayRandomSound(mixer, this.soundBank.StrafeLeftSounds, this.Chest.transform, volumeScale, spatialBlend);
          break;
        case Pivot.SoundEnum.StrafeRight:
          this.PlayRandomSound(mixer, this.soundBank.StrafeRightSounds, this.Chest.transform, volumeScale, spatialBlend);
          break;
        case Pivot.SoundEnum.StepBack:
          this.PlayRandomSound(mixer, this.soundBank.StepBackSounds, this.Chest.transform, volumeScale, spatialBlend);
          break;
        case Pivot.SoundEnum.BlockHitted:
          this.PlayRandomSound(mixer, this.soundBank.BlockHittedSounds, this.Chest.transform, volumeScale, spatialBlend);
          break;
        case Pivot.SoundEnum.FaceHitted:
          this.PlayRandomSound(mixer, this.soundBank.FaceHittedSounds, this.Head.transform, volumeScale, spatialBlend);
          break;
        case Pivot.SoundEnum.Stab:
          this.PlayRandomSound(mixer, this.soundBank.StabSounds, this.Chest.transform, volumeScale, spatialBlend);
          break;
        case Pivot.SoundEnum.StabBlock:
          this.PlayRandomSound(mixer, this.soundBank.StabBlockSounds, this.Chest.transform, volumeScale, spatialBlend);
          break;
        case Pivot.SoundEnum.BulletHit:
          this.PlayRandomSound(mixer, this.soundBank.BulletHitSounds, this.Chest.transform, volumeScale, spatialBlend);
          break;
      }
    }

    public Transform GetAimTransform(Pivot.AimWeaponType aimType)
    {
      switch (aimType)
      {
        case Pivot.AimWeaponType.Head:
          return (UnityEngine.Object) this.Head != (UnityEngine.Object) null ? this.Head.transform : (Transform) null;
        case Pivot.AimWeaponType.Chest:
          return (UnityEngine.Object) this.Chest != (UnityEngine.Object) null ? this.Chest.transform : (Transform) null;
        case Pivot.AimWeaponType.Belly:
          return (UnityEngine.Object) this.Belly != (UnityEngine.Object) null ? this.Belly.transform : (Transform) null;
        case Pivot.AimWeaponType.Foot:
          return (UnityEngine.Object) this.Foot == (UnityEngine.Object) null ? this.transform : this.Foot.transform;
        default:
          throw new NotImplementedException();
      }
    }

    public void RagdollApplyImpulseToHead(Vector3 impulseWorldSpace)
    {
      this.headImpulseValue = impulseWorldSpace;
    }

    public float RagdollWeight
    {
      get => this.ragdollWeight;
      set => this.ragdollWeight = value;
    }

    public float ActualRagdollWeight
    {
      get => this.actualRagdollWeight;
      set
      {
        if ((double) this.actualRagdollWeight < 0.949999988079071 && (double) value >= 0.949999988079071)
          this.PlaySound(Pivot.SoundEnum.RagdollFall);
        this.actualRagdollWeight = value;
        IKController component1 = this.GetComponent<IKController>();
        if ((UnityEngine.Object) component1 != (UnityEngine.Object) null)
          component1.enabled = (double) value == 0.0;
        if (this.HierarhyStructure == Pivot.HierarhyStructureEnum.Ordinary)
        {
          bool enable = (double) value > 0.0;
          this.EnableRagdool(this.ragdollPelvis, enable);
          this.EnableRagdool(this.ragdollLeftHips, enable);
          this.EnableRagdool(this.ragdollRightHips, enable);
          this.EnableRagdool(this.ragdollLeftKnee, enable);
          this.EnableRagdool(this.ragdollRightKnee, enable);
          this.EnableRagdool(this.ragdollLeftArm, enable);
          this.EnableRagdool(this.ragdollRightArm, enable);
          this.EnableRagdool(this.ragdollLeftElbow, enable);
          this.EnableRagdool(this.ragdollRightElbow, enable);
          this.EnableRagdool(this.ragdollMiddleSpine, enable);
          this.EnableRagdool(this.ragdollHead, enable);
          if ((UnityEngine.Object) this.ragdollHead != (UnityEngine.Object) null && enable && (double) this.headImpulseValue.sqrMagnitude > 0.10000000149011612)
          {
            Rigidbody component2 = this.ragdollHead.GetComponent<Rigidbody>();
            if ((UnityEngine.Object) component2 != (UnityEngine.Object) null)
            {
              component2.AddForceAtPosition(this.headImpulseValue, this.ragdollHead.transform.position, ForceMode.Impulse);
              this.headImpulseValue = Vector3.zero;
            }
          }
          this.EnableRagdool(this.ragdollWeapon, enable);
        }
        else
        {
          if (this.HierarhyStructure != Pivot.HierarhyStructureEnum.PuppetMaster || (UnityEngine.Object) this.puppet == (UnityEngine.Object) null)
            return;
          float a = 0.0f;
          if ((double) value < 0.029999999329447746)
          {
            this.puppet.pinWeight = 1f;
            this.puppet.muscleWeight = 1f;
            this.puppet.mode = PuppetMaster.Mode.Kinematic;
            this.puppet.state = PuppetMaster.State.Alive;
            if (this.puppet.gameObject.activeSelf)
              this.puppet.gameObject.SetActive(false);
          }
          else if ((double) value > 0.949999988079071)
          {
            if (!this.puppet.gameObject.activeSelf)
              this.puppet.gameObject.SetActive(true);
            this.puppet.pinWeight = 0.0f;
            this.puppet.muscleWeight = a;
            this.puppet.mode = PuppetMaster.Mode.Active;
            this.puppet.state = PuppetMaster.State.Dead;
          }
          else
          {
            if (!this.puppet.gameObject.activeSelf)
              this.puppet.gameObject.SetActive(true);
            this.puppet.pinWeight = 1f - value;
            this.puppet.muscleWeight = Mathf.Max(a, 1f - value);
            this.puppet.mode = PuppetMaster.Mode.Active;
            this.puppet.state = PuppetMaster.State.Alive;
          }
        }
      }
    }

    public bool RgdollInternalCollisions
    {
      get => this.ragdollInternalCollisions;
      set
      {
        this.ragdollInternalCollisions = value;
        if (this.HierarhyStructure != Pivot.HierarhyStructureEnum.PuppetMaster || !((UnityEngine.Object) this.puppet != (UnityEngine.Object) null))
          return;
        this.puppet.internalCollisions = this.ragdollInternalCollisions;
      }
    }

    private void EnableRagdool(GameObject go, bool enable)
    {
      if (!((UnityEngine.Object) go != (UnityEngine.Object) null))
        return;
      go.layer = enable ? ScriptableObjectInstance<GameSettingsData>.Instance.RagdollLayer.GetIndex() : ScriptableObjectInstance<GameSettingsData>.Instance.DynamicLayer.GetIndex();
      Rigidbody component1 = go.GetComponent<Rigidbody>();
      if ((UnityEngine.Object) component1 != (UnityEngine.Object) null)
      {
        component1.isKinematic = !enable;
        component1.useGravity = enable;
      }
      Collider component2 = go.GetComponent<Collider>();
      if ((UnityEngine.Object) component2 != (UnityEngine.Object) null)
        component2.enabled = enable;
    }

    void IEntityAttachable.Attach(IEntity owner)
    {
      this.Owner = owner;
      NavigationComponent component = this.Owner.GetComponent<NavigationComponent>();
      if (component == null)
        return;
      component.OnTeleport += new Action<INavigationComponent, IEntity>(this.NavigationComponent_OnTeleport);
      this.NavigationComponent_OnTeleport((INavigationComponent) null, (IEntity) null);
    }

    void IEntityAttachable.Detach()
    {
      NavigationComponent component = this.Owner.GetComponent<NavigationComponent>();
      if (component != null)
        component.OnTeleport -= new Action<INavigationComponent, IEntity>(this.NavigationComponent_OnTeleport);
      this.Owner = (IEntity) null;
    }

    private void NavigationComponent_OnTeleport(
      INavigationComponent navigationComponent,
      IEntity entity)
    {
      LocationItemComponent component = this.Owner?.GetComponent<LocationItemComponent>();
      this.Indoor = component != null && component.IsIndoor;
      Action<bool> indoorChangedEvent = this.IndoorChangedEvent;
      if (indoorChangedEvent == null)
        return;
      indoorChangedEvent(this.Indoor);
    }

    public bool HasFastPOIExit(POIAnimationEnum poiType) => this.supportsFastPOIExits;

    private void Update()
    {
      if (Mathf.Approximately(this.ActualRagdollWeight, this.RagdollWeight))
        return;
      this.ActualRagdollWeight = this.RagdollWeight;
    }

    public enum HierarhyStructureEnum
    {
      Ordinary,
      PuppetMaster,
    }

    public enum AimWeaponType
    {
      Unknown,
      Head,
      Chest,
      Belly,
      Foot,
    }

    public enum SoundEnum
    {
      Unknown,
      RagdollFall,
      HittedVocal,
      HittedLowStaminaVocal,
      HittedDodgeVocal,
      StaggerVocal,
      StaggerNonVocal,
      StrafeLeft,
      StrafeRight,
      StepBack,
      BlockHitted,
      FaceHitted,
      Stab,
      StabBlock,
      BulletHit,
    }
  }
}
