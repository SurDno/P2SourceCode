namespace PLVirtualMachine.Common
{
  public interface IWorldBlueprint : 
    IBlueprint,
    IGameObjectContext,
    IContainer,
    IObject,
    IEditorBaseTemplate,
    INamedElement,
    INamed,
    IStaticUpdateable,
    IContext,
    ILogicObject,
    IWorldObject,
    IEngineTemplated
  {
  }
}
