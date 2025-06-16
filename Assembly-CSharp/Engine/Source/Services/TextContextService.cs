using System.Collections.Generic;
using Cofe.Serializations.Converters;
using Engine.Common.Services;

namespace Engine.Source.Services
{
  [GameService(typeof (ITextContextService), typeof (TextContextService))]
  public class TextContextService : ITextContextService
  {
    private Dictionary<string, string> contexts = new Dictionary<string, string>();

    public void SetInt(string tag, int value)
    {
      contexts[tag] = DefaultConverter.ToString(value);
    }

    public string ComputeText(string source)
    {
      foreach (KeyValuePair<string, string> context in contexts)
        source = source.Replace(context.Key, context.Value);
      return source;
    }
  }
}
