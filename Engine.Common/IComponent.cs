namespace Engine.Common
{
  public interface IComponent
  {
    IEntity Owner { get; }

    bool IsDisposed { get; }
  }
}
