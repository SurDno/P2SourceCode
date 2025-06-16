// Decompiled with JetBrains decompiler
// Type: Engine.Source.Services.EffectsService
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using Engine.Source.VisualEffects;
using Inspectors;
using System;
using System.Collections.Generic;

#nullable disable
namespace Engine.Source.Services
{
  [RuntimeService(new Type[] {typeof (EffectsService)})]
  public class EffectsService
  {
    [Inspected]
    private Dictionary<string, List<IParameter>> parameters = new Dictionary<string, List<IParameter>>();

    public void GetParameters<T>(string name, List<IParameter<T>> result) where T : struct
    {
      result.Clear();
      List<IParameter> parameterList;
      if (!this.parameters.TryGetValue(name, out parameterList))
        return;
      foreach (IParameter parameter1 in parameterList)
      {
        if (parameter1 is IParameter<T> parameter2)
          result.Add(parameter2);
      }
    }

    public IEnumerable<IParameter<T>> GetParameters<T>(string name) where T : struct
    {
      List<IParameter> result;
      if (this.parameters.TryGetValue(name, out result))
      {
        foreach (IParameter parameter in result)
        {
          if (parameter is IParameter<T> item)
            yield return item;
          item = (IParameter<T>) null;
        }
      }
    }

    public void AddParameter(string name, IParameter parameter)
    {
      List<IParameter> parameterList;
      if (!this.parameters.TryGetValue(name, out parameterList))
      {
        parameterList = new List<IParameter>();
        this.parameters.Add(name, parameterList);
      }
      parameterList.Add(parameter);
    }

    public void RemoveParameter(string name, IParameter parameter)
    {
      List<IParameter> parameterList;
      if (!this.parameters.TryGetValue(name, out parameterList))
      {
        parameterList = new List<IParameter>();
        this.parameters.Add(name, parameterList);
      }
      parameterList.Remove(parameter);
    }
  }
}
