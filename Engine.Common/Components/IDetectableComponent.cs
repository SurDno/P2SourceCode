namespace Engine.Common.Components
{
  public interface IDetectableComponent : IComponent
  {
    bool IsEnabled { get; set; }
  }
}
