using Engine.Source.Settings.External;

namespace Engine.Source.Services.Templates
{
  public static class TemplateLoader
  {
    private static ITemplateLoader instance;

    public static ITemplateLoader Instance
    {
      get
      {
        if (instance == null)
          instance = !Compress ? new RuntimeTemplateLoader() : new RuntimeCompressedTemplateLoader();
        return instance;
      }
    }

    private static bool Compress
    {
      get => ExternalSettingsInstance<ExternalOptimizationSettings>.Instance.UseCompressedTemplates;
    }
  }
}
