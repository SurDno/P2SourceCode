using System.Collections;
using Engine.Common;
using Engine.Common.Services;
using Engine.Impl.Services;
using Engine.Source.Commons;
using Engine.Source.Otimizations;
using Engine.Source.Settings.External;
using Engine.Source.Utility;

namespace Engine.Source.Services
{
  [GameService(typeof (MemoryStrategyService))]
  public class MemoryStrategyService : IInitialisable, IUpdatable
  {
    private static float timeLeft;
    private IEnumerator enumerator;

    public static void ResetTime()
    {
      timeLeft = Mathf.Max((float) ExternalSettingsInstance<ExternalOptimizationSettings>.Instance.MemoryStrategyTimeEventPeriod, 10f);
    }

    public void ComputeUpdate()
    {
      if (enumerator != null)
      {
        if (enumerator.MoveNext())
          return;
        StartGame();
        enumerator = null;
      }
      timeLeft -= Time.deltaTime;
      if (timeLeft > 0.0 || InstanceByRequest<EngineApplication>.Instance.IsPaused || !PlayerUtility.IsPlayerCanControlling)
        return;
      ResetTime();
      enumerator = MemoryStrategy.Instance.Compute(MemoryStrategyContextEnum.Time);
      if (enumerator.MoveNext())
        StopGame();
      else
        enumerator = null;
    }

    private void StopGame()
    {
      InstanceByRequest<EngineApplication>.Instance.IsPaused = true;
      ServiceLocator.GetService<UIService>().SmallLoading.gameObject.SetActive(true);
    }

    private void StartGame()
    {
      InstanceByRequest<EngineApplication>.Instance.IsPaused = false;
      ServiceLocator.GetService<UIService>().SmallLoading.gameObject.SetActive(false);
    }

    public void Initialise()
    {
      ResetTime();
      InstanceByRequest<UpdateService>.Instance.Updater.AddUpdatable(this);
    }

    public void Terminate()
    {
      InstanceByRequest<UpdateService>.Instance.Updater.RemoveUpdatable(this);
    }
  }
}
