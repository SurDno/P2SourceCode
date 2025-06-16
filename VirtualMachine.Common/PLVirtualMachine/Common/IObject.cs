namespace PLVirtualMachine.Common
{
  public interface IObject : IEditorBaseTemplate
  {
    bool IsEqual(IObject other);

    string GuidStr { get; }
  }
}
