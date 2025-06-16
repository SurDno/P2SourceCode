using System.ComponentModel;
using Engine.Common.Binders;

namespace PLVirtualMachine.Common.EngineAPI.VMECS
{
  [EnumType("GameLocalizationName")]
  public enum EVMGameLocalizationName
  {
    [Description("Русский")] russian,
    [Description("Английский")] english,
    [Description("Немецкий")] german,
    [Description("Японский")] japanese,
    [Description("Французский")] french,
  }
}
