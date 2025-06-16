// Decompiled with JetBrains decompiler
// Type: Engine.Source.Commons.Parameters.ParametersUtility
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using Engine.Common;
using Engine.Common.Components.Parameters;
using Engine.Source.Components;

#nullable disable
namespace Engine.Source.Commons.Parameters
{
  public static class ParametersUtility
  {
    public static IParameter<T> GetParameter<T>(IComponent component, ParameterNameEnum name) where T : struct
    {
      return component.GetComponent<ParametersComponent>()?.GetByName<T>(name);
    }

    public static IParameter<T> GetParameter<T>(IEntity owner, ParameterNameEnum name) where T : struct
    {
      return owner.GetComponent<ParametersComponent>()?.GetByName<T>(name);
    }
  }
}
