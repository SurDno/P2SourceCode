// Decompiled with JetBrains decompiler
// Type: Engine.Behaviours.Engines.Controllers.IMovementController
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using Engine.Behaviours.Components;
using UnityEngine;

#nullable disable
namespace Engine.Behaviours.Engines.Controllers
{
  public interface IMovementController
  {
    bool IsPaused { get; set; }

    bool GeometryVisible { set; }

    void Initialize(GameObject gameObject);

    void StartMovement(Vector3 direction, EngineBehavior.GaitType gait);

    bool Move(Vector3 direction, float remainingDistance, EngineBehavior.GaitType gait);

    bool Rotate(Vector3 direction);

    void OnAnimatorMove();

    void Update();

    void FixedUpdate();
  }
}
