using Engine.Common.Generator;
using Engine.Impl.Services.Factories;
using UnityEngine;

namespace Engine.Source.Effects.Values
{
  [Factory(typeof (Vector2AbilityValue))]
  [GenerateProxy(TypeEnum.Cloneable | TypeEnum.Copyable | TypeEnum.DataRead | TypeEnum.DataWrite)]
  public class Vector2AbilityValue : AbilityValue<Vector2>
  {
  }
}
