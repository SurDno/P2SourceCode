using Cofe.Serializations.Converters;
using Engine.Common.Services;
using System;
using System.Collections.Generic;

namespace Engine.Source.Services
{
  [GameService(new Type[] {typeof (ITextContextService), typeof (TextContextService)})]
  public class TextContextService : ITextContextService
  {
    private Dictionary<string, string> contexts = new Dictionary<string, string>();

    public void SetInt(string tag, int value)
    {
      this.contexts[tag] = DefaultConverter.ToString(value);
    }

    public string ComputeText(string source)
    {
      foreach (KeyValuePair<string, string> context in this.contexts)
        source = source.Replace(context.Key, context.Value);
      return source;
    }
  }
}
