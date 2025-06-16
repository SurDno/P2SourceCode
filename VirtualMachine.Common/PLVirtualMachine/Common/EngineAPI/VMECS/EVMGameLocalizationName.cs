using Engine.Common.Binders;
using System.ComponentModel;

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
