// Decompiled with JetBrains decompiler
// Type: Engine.Source.Achievements.AchievementServiceStub
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using System;
using System.Collections.Generic;

#nullable disable
namespace Engine.Source.Achievements
{
  public class AchievementServiceStub : IAchievementServiceImpl
  {
    public IEnumerable<string> Ids => (IEnumerable<string>) Array.Empty<string>();

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
