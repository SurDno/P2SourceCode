// Decompiled with JetBrains decompiler
// Type: Engine.Behaviours.Engines.Controllers.IWeaponController
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using Engine.Common;
using Engine.Common.Components.AttackerPlayer;
using System;
using UnityEngine;

#nullable disable
namespace Engine.Behaviours.Engines.Controllers
{
  public interface IWeaponController
  {
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
}
