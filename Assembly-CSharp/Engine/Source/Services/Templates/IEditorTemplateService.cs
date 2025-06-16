using Engine.Common;
using System;

namespace Engine.Source.Services.Templates
{
  public interface IEditorTemplateService
  {
    IObject GetTemplate(Type type, Guid id);

    T GetTemplate<T>(Guid id) where T : class, IObject;

    T CreateTemplate<T>(string path, Guid id) where T : class, IObject;

    void DeleteTemplate(IObject template);

    void SetDirty(IObject template);

    void SaveTemplates();

    void SaveTemplate(IObject template);

    void Cleanup();
  }
}
