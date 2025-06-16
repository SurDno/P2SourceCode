using System;
using Engine.Common;
using Engine.Common.Components.AttackerPlayer;
using UnityEngine;

public interface INPCWeaponController
{
  event Action<IEntity, ShotType, ReactionType> WeaponShootEvent;

  void Initialise(NPCWeaponService service);

  void Activate();

  void Shutdown();

  void ActivateImmediate();

  void ShutdownImmediate();

  bool Validate(GameObject gameObject);

  void Update();

  void UpdateSilent();

  void TriggerAction(WeaponActionEnum weaponAction);

  void OnAnimatorEvent(string data);

  void IndoorChanged();

  void PunchReaction(ReactionType reactionType);

  bool IsChangingWeapon();
}
