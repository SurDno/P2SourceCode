using Engine.Common.Services;
using Inspectors;

namespace Engine.Source.Settings;

public class LocalizationSettings : SettingsInstanceByRequest<LocalizationSettings> {
	[Inspected] public IValue<LanguageEnum> OverrideLanguage = new EnumValue<LanguageEnum>(nameof(OverrideLanguage));
	[Inspected] public IValue<LanguageEnum> Language = new EnumValue<LanguageEnum>(nameof(Language));
	[Inspected] public IValue<LanguageEnum> SubTitlesLanguage = new EnumValue<LanguageEnum>(nameof(SubTitlesLanguage));
	[Inspected] public IValue<LanguageEnum> LipSyncLanguage = new EnumValue<LanguageEnum>(nameof(LipSyncLanguage));
}