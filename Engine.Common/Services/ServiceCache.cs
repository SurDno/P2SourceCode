using Cofe.Loggers;

namespace Engine.Common.Services
{
  public static class ServiceCache
  {
    private static IFactory factory;
    private static ISimulation simulation;
    private static ITemplateService templateService;
    private static IOptimizationService optimizationService;
    private static IMMService mindMapService;

    public static IFactory Factory
    {
      get
      {
        if (ServiceCache.factory == null)
        {
          ServiceCache.factory = ServiceLocator.GetService<IFactory>();
          if (ServiceCache.factory == null)
            Logger.AddError("IFactory not found");
        }
        return ServiceCache.factory;
      }
    }

    public static ISimulation Simulation
    {
      get
      {
        if (ServiceCache.simulation == null)
        {
          ServiceCache.simulation = ServiceLocator.GetService<ISimulation>();
          if (ServiceCache.simulation == null)
            Logger.AddError("ISimulation not found");
        }
        return ServiceCache.simulation;
      }
    }

    public static ITemplateService TemplateService
    {
      get
      {
        if (ServiceCache.templateService == null)
        {
          ServiceCache.templateService = ServiceLocator.GetService<ITemplateService>();
          if (ServiceCache.templateService == null)
            Logger.AddError("ITemplateService not found");
        }
        return ServiceCache.templateService;
      }
    }

    public static IOptimizationService OptimizationService
    {
      get
      {
        if (ServiceCache.optimizationService == null)
        {
          ServiceCache.optimizationService = ServiceLocator.GetService<IOptimizationService>();
          if (ServiceCache.optimizationService == null)
            Logger.AddError("IOptimizationService not found");
        }
        return ServiceCache.optimizationService;
      }
    }

    public static IMMService MindMapService
    {
      get
      {
        if (ServiceCache.mindMapService == null)
        {
          ServiceCache.mindMapService = ServiceLocator.GetService<IMMService>();
          if (ServiceCache.mindMapService == null)
            Logger.AddError("IMMService not found");
        }
        return ServiceCache.mindMapService;
      }
    }
  }
}
