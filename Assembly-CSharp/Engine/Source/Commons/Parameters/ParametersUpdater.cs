using Engine.Common;
using Engine.Source.Services;
using Inspectors;
using System;
using System.Collections.Generic;

namespace Engine.Source.Commons.Parameters
{
  [GameService(new Type[] {typeof (ParametersUpdater)})]
  public class ParametersUpdater : IInitialisable, IUpdatable
  {
    [Inspected]
    private List<IComputeParameter> parameters = new List<IComputeParameter>();
    private List<IComputeParameter> swap = new List<IComputeParameter>();

    public void AddParameter(IComputeParameter parameter) => this.parameters.Add(parameter);

    public void ComputeUpdate()
    {
      if (this.parameters.Count == 0)
        return;
      List<IComputeParameter> swap = this.swap;
      this.swap = this.parameters;
      this.parameters = swap;
      this.parameters.Clear();
      foreach (IComputeParameter computeParameter in this.swap)
      {
        computeParameter.CorrectValue();
        computeParameter.ComputeEvent();
      }
    }

    public void Initialise()
    {
      InstanceByRequest<UpdateService>.Instance.ParametersUpdater.AddUpdatable((IUpdatable) this);
    }

    public void Terminate()
    {
      InstanceByRequest<UpdateService>.Instance.ParametersUpdater.RemoveUpdatable((IUpdatable) this);
      this.parameters.Clear();
      this.swap.Clear();
    }
  }
}
