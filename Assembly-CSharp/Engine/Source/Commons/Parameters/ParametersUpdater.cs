// Decompiled with JetBrains decompiler
// Type: Engine.Source.Commons.Parameters.ParametersUpdater
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using Engine.Common;
using Engine.Source.Services;
using Inspectors;
using System;
using System.Collections.Generic;

#nullable disable
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
