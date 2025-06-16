using System.Collections.Generic;

namespace Engine.Source.Achievements
{
  public interface IAchievementServiceImpl
  {
    IEnumerable<string> Ids { get; }

    void Initialise();

    void Shutdown();

    void Update();

    void Unlock(string id);

    void Reset(string id);

    bool IsUnlocked(string id);
  }
}
