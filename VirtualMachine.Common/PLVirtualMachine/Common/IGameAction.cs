namespace PLVirtualMachine.Common
{
  public interface IGameAction : 
    IOrderedChild,
    IContextElement,
    IObject,
    IEditorBaseTemplate,
    INamed,
    IBaseAction,
    IStaticUpdateable
  {
  }
}
