using Engine.Behaviours.Components;
using Engine.Common;
using Engine.Common.Components;
using Engine.Common.Components.AttackerPlayer;
using Engine.Source.Commons;
using Engine.Source.Components;
using Engine.Source.Components.Utilities;
using Inspectors;
using System;
using System.Collections.Generic;
using UnityEngine;

public class NPCWeaponService : WeaponServiceBase, IEntityAttachable
{
  private const float unknownRelaxTime = 2f;
  public GameObject KnifePrefab;
  public Transform KnifeParent;
  public GameObject BombPrefab;
  public Transform BombParent;
  public GameObject SamopalPrefab;
  public Transform SamopalParent;
  public GameObject Rifle;
  private Vector3 knifeSpeed;
  public Vector3 ProjectileHitPosition;
  public Quaternion ProjectileHitRotation;
  private WeaponEnum weapon = WeaponEnum.Unknown;
  private Animator animator;
  private FightAnimatorBehavior.AnimatorState fightAnimatorState;
  private AnimatorEventProxy animatorEventProxy;
  private Vector3 knifeParentPositionPrev;
  private bool inited = false;
  private Vector3 weaponStartPosition;
  private Vector3 weaponAimDirection;
  private Dictionary<int, float> neededLayerWeights = new Dictionary<int, float>();
  private Dictionary<int, float> currentLayerWeights = new Dictionary<int, float>();
  private float unknownRelaxTimeLeft;
  private Dictionary<WeaponEnum, INPCWeaponController> weaponControllers = new Dictionary<WeaponEnum, INPCWeaponController>((IEqualityComparer<WeaponEnum>) WeaponEnumComparer.Instance)
  {
    {
      WeaponEnum.Unknown,
      (INPCWeaponController) new NPCEmptyWeaponController()
    },
    {
      WeaponEnum.Hands,
      (INPCWeaponController) new NPCHandsWeaponController()
    },
    {
      WeaponEnum.Knife,
      (INPCWeaponController) new NPCKnifeWeaponController()
    },
    {
      WeaponEnum.Flamethrower,
      (INPCWeaponController) new NPCFlamethrowerWeaponController()
    },
    {
      WeaponEnum.Bomb,
      (INPCWeaponController) new NPCBombWeaponController()
    },
    {
      WeaponEnum.Samopal,
      (INPCWeaponController) new NPCSamopalWeaponController()
    },
    {
      WeaponEnum.Rifle,
      (INPCWeaponController) new NPCRifleWeaponController()
    },
    {
      WeaponEnum.RifleClose,
      (INPCWeaponController) new NPCRifleCloseWeaponController()
    }
  };

  [Inspected]
  public IEntity Owner { get; private set; }

  [Inspected]
  public bool IsIndoor { get; private set; }

  public Pivot pivot { get; private set; }

  public event Action<WeaponEnum, IEntity, ShotType, ReactionType> WeaponShootEvent;

  public float ReactionLayerWeight { get; set; }

  [Inspected(Mutable = true)]
  public override WeaponEnum Weapon
  {
    get => this.weapon;
    set
    {
      if (!this.inited)
        this.Init();
      if (value != WeaponEnum.Unknown && this.weapon == value)
        return;
      if (value == WeaponEnum.Unknown)
        this.unknownRelaxTimeLeft = 2f;
      this.weaponControllers[this.weapon].Shutdown();
      this.weaponControllers[value].Activate();
      this.weapon = value;
    }
  }

  public bool IsChangingWeapon() => this.weaponControllers[this.weapon].IsChangingWeapon();

  public void SwitchWeaponOnImmediate()
  {
    foreach (KeyValuePair<WeaponEnum, INPCWeaponController> weaponController in this.weaponControllers)
    {
      if (weaponController.Key != this.weapon)
        weaponController.Value.ShutdownImmediate();
    }
    this.weaponControllers[this.weapon].ActivateImmediate();
  }

  public void SwitchWeaponOffImmediate()
  {
    foreach (KeyValuePair<WeaponEnum, INPCWeaponController> weaponController in this.weaponControllers)
      weaponController.Value.ShutdownImmediate();
    this.weapon = WeaponEnum.Unknown;
    this.weaponControllers[WeaponEnum.Unknown].ActivateImmediate();
  }

  private void Init()
  {
    this.pivot = this.GetComponent<Pivot>();
    if (!(bool) (UnityEngine.Object) this.pivot)
    {
      Debug.LogError((object) (typeof (NPCWeaponService).Name + " requires " + typeof (Pivot).Name + " "));
    }
    else
    {
      this.animator = this.pivot.GetAnimator();
      this.animatorEventProxy = this.pivot.GetAnimatorEventProxy();
      this.animatorEventProxy.AnimatorEventEvent += new Action<string>(this.OnAnimatorEvent);
      this.fightAnimatorState = FightAnimatorBehavior.GetAnimatorState(this.animator);
      foreach (KeyValuePair<WeaponEnum, INPCWeaponController> weaponController1 in this.weaponControllers)
      {
        INPCWeaponController weaponController2 = weaponController1.Value;
        WeaponEnum weaponKind = weaponController1.Key;
        weaponController2.Initialise(this);
        weaponController2.WeaponShootEvent += (Action<IEntity, ShotType, ReactionType>) ((entity, shotType, reactionType) =>
        {
          if (this.fightAnimatorState.IsReaction)
            return;
          Action<WeaponEnum, IEntity, ShotType, ReactionType> weaponShootEvent = this.WeaponShootEvent;
          if (weaponShootEvent == null)
            return;
          weaponShootEvent(weaponKind, entity, shotType, reactionType);
        });
      }
      this.inited = true;
    }
  }

  void IEntityAttachable.Attach(IEntity owner)
  {
    this.Owner = owner;
    NavigationComponent component = this.Owner.GetComponent<NavigationComponent>();
    if (component == null)
      return;
    component.OnTeleport += new Action<INavigationComponent, IEntity>(this.NavigationComponent_OnTeleport);
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
    this.IsIndoor = component != null && component.IsIndoor;
    this.weaponControllers[this.weapon].IndoorChanged();
  }

  private void Start()
  {
    if (this.inited)
      return;
    this.Init();
  }

  public override Vector3 KnifeSpeed => this.knifeSpeed;

  public override Vector3 KnifePosition
  {
    get
    {
      return (UnityEngine.Object) this.KnifeParent != (UnityEngine.Object) null ? this.KnifeParent.position : this.transform.position;
    }
  }

  private void Update()
  {
    if (!(bool) (UnityEngine.Object) this.pivot || InstanceByRequest<EngineApplication>.Instance.IsPaused)
      return;
    if (this.weapon == WeaponEnum.Unknown)
    {
      if ((double) this.unknownRelaxTimeLeft <= 0.0)
        return;
      this.unknownRelaxTimeLeft -= Time.deltaTime;
    }
    foreach (KeyValuePair<WeaponEnum, INPCWeaponController> weaponController in this.weaponControllers)
    {
      if (weaponController.Key == this.weapon)
        weaponController.Value.Update();
      else
        weaponController.Value.UpdateSilent();
    }
    this.CountLayersWeights();
    if ((UnityEngine.Object) this.KnifeParent != (UnityEngine.Object) null)
    {
      if (this.knifeParentPositionPrev == Vector3.zero)
        this.knifeParentPositionPrev = this.KnifeParent.position;
      Vector3 vector3_1 = this.KnifeParent.position - this.knifeParentPositionPrev;
      Vector3 vector3_2 = (double) vector3_1.magnitude > (double) Time.deltaTime * 0.05000000074505806 ? vector3_1 / Time.deltaTime : Vector3.zero;
      float num = 1f;
      this.knifeSpeed = this.knifeSpeed * (1f - num) + vector3_2 * num;
      this.knifeParentPositionPrev = this.KnifeParent.position;
    }
    if (!((UnityEngine.Object) this.pivot != (UnityEngine.Object) null) || !((UnityEngine.Object) this.pivot.ShootStart != (UnityEngine.Object) null))
      return;
    this.weaponStartPosition = this.pivot.ShootStart.transform.position;
    this.weaponAimDirection = this.pivot.ShootStart.transform.forward;
  }

  public void AddNeededLayer(int key, float value)
  {
    if ((UnityEngine.Object) this.animator == (UnityEngine.Object) null)
      return;
    if (!this.neededLayerWeights.ContainsKey(key))
    {
      this.neededLayerWeights.Add(key, value);
      this.currentLayerWeights.Add(key, this.animator.GetLayerWeight(key));
    }
    else
      this.neededLayerWeights[key] = value;
  }

  public void ForceUpdateLayers() => this.CountLayersWeights(true);

  private void CountLayersWeights(bool immediate = false)
  {
    foreach (KeyValuePair<int, float> neededLayerWeight in this.neededLayerWeights)
    {
      if (neededLayerWeight.Key != -1)
      {
        float currentLayerWeight = this.currentLayerWeights[neededLayerWeight.Key];
        float num = Mathf.MoveTowards(currentLayerWeight, neededLayerWeight.Value, Time.deltaTime / 0.5f);
        if (immediate)
          num = neededLayerWeight.Value;
        if (!Mathf.Approximately(num, currentLayerWeight))
        {
          this.currentLayerWeights[neededLayerWeight.Key] = num;
          float weight = SmoothUtility.Smooth22(num);
          if ((double) this.animator.GetLayerWeight(neededLayerWeight.Key) != (double) weight)
            this.animator.SetLayerWeight(neededLayerWeight.Key, weight);
        }
      }
    }
  }

  public void TriggerAction(WeaponActionEnum weaponAction)
  {
    this.weaponControllers[this.weapon].TriggerAction(weaponAction);
  }

  public void OnAnimatorEvent(string data)
  {
    this.weaponControllers[this.weapon].OnAnimatorEvent(data);
  }

  public Vector3 GetWeaponStartPoint() => this.weaponStartPosition;

  public Vector3 GetWeaponAimDirection() => this.weaponAimDirection;

  public void PunchReaction(ReactionType reactionType)
  {
    this.weaponControllers[this.weapon].PunchReaction(reactionType);
  }
}
