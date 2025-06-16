// Decompiled with JetBrains decompiler
// Type: Engine.Impl.UI.Controls.ProfileLastSaveTimeStringView
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using Engine.Source.Services.Profiles;
using System;
using UnityEngine;

#nullable disable
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
