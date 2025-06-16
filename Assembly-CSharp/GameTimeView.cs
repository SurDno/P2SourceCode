using Engine.Behaviours.Localization;
using Engine.Common.Services;
using System;
using UnityEngine;

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
