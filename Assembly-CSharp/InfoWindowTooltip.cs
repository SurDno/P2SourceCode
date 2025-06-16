using Engine.Common.Services;
using Engine.Impl.Services;
using Engine.Impl.UI;
using UnityEngine;
using UnityEngine.UI;

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
