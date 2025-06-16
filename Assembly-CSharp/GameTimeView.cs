// Decompiled with JetBrains decompiler
// Type: GameTimeView
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using Engine.Behaviours.Localization;
using Engine.Common.Services;
using System;
using UnityEngine;

#nullable disable
[RequireComponent(typeof (Localizer))]
public class GameTimeView : MonoBehaviour
{
  private Localizer localizer;

  private void OnEnable() => this.Redraw();

  private void Redraw()
  {
    if ((UnityEngine.Object) this.localizer == (UnityEngine.Object) null)
      this.localizer = this.GetComponent<Localizer>();
    ITimeService service = ServiceLocator.GetService<ITimeService>();
    TimeSpan gameTime = service.GameTime;
    if (gameTime.Days < 1)
    {
      this.localizer.Signature = string.Empty;
    }
    else
    {
      Localizer localizer = this.localizer;
      string shortTimeString = service.GameTime.ToShortTimeString();
      gameTime = service.GameTime;
      string str1 = gameTime.Days.ToString();
      string str2 = shortTimeString + " — {DateTime.Day} " + str1;
      localizer.Signature = str2;
    }
  }

  private void Update() => this.Redraw();
}
