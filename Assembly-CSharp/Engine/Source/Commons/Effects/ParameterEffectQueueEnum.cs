using System.ComponentModel;

namespace Engine.Source.Commons.Effects
{
  public enum ParameterEffectQueueEnum
  {
    None = 0,
    [Description("Предварительные изменения")] Prepare = 100,
    [Description("Первичная обработка")] FirstCompute = 150,
    [Description("Добавить урон")] AddDamage = 160,
    [Description("Уменьшить урон")] ReduceDamage = 170,
    [Description("Положительные эффекты")] Buff = 200,
    [Description("Отрицательные эффекты")] Debuff = 300,
    [Description("Применить урон")] ApplyDamage = 500,
    [Description("Окончательные изменения")] FinalApply = 1000,
  }
}
