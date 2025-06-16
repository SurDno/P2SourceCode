// Decompiled with JetBrains decompiler
// Type: Engine.Source.Commons.Effects.IEffect
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using Engine.Common;
using Engine.Source.Commons.Abilities;

#nullable disable
namespace Engine.Source.Commons.Effects
{
  public interface IEffect
  {
    string Name { get; }

    AbilityItem AbilityItem { get; set; }

    IEntity Target { get; set; }

    ParameterEffectQueueEnum Queue { get; }

    bool Prepare(float currentRealTime, float currentGameTime);

    bool Compute(float currentRealTime, float currentGameTime);

    void Cleanup();
  }
}
