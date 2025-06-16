namespace PLVirtualMachine.Common
{
  public interface ILink : 
    IContainer,
    IObject,
    IEditorBaseTemplate,
    INamedElement,
    INamed,
    IStaticUpdateable
  {
    IGraphObject Source { get; }

    IGraphObject Destination { get; }
  }
}
