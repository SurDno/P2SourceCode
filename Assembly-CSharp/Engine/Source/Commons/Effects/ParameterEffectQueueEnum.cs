// Decompiled with JetBrains decompiler
// Type: Engine.Source.Commons.Effects.ParameterEffectQueueEnum
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using System.ComponentModel;

#nullable disable
namespace Engine.Source.Commons.Effects
{
  public enum ParameterEffectQueueEnum
  {
    None = 0,
    [Description("Предварительные изменения")] Prepare = 100, // 0x00000064
    [Description("Первичная обработка")] FirstCompute = 150, // 0x00000096
    [Description("Добавить урон")] AddDamage = 160, // 0x000000A0
    [Description("Уменьшить урон")] ReduceDamage = 170, // 0x000000AA
    [Description("Положительные эффекты")] Buff = 200, // 0x000000C8
    [Description("Отрицательные эффекты")] Debuff = 300, // 0x0000012C
    [Description("Применить урон")] ApplyDamage = 500, // 0x000001F4
    [Description("Окончательные изменения")] FinalApply = 1000, // 0x000003E8
  }
}
