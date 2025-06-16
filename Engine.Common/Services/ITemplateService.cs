using System;
using System.Collections.Generic;

namespace Engine.Common.Services
{
  public interface ITemplateService
  {
    IObject GetTemplate(Type type, Guid id);

    T GetTemplate<T>(Guid id) where T : class, IObject;

    IEnumerable<IObject> GetTemplates(Type type);

    IEnumerable<T> GetTemplates<T>() where T : class, IObject;
  }
}
