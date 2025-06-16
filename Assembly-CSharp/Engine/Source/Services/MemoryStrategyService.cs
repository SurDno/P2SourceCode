using Engine.Common;
using Engine.Common.Services;
using Engine.Impl.Services;
using Engine.Source.Commons;
using Engine.Source.Otimizations;
using Engine.Source.Settings.External;
using Engine.Source.Utility;
using System.Collections;
using UnityEngine;

namespace Engine.Source.Services
{
  [GameService(new System.Type[] {typeof (MemoryStrategyService)})]
  public class MemoryStrategyService : IInitialisable, IUpdatable
  {
    private static float timeLeft;
    private IEnumerator enumerator;

    public static void ResetTime()
    {
      MemoryStrategyService.timeLeft = Mathf.Max((float) ExternalSettingsInstance<ExternalOptimizationSettings>.Instance.MemoryStrategyTimeEventPeriod, 10f);
    }

    public void ComputeUpdate()
    {
      if (this.enumerator != null)
      {
        if (this.enumerator.MoveNext())
          return;
        this.StartGame();
        this.enumerator = (IEnumerator) null;
      }
      MemoryStrategyService.timeLeft -= Time.deltaTime;
      if ((double) MemoryStrategyService.timeLeft > 0.0 || InstanceByRequest<EngineApplication>.Instance.IsPaused || !PlayerUtility.IsPlayerCanControlling)
        return;
      MemoryStrategyService.ResetTime();
      this.enumerator = MemoryStrategy.Instance.Compute(MemoryStrategyContextEnum.Time);
      if (this.enumerator.MoveNext())
        this.StopGame();
      else
        this.enumerator = (IEnumerator) null;
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
      MemoryStrategyService.ResetTime();
      InstanceByRequest<UpdateService>.Instance.Updater.AddUpdatable((IUpdatable) this);
    }

    public void Terminate()
    {
      InstanceByRequest<UpdateService>.Instance.Updater.RemoveUpdatable((IUpdatable) this);
    }
  }
}
