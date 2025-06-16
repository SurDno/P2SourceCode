using System;
using System.Collections.Generic;

namespace Engine.Source.Achievements
{
  public class AchievementServiceStub : IAchievementServiceImpl
  {
    public IEnumerable<string> Ids => Array.Empty<string>();

    public void Initialise()
    {
    }

    public void Shutdown()
    {
    }

    public void Update()
    {
    }

    public bool IsUnlocked(string id) => true;

    public void Unlock(string id)
    {
    }

    public void Reset(string id)
    {
    }
  }
}
