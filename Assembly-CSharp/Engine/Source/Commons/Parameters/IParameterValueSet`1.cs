using Engine.Common.Components.Parameters;

namespace Engine.Source.Commons.Parameters
{
  public interface IParameterValueSet<T> where T : struct
  {
    void Set(IParameter<T> parameter);
  }
}
