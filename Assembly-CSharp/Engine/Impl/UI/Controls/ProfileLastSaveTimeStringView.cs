using Engine.Source.Services.Profiles;
using System;
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
      if (!((UnityEngine.Object) this.view != (UnityEngine.Object) null))
        return;
      this.view.SkipAnimation();
    }

    protected override void ApplyStringValue()
    {
      if (!((UnityEngine.Object) this.view != (UnityEngine.Object) null) || !Application.isPlaying)
        return;
      string lastSave = ProfilesUtility.GetLastSave(this.StringValue);
      if (lastSave != "")
      {
        DateTime saveCreationTime = ProfilesUtility.GetSaveCreationTime(this.StringValue, lastSave);
        this.view.StringValue = !(saveCreationTime != DateTime.MinValue) ? "" : ProfilesUtility.ConvertCreationTime(saveCreationTime, this.formatTag);
      }
      else
        this.view.StringValue = "";
    }
  }
}
