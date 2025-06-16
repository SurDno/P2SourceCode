using System.Collections.Generic;
using Engine.Common;
using Engine.Common.Commons;
using Engine.Common.Components.Parameters;
using Engine.Common.Generator;
using Engine.Impl.Services.Factories;
using Engine.Source.Commons;
using Engine.Source.Commons.Parameters;
using Inspectors;
using UnityEngine;

namespace Engine.Source.Components;

[Factory]
[GenerateProxy(TypeEnum.Cloneable | TypeEnum.Copyable | TypeEnum.DataRead | TypeEnum.DataWrite | TypeEnum.StateSave |
               TypeEnum.StateLoad)]
public class ParametersComponent : EngineComponent, INeedSave {
	[DataReadProxy]
	[DataWriteProxy]
	[StateSaveProxy(MemberEnum.CustomListParameter)]
	[StateLoadProxy(MemberEnum.CustomListParameter)]
	[CopyableProxy()]
	[Inspected(Mutable = true, Mode = ExecuteMode.Edit)]
	protected List<IParameter> parameters = new();

	private static List<IParameter> stubParameters = new();

	[Inspected] public List<IParameter> Parameters => parameters;

	public bool NeedSave {
		get {
			if (!(Owner.Template is IEntity template)) {
				Debug.LogError("Template not found, owner : " + Owner.GetInfo());
				return true;
			}

			var component = template.GetComponent<ParametersComponent>();
			if (component == null) {
				Debug.LogError(GetType().Name + " not found, owner : " + Owner.GetInfo());
				return true;
			}

			if (component.parameters.Count != parameters.Count) {
				Debug.LogError("Parameters count is not equal, owner : " + Owner.GetInfo());
				return true;
			}

			for (var index = 0; index < parameters.Count; ++index)
				if (parameters[index] is IComputeNeedSave parameter)
					parameter.ComputeNeedSave(component.parameters[index]);
			for (var index = 0; index < parameters.Count; ++index)
				if (!(parameters[index] is INeedSave parameter) || parameter.NeedSave)
					return true;
			return false;
		}
	}

	public IParameter<T> GetByName<T>(ParameterNameEnum name) where T : struct {
		return GetByName(name) as IParameter<T>;
	}

	public IParameter GetByName(ParameterNameEnum name) {
		for (var index = 0; index < parameters.Count; ++index) {
			var parameter = parameters[index];
			if (parameter.Name == name)
				return parameter;
		}

		return null;
	}

	public IParameter<T> GetOrCreateByName<T>(ParameterNameEnum name) where T : struct {
		for (var index = 0; index < parameters.Count; ++index) {
			var parameter = parameters[index];
			if (parameter.Name == name)
				return (IParameter<T>)parameter;
		}

		for (var index = 0; index < stubParameters.Count; ++index) {
			var stubParameter = stubParameters[index];
			if (stubParameter.Name == name)
				return (IParameter<T>)stubParameter;
		}

		var byName = new StubParameter<T>();
		byName.Name = name;
		stubParameters.Add(byName);
		return byName;
	}
}