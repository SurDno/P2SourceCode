// Decompiled with JetBrains decompiler
// Type: InfoWindowTooltip
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using Engine.Common.Services;
using Engine.Impl.Services;
using Engine.Impl.UI;
using UnityEngine;
using UnityEngine.UI;

#nullable disable
public class InfoWindowTooltip : UIControl
{
  [SerializeField]
  private Text Name;
  [SerializeField]
  private Text Value;

  public void SetParams(StorableTooltipInfo info)
  {
    if ((Object) this.Name != (Object) null && info != null)
      this.Name.text = ServiceLocator.GetService<LocalizationService>().GetText(info.Name.ToString());
    if (!((Object) this.Value != (Object) null) || info == null || info.Value == null)
      return;
    this.Value.text = ServiceLocator.GetService<LocalizationService>().GetText(info.Value.ToString());
    this.Value.color = info.Color;
  }
}
