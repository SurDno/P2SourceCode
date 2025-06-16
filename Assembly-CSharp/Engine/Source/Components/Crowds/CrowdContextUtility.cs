// Decompiled with JetBrains decompiler
// Type: Engine.Source.Components.Crowds.CrowdContextUtility
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using Engine.Common.Commons.Cloneable;
using Engine.Common.Components.Parameters;
using System.Collections.Generic;
using System.Linq;

#nullable disable
namespace Engine.Source.Components.Crowds
{
  public static class CrowdContextUtility
  {
    public static void Store(
      ParametersComponent parameters,
      List<IParameter> states,
      params ParameterNameEnum[] names)
    {
      if (parameters == null)
        return;
      foreach (ParameterNameEnum name in names)
      {
        IParameter byName = parameters.GetByName(name);
        if (byName != null)
        {
          IParameter parameter = CloneableObjectUtility.Clone<IParameter>(byName);
          states.Add(parameter);
        }
      }
    }

    public static void Restore(
      ParametersComponent parameters,
      List<IParameter> states,
      params ParameterNameEnum[] names)
    {
      if (parameters == null)
        return;
      foreach (IParameter state in states)
      {
        if (((IEnumerable<ParameterNameEnum>) names).Contains<ParameterNameEnum>(state.Name))
        {
          IParameter byName = parameters.GetByName(state.Name);
          if (byName != null && byName.GetType() == state.GetType())
            ((ICopyable) state).CopyTo((object) byName);
        }
      }
    }
  }
}
