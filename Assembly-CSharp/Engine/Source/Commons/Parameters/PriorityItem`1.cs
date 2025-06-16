using Engine.Common.Components.Parameters;
using Engine.Common.Generator;
using Inspectors;

namespace Engine.Source.Commons.Parameters
{
  public class PriorityItem<T>
  {
    [DataReadProxy]
    [DataWriteProxy]
    [StateSaveProxy]
    [StateLoadProxy]
    [CopyableProxy]
    [Inspected]
    [Inspected(Mutable = true, Mode = ExecuteMode.Edit)]
    public PriorityParameterEnum Priority;
    [DataReadProxy]
    [DataWriteProxy]
    [StateSaveProxy]
    [StateLoadProxy]
    [CopyableProxy()]
    [Inspected]
    [Inspected(Mutable = true, Mode = ExecuteMode.Edit)]
    public T Value;
  }
}
