using System.Collections.Generic;
using Engine.Source.VisualEffects;
using Inspectors;

namespace Engine.Source.Services
{
  [RuntimeService(typeof (EffectsService))]
  public class EffectsService
  {
    [Inspected]
    private Dictionary<string, List<IParameter>> parameters = new();

    public void GetParameters<T>(string name, List<IParameter<T>> result) where T : struct
    {
      result.Clear();
      if (!parameters.TryGetValue(name, out List<IParameter> parameterList))
        return;
      foreach (IParameter parameter1 in parameterList)
      {
        if (parameter1 is IParameter<T> parameter2)
          result.Add(parameter2);
      }
    }

    public IEnumerable<IParameter<T>> GetParameters<T>(string name) where T : struct
    {
      if (parameters.TryGetValue(name, out List<IParameter> result))
      {
        foreach (IParameter parameter in result)
        {
          if (parameter is IParameter<T> item)
            yield return item;
          item = null;
        }
      }
    }

    public void AddParameter(string name, IParameter parameter)
    {
      if (!parameters.TryGetValue(name, out List<IParameter> parameterList))
      {
        parameterList = [];
        parameters.Add(name, parameterList);
      }
      parameterList.Add(parameter);
    }

    public void RemoveParameter(string name, IParameter parameter)
    {
      if (!parameters.TryGetValue(name, out List<IParameter> parameterList))
      {
        parameterList = [];
        parameters.Add(name, parameterList);
      }
      parameterList.Remove(parameter);
    }
  }
}
