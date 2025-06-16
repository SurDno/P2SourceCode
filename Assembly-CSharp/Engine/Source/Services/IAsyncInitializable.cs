using System.Collections;

namespace Engine.Source.Services
{
  public interface IAsyncInitializable
  {
    int AsyncCount { get; }

    IEnumerator AsyncInitialize();
  }
}
