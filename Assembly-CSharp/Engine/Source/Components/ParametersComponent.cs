// Decompiled with JetBrains decompiler
// Type: Engine.Source.Components.ParametersComponent
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using Engine.Common;
using Engine.Common.Commons;
using Engine.Common.Components.Parameters;
using Engine.Common.Generator;
using Engine.Impl.Services.Factories;
using Engine.Source.Commons;
using Engine.Source.Commons.Parameters;
using Inspectors;
using System.Collections.Generic;
using UnityEngine;

#nullable disable
namespace Engine.Source.Components
{
  [Factory]
  [GenerateProxy(TypeEnum.Cloneable | TypeEnum.Copyable | TypeEnum.DataRead | TypeEnum.DataWrite | TypeEnum.StateSave | TypeEnum.StateLoad)]
  public class ParametersComponent : EngineComponent, INeedSave
  {
    [DataReadProxy(MemberEnum.None)]
    [DataWriteProxy(MemberEnum.None)]
    [StateSaveProxy(MemberEnum.CustomListParameter)]
    [StateLoadProxy(MemberEnum.CustomListParameter)]
    [CopyableProxy(MemberEnum.None)]
    [Inspected(Mutable = true, Mode = ExecuteMode.Edit)]
    protected List<IParameter> parameters = new List<IParameter>();
    private static List<IParameter> stubParameters = new List<IParameter>();

    [Inspected]
    public List<IParameter> Parameters => this.parameters;

    public bool NeedSave
    {
      get
      {
        if (!(this.Owner.Template is IEntity template))
        {
          Debug.LogError((object) ("Template not found, owner : " + this.Owner.GetInfo()));
          return true;
        }
        ParametersComponent component = template.GetComponent<ParametersComponent>();
        if (component == null)
        {
          Debug.LogError((object) (this.GetType().Name + " not found, owner : " + this.Owner.GetInfo()));
          return true;
        }
        if (component.parameters.Count != this.parameters.Count)
        {
          Debug.LogError((object) ("Parameters count is not equal, owner : " + this.Owner.GetInfo()));
          return true;
        }
        for (int index = 0; index < this.parameters.Count; ++index)
        {
          if (this.parameters[index] is IComputeNeedSave parameter)
            parameter.ComputeNeedSave((object) component.parameters[index]);
        }
        for (int index = 0; index < this.parameters.Count; ++index)
        {
          if (!(this.parameters[index] is INeedSave parameter) || parameter.NeedSave)
            return true;
        }
        return false;
      }
    }

    public IParameter<T> GetByName<T>(ParameterNameEnum name) where T : struct
    {
      return this.GetByName(name) as IParameter<T>;
    }

    public IParameter GetByName(ParameterNameEnum name)
    {
      for (int index = 0; index < this.parameters.Count; ++index)
      {
        IParameter parameter = this.parameters[index];
        if (parameter.Name == name)
          return parameter;
      }
      return (IParameter) null;
    }

    public IParameter<T> GetOrCreateByName<T>(ParameterNameEnum name) where T : struct
    {
      for (int index = 0; index < this.parameters.Count; ++index)
      {
        IParameter parameter = this.parameters[index];
        if (parameter.Name == name)
          return (IParameter<T>) parameter;
      }
      for (int index = 0; index < ParametersComponent.stubParameters.Count; ++index)
      {
        IParameter stubParameter = ParametersComponent.stubParameters[index];
        if (stubParameter.Name == name)
          return (IParameter<T>) stubParameter;
      }
      StubParameter<T> byName = new StubParameter<T>();
      byName.Name = name;
      ParametersComponent.stubParameters.Add((IParameter) byName);
      return (IParameter<T>) byName;
    }
  }
}
