using Engine.Common.Services;
using Inspectors;

namespace Engine.Source.Settings
{
  public class LocalizationSettings : SettingsInstanceByRequest<LocalizationSettings>
  {
    [Inspected]
    public IValue<LanguageEnum> OverrideLanguage = (IValue<LanguageEnum>) new EnumValue<LanguageEnum>(nameof (OverrideLanguage));
    [Inspected]
    public IValue<LanguageEnum> Language = (IValue<LanguageEnum>) new EnumValue<LanguageEnum>(nameof (Language));
    [Inspected]
    public IValue<LanguageEnum> SubTitlesLanguage = (IValue<LanguageEnum>) new EnumValue<LanguageEnum>(nameof (SubTitlesLanguage));
    [Inspected]
    public IValue<LanguageEnum> LipSyncLanguage = (IValue<LanguageEnum>) new EnumValue<LanguageEnum>(nameof (LipSyncLanguage));
  }
}
