using System.Collections.Generic;
using Engine.Common;
using Engine.Source.Services;
using Inspectors;

namespace Engine.Source.Commons.Parameters;

[GameService(typeof(ParametersUpdater))]
public class ParametersUpdater : IInitialisable, IUpdatable {
	[Inspected] private List<IComputeParameter> parameters = new();
	private List<IComputeParameter> swap = new();

	public void AddParameter(IComputeParameter parameter) {
		parameters.Add(parameter);
	}

	public void ComputeUpdate() {
		if (parameters.Count == 0)
			return;
		var swap = this.swap;
		this.swap = parameters;
		parameters = swap;
		parameters.Clear();
		foreach (var computeParameter in this.swap) {
			computeParameter.CorrectValue();
			computeParameter.ComputeEvent();
		}
	}

	public void Initialise() {
		InstanceByRequest<UpdateService>.Instance.ParametersUpdater.AddUpdatable(this);
	}

	public void Terminate() {
		InstanceByRequest<UpdateService>.Instance.ParametersUpdater.RemoveUpdatable(this);
		parameters.Clear();
		swap.Clear();
	}
}