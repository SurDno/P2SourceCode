using Engine.Common.Services;
using Engine.Impl.Services;
using Engine.Impl.UI.Controls;
using Engine.Source.Services;
using Engine.Source.Services.Profiles;
using UnityEngine;

public class CurrentProfileView : MonoBehaviour
{
  [SerializeField]
  private StringView view;

  private void OnEnable()
  {
    if ((Object) this.view == (Object) null)
      return;
    ProfilesService service = ServiceLocator.GetService<ProfilesService>();
    if (service != null)
    {
      string gameName = ProfilesUtility.GetGameName(service.Current.Name);
      int intValue = service.GetIntValue("Deaths");
      this.view.StringValue = ServiceLocator.GetService<LocalizationService>().GetText("{UI.Menu.Main.Profile.Deaths}").Replace("<gamename>", gameName).Replace("<deaths>", intValue.ToString());
    }
    else
      this.view.StringValue = ServiceLocator.GetService<LocalizationService>().GetText("{UI.Menu.Main.Profile.Empty}");
  }
}
