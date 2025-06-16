using System;
using Engine.Common;
using Engine.Common.Components.AttackerPlayer;
using UnityEngine;

namespace Engine.Behaviours.Engines.Controllers;

public interface IWeaponController {
	event Action WeaponHolsterStartEvent;

	event Action WeaponUnholsterEndEvent;

	event Action<IEntity, ShotType, ReactionType, ShotSubtypeEnum> WeaponShootEvent;

	bool GeometryVisible { set; get; }

	void SetItem(IEntity item);

	IEntity GetItem();

	void Initialise(IEntity entity, GameObject gameObject, Animator animator);

	void Activate(bool geometryVisible);

	void Shutdown();

	void Reset();

	bool Validate(GameObject gameObject, IEntity item);

	void FixedUpdate(IEntity target);

	void Update(IEntity target);

	void UpdateSilent(IEntity target);

	void LateUpdate(IEntity target);

	void OnAnimatorEvent(string data);

	void OnEnable();

	void OnDisable();

	void Reaction();
}