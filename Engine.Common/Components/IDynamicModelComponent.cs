namespace Engine.Common.Components
{
  public interface IDynamicModelComponent : IComponent
  {
    IModel Model { get; set; }
  }
}
