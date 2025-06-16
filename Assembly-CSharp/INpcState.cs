// Decompiled with JetBrains decompiler
// Type: INpcState
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
public interface INpcState
{
  GameObject GameObject { get; }

  NpcStateStatusEnum Status { get; }

  void Shutdown();

  void OnAnimatorMove();

  void OnAnimatorEventEvent(string obj);

  void Update();

  void OnLodStateChanged(bool enabled);
}
