using Engine.Common.Services;
using Engine.Impl.Services;
using Engine.Impl.UI;

public class InfoWindowTooltip : UIControl
{
  [SerializeField]
  private Text Name;
  [SerializeField]
  private Text Value;

  public void SetParams(StorableTooltipInfo info)
  {
    if ((Object) Name != (Object) null && info != null)
      Name.text = ServiceLocator.GetService<LocalizationService>().GetText(info.Name.ToString());
    if (!((Object) Value != (Object) null) || info == null || info.Value == null)
      return;
    Value.text = ServiceLocator.GetService<LocalizationService>().GetText(info.Value);
    Value.color = info.Color;
  }
}
