using Engine.Common.Components.Parameters;
using Inspectors;

namespace Engine.Source.Commons.Parameters
{
  public class StubParameter<T> : IParameter<T>, IParameter, IComputeParameter where T : struct
  {
    private ParameterNameEnum name;

    [Inspected(Header = true)]
    public ParameterNameEnum Name
    {
      get => this.name;
      set => this.name = value;
    }

    [Inspected(Header = true)]
    public T Value
    {
      get => default (T);
      set
      {
      }
    }

    [Inspected]
    public T BaseValue
    {
      get => default (T);
      set
      {
      }
    }

    [Inspected]
    public T MinValue
    {
      get => default (T);
      set
      {
      }
    }

    [Inspected]
    public T MaxValue
    {
      get => default (T);
      set
      {
      }
    }

    [Inspected]
    public bool Resetable => false;

    public object ValueData => (object) this.Value;

    public void AddListener(IChangeParameterListener listener)
    {
    }

    public void RemoveListener(IChangeParameterListener listener)
    {
    }

    void IComputeParameter.ResetResetable()
    {
    }

    void IComputeParameter.CorrectValue()
    {
    }

    void IComputeParameter.ComputeEvent()
    {
    }
  }
}
