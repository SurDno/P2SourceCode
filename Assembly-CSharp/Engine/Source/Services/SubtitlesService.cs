// Decompiled with JetBrains decompiler
// Type: Engine.Source.Services.SubtitlesService
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using Engine.Common;
using Engine.Source.Audio;
using System;

#nullable disable
namespace Engine.Source.Services
{
  [RuntimeService(new System.Type[] {typeof (SubtitlesService)})]
  public class SubtitlesService
  {
    public Action<IEntity, string, AudioState, UnityEngine.Object> AddSubtitlesEvent;
    public Action<IEntity> RemoveSubtitlesEvent;

    public bool SubtitlesEnabled { get; set; }

    public bool DialogSubtitlesEnabled { get; set; }

    public void AddSubtitles(IEntity actor, string tag, AudioState state, UnityEngine.Object context)
    {
      Action<IEntity, string, AudioState, UnityEngine.Object> addSubtitlesEvent = this.AddSubtitlesEvent;
      if (addSubtitlesEvent == null)
        return;
      addSubtitlesEvent(actor, tag, state, context);
    }

    public void RemoveSubtitles(IEntity actor)
    {
      Action<IEntity> removeSubtitlesEvent = this.RemoveSubtitlesEvent;
      if (removeSubtitlesEvent == null)
        return;
      removeSubtitlesEvent(actor);
    }
  }
}
