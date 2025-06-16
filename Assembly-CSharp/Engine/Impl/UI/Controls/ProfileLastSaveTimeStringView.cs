using System;
using Engine.Source.Services.Profiles;
using UnityEngine;

namespace Engine.Impl.UI.Controls
{
  public class ProfileLastSaveTimeStringView : StringView
  {
    [SerializeField]
    private StringView view;
    [SerializeField]
    private string formatTag;

    public override void SkipAnimation()
    {
      if (!(view != null))
        return;
      view.SkipAnimation();
    }

    protected override void ApplyStringValue()
    {
      if (!(view != null) || !Application.isPlaying)
        return;
      string lastSave = ProfilesUtility.GetLastSave(StringValue);
      if (lastSave != "")
      {
        DateTime saveCreationTime = ProfilesUtility.GetSaveCreationTime(StringValue, lastSave);
        view.StringValue = !(saveCreationTime != DateTime.MinValue) ? "" : ProfilesUtility.ConvertCreationTime(saveCreationTime, formatTag);
      }
      else
        view.StringValue = "";
    }
  }
}
