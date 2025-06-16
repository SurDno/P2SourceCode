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
    if (view == null)
      return;
    ProfilesService service = ServiceLocator.GetService<ProfilesService>();
    if (service != null)
    {
      string gameName = ProfilesUtility.GetGameName(service.Current.Name);
      int intValue = service.GetIntValue("Deaths");
      view.StringValue = ServiceLocator.GetService<LocalizationService>().GetText("{UI.Menu.Main.Profile.Deaths}").Replace("<gamename>", gameName).Replace("<deaths>", intValue.ToString());
    }
    else
      view.StringValue = ServiceLocator.GetService<LocalizationService>().GetText("{UI.Menu.Main.Profile.Empty}");
  }
}
