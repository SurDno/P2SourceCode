// Decompiled with JetBrains decompiler
// Type: Engine.Source.Achievements.IAchievementServiceImpl
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using System.Collections.Generic;

#nullable disable
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
