// Decompiled with JetBrains decompiler
// Type: Engine.Source.Settings.CommonSettings
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using Inspectors;

#nullable disable
namespace Engine.Source.Settings
{
  public class CommonSettings : SettingsInstanceByRequest<CommonSettings>
  {
    [Inspected]
    public IValue<bool> NotFirstStart = (IValue<bool>) new BoolValue(nameof (NotFirstStart));
  }
}
