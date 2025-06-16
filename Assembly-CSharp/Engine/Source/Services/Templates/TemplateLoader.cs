// Decompiled with JetBrains decompiler
// Type: Engine.Source.Services.Templates.TemplateLoader
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using Engine.Source.Settings.External;

#nullable disable
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
