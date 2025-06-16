using System.ComponentModel;

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
