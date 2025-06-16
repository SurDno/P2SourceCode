using System;
using Engine.Common.Services;

public class ClockArrowsController : MonoBehaviour
{
  private ITimeService timeService;

  private void Start() => timeService = ServiceLocator.GetService<ITimeService>();

  private void Update()
  {
    if (timeService == null)
      return;
    Animator component = this.gameObject.GetComponent<Animator>();
    TimeSpan gameTime = timeService.GameTime;
    double num1 = gameTime.Hours % 12;
    gameTime = timeService.GameTime;
    double num2 = gameTime.Minutes / 60.0;
    double num3 = num1 + num2;
    gameTime = timeService.GameTime;
    double num4 = gameTime.Seconds / 3600.0;
    float num5 = (float) (num3 + num4) / 12f;
    component.SetFloat("time", num5);
  }
}
