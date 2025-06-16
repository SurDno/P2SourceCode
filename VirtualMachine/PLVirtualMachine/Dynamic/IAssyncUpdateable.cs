using System;

namespace PLVirtualMachine.Dynamic
{
  public interface IAssyncUpdateable
  {
    void Update(TimeSpan delta);

    bool Active { get; }

    void Clear();
  }
}
