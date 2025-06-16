// Decompiled with JetBrains decompiler
// Type: Engine.Common.Services.ITemplateService
// Assembly: Engine.Common, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3568A167-18A7-4983-8BC2-C25824901591
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Engine.Common.dll

using System;
using System.Collections.Generic;

#nullable disable
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
