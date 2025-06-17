using System.Collections.Generic;
using Engine.Common;
using Engine.Source.Services;
using Inspectors;

namespace Engine.Source.Commons.Parameters
{
  [GameService(typeof (ParametersUpdater))]
  public class ParametersUpdater : IInitialisable, IUpdatable
  {
    [Inspected]
    private List<IComputeParameter> parameters = [];
    private List<IComputeParameter> swap = [];

    public void AddParameter(IComputeParameter parameter) => parameters.Add(parameter);

    public void ComputeUpdate()
    {
      if (parameters.Count == 0)
        return;
      List<IComputeParameter> swap = this.swap;
      this.swap = parameters;
      parameters = swap;
      parameters.Clear();
      foreach (IComputeParameter computeParameter in this.swap)
      {
        computeParameter.CorrectValue();
        computeParameter.ComputeEvent();
      }
    }

    public void Initialise()
    {
      InstanceByRequest<UpdateService>.Instance.ParametersUpdater.AddUpdatable(this);
    }

    public void Terminate()
    {
      InstanceByRequest<UpdateService>.Instance.ParametersUpdater.RemoveUpdatable(this);
      parameters.Clear();
      swap.Clear();
    }
  }
}
