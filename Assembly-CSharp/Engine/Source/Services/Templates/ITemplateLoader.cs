using Engine.Common;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Engine.Source.Services.Templates
{
  public interface ITemplateLoader
  {
    int AsyncCount { get; }

    IEnumerator Load(Dictionary<Guid, IObject> items, Dictionary<Guid, string> names);
  }
}
