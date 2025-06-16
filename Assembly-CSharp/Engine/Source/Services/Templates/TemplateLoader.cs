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
        if (TemplateLoader.instance == null)
          TemplateLoader.instance = !TemplateLoader.Compress ? (ITemplateLoader) new RuntimeTemplateLoader() : (ITemplateLoader) new RuntimeCompressedTemplateLoader();
        return TemplateLoader.instance;
      }
    }

    private static bool Compress
    {
      get => ExternalSettingsInstance<ExternalOptimizationSettings>.Instance.UseCompressedTemplates;
    }
  }
}
