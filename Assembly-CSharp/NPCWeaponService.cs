using System;
using System.Collections.Generic;
using Engine.Behaviours.Components;
using Engine.Common;
using Engine.Common.Components;
using Engine.Common.Components.AttackerPlayer;
using Engine.Source.Commons;
using Engine.Source.Components;
using Engine.Source.Components.Utilities;
using Inspectors;
using UnityEngine;
using Object = UnityEngine.Object;

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
  private bool inited;
  private Vector3 weaponStartPosition;
  private Vector3 weaponAimDirection;
  private Dictionary<int, float> neededLayerWeights = new();
  private Dictionary<int, float> currentLayerWeights = new();
  private float unknownRelaxTimeLeft;
  private Dictionary<WeaponEnum, INPCWeaponController> weaponControllers = new(WeaponEnumComparer.Instance)
  {
    {
      WeaponEnum.Unknown,
      new NPCEmptyWeaponController()
    },
    {
      WeaponEnum.Hands,
      new NPCHandsWeaponController()
    },
    {
      WeaponEnum.Knife,
      new NPCKnifeWeaponController()
    },
    {
      WeaponEnum.Flamethrower,
      new NPCFlamethrowerWeaponController()
    },
    {
      WeaponEnum.Bomb,
      new NPCBombWeaponController()
    },
    {
      WeaponEnum.Samopal,
      new NPCSamopalWeaponController()
    },
    {
      WeaponEnum.Rifle,
      new NPCRifleWeaponController()
    },
    {
      WeaponEnum.RifleClose,
      new NPCRifleCloseWeaponController()
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
    get => weapon;
    set
    {
      if (!inited)
        Init();
      if (value != WeaponEnum.Unknown && weapon == value)
        return;
      if (value == WeaponEnum.Unknown)
        unknownRelaxTimeLeft = 2f;
      weaponControllers[weapon].Shutdown();
      weaponControllers[value].Activate();
      weapon = value;
    }
  }

  public bool IsChangingWeapon() => weaponControllers[weapon].IsChangingWeapon();

  public void SwitchWeaponOnImmediate()
  {
    foreach (KeyValuePair<WeaponEnum, INPCWeaponController> weaponController in weaponControllers)
    {
      if (weaponController.Key != weapon)
        weaponController.Value.ShutdownImmediate();
    }
    weaponControllers[weapon].ActivateImmediate();
  }

  public void SwitchWeaponOffImmediate()
  {
    foreach (KeyValuePair<WeaponEnum, INPCWeaponController> weaponController in weaponControllers)
      weaponController.Value.ShutdownImmediate();
    weapon = WeaponEnum.Unknown;
    weaponControllers[WeaponEnum.Unknown].ActivateImmediate();
  }

  private void Init()
  {
    pivot = GetComponent<Pivot>();
    if (!(bool) (Object) pivot)
    {
      Debug.LogError(typeof (NPCWeaponService).Name + " requires " + typeof (Pivot).Name + " ");
    }
    else
    {
      animator = pivot.GetAnimator();
      animatorEventProxy = pivot.GetAnimatorEventProxy();
      animatorEventProxy.AnimatorEventEvent += OnAnimatorEvent;
      fightAnimatorState = FightAnimatorBehavior.GetAnimatorState(animator);
      foreach (KeyValuePair<WeaponEnum, INPCWeaponController> weaponController1 in weaponControllers)
      {
        INPCWeaponController weaponController2 = weaponController1.Value;
        WeaponEnum weaponKind = weaponController1.Key;
        weaponController2.Initialise(this);
        weaponController2.WeaponShootEvent += (entity, shotType, reactionType) =>
        {
          if (fightAnimatorState.IsReaction)
            return;
          Action<WeaponEnum, IEntity, ShotType, ReactionType> weaponShootEvent = WeaponShootEvent;
          if (weaponShootEvent == null)
            return;
          weaponShootEvent(weaponKind, entity, shotType, reactionType);
        };
      }
      inited = true;
    }
  }

  void IEntityAttachable.Attach(IEntity owner)
  {
    Owner = owner;
    NavigationComponent component = Owner.GetComponent<NavigationComponent>();
    if (component == null)
      return;
    component.OnTeleport += NavigationComponent_OnTeleport;
  }

  void IEntityAttachable.Detach()
  {
    NavigationComponent component = Owner.GetComponent<NavigationComponent>();
    if (component != null)
      component.OnTeleport -= NavigationComponent_OnTeleport;
    Owner = null;
  }

  private void NavigationComponent_OnTeleport(
    INavigationComponent navigationComponent,
    IEntity entity)
  {
    LocationItemComponent component = Owner?.GetComponent<LocationItemComponent>();
    IsIndoor = component != null && component.IsIndoor;
    weaponControllers[weapon].IndoorChanged();
  }

  private void Start()
  {
    if (inited)
      return;
    Init();
  }

  public override Vector3 KnifeSpeed => knifeSpeed;

  public override Vector3 KnifePosition => KnifeParent != null ? KnifeParent.position : transform.position;

  private void Update()
  {
    if (!(bool) (Object) pivot || InstanceByRequest<EngineApplication>.Instance.IsPaused)
      return;
    if (weapon == WeaponEnum.Unknown)
    {
      if (unknownRelaxTimeLeft <= 0.0)
        return;
      unknownRelaxTimeLeft -= Time.deltaTime;
    }
    foreach (KeyValuePair<WeaponEnum, INPCWeaponController> weaponController in weaponControllers)
    {
      if (weaponController.Key == weapon)
        weaponController.Value.Update();
      else
        weaponController.Value.UpdateSilent();
    }
    CountLayersWeights();
    if (KnifeParent != null)
    {
      if (knifeParentPositionPrev == Vector3.zero)
        knifeParentPositionPrev = KnifeParent.position;
      Vector3 vector3_1 = KnifeParent.position - knifeParentPositionPrev;
      Vector3 vector3_2 = vector3_1.magnitude > Time.deltaTime * 0.05000000074505806 ? vector3_1 / Time.deltaTime : Vector3.zero;
      float num = 1f;
      knifeSpeed = knifeSpeed * (1f - num) + vector3_2 * num;
      knifeParentPositionPrev = KnifeParent.position;
    }
    if (!(pivot != null) || !(pivot.ShootStart != null))
      return;
    weaponStartPosition = pivot.ShootStart.transform.position;
    weaponAimDirection = pivot.ShootStart.transform.forward;
  }

  public void AddNeededLayer(int key, float value)
  {
    if (animator == null)
      return;
    if (!neededLayerWeights.ContainsKey(key))
    {
      neededLayerWeights.Add(key, value);
      currentLayerWeights.Add(key, animator.GetLayerWeight(key));
    }
    else
      neededLayerWeights[key] = value;
  }

  public void ForceUpdateLayers() => CountLayersWeights(true);

  private void CountLayersWeights(bool immediate = false)
  {
    foreach (KeyValuePair<int, float> neededLayerWeight in neededLayerWeights)
    {
      if (neededLayerWeight.Key != -1)
      {
        float currentLayerWeight = currentLayerWeights[neededLayerWeight.Key];
        float num = Mathf.MoveTowards(currentLayerWeight, neededLayerWeight.Value, Time.deltaTime / 0.5f);
        if (immediate)
          num = neededLayerWeight.Value;
        if (!Mathf.Approximately(num, currentLayerWeight))
        {
          currentLayerWeights[neededLayerWeight.Key] = num;
          float weight = SmoothUtility.Smooth22(num);
          if (animator.GetLayerWeight(neededLayerWeight.Key) != (double) weight)
            animator.SetLayerWeight(neededLayerWeight.Key, weight);
        }
      }
    }
  }

  public void TriggerAction(WeaponActionEnum weaponAction)
  {
    weaponControllers[weapon].TriggerAction(weaponAction);
  }

  public void OnAnimatorEvent(string data)
  {
    weaponControllers[weapon].OnAnimatorEvent(data);
  }

  public Vector3 GetWeaponStartPoint() => weaponStartPosition;

  public Vector3 GetWeaponAimDirection() => weaponAimDirection;

  public void PunchReaction(ReactionType reactionType)
  {
    weaponControllers[weapon].PunchReaction(reactionType);
  }
}
