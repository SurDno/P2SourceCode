// Decompiled with JetBrains decompiler
// Type: Engine.Source.Settings.LocalizationSettings
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using Engine.Common.Services;
using Inspectors;

#nullable disable
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
