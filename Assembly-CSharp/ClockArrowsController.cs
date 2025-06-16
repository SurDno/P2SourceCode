// Decompiled with JetBrains decompiler
// Type: ClockArrowsController
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using Engine.Common.Services;
using System;
using UnityEngine;

#nullable disable
public class ClockArrowsController : MonoBehaviour
{
  private ITimeService timeService;

  private void Start() => this.timeService = ServiceLocator.GetService<ITimeService>();

  private void Update()
  {
    if (this.timeService == null)
      return;
    Animator component = this.gameObject.GetComponent<Animator>();
    TimeSpan gameTime = this.timeService.GameTime;
    double num1 = (double) (gameTime.Hours % 12);
    gameTime = this.timeService.GameTime;
    double num2 = (double) gameTime.Minutes / 60.0;
    double num3 = num1 + num2;
    gameTime = this.timeService.GameTime;
    double num4 = (double) gameTime.Seconds / 3600.0;
    float num5 = (float) (num3 + num4) / 12f;
    component.SetFloat("time", num5);
  }
}
