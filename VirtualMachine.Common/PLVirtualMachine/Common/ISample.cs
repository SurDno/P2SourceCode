namespace PLVirtualMachine.Common
{
  public interface ISample : IObject, IEditorBaseTemplate, IEngineTemplated
  {
    string SampleType { get; }
  }
}
