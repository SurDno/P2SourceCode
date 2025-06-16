using System;
using Engine.Behaviours.Localization;
using Engine.Common.Services;
using Engine.Source.Services.Times;
using UnityEngine;

[RequireComponent(typeof (Localizer))]
public class GameTimeView : MonoBehaviour
{
  private Localizer localizer;

  private void OnEnable() => Redraw();

  private void Redraw()
  {
    if (this.localizer == null)
      this.localizer = GetComponent<Localizer>();
    ITimeService service = ServiceLocator.GetService<ITimeService>();
    TimeSpan gameTime = service.GameTime;
    if (gameTime.Days < 1)
    {
      localizer.Signature = string.Empty;
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

  private void Update() => Redraw();
}
