using Engine.Common;

namespace Engine.Source.Commons
{
  public interface IEntityAttachable
  {
    void Attach(IEntity owner);

    void Detach();
  }
}
