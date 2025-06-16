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
        if (factory == null)
        {
          factory = ServiceLocator.GetService<IFactory>();
          if (factory == null)
            Logger.AddError("IFactory not found");
        }
        return factory;
      }
    }

    public static ISimulation Simulation
    {
      get
      {
        if (simulation == null)
        {
          simulation = ServiceLocator.GetService<ISimulation>();
          if (simulation == null)
            Logger.AddError("ISimulation not found");
        }
        return simulation;
      }
    }

    public static ITemplateService TemplateService
    {
      get
      {
        if (templateService == null)
        {
          templateService = ServiceLocator.GetService<ITemplateService>();
          if (templateService == null)
            Logger.AddError("ITemplateService not found");
        }
        return templateService;
      }
    }

    public static IOptimizationService OptimizationService
    {
      get
      {
        if (optimizationService == null)
        {
          optimizationService = ServiceLocator.GetService<IOptimizationService>();
          if (optimizationService == null)
            Logger.AddError("IOptimizationService not found");
        }
        return optimizationService;
      }
    }

    public static IMMService MindMapService
    {
      get
      {
        if (mindMapService == null)
        {
          mindMapService = ServiceLocator.GetService<IMMService>();
          if (mindMapService == null)
            Logger.AddError("IMMService not found");
        }
        return mindMapService;
      }
    }
  }
}
