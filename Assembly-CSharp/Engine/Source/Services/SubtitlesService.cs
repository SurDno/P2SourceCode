using System;
using Engine.Common;
using Engine.Source.Audio;

namespace Engine.Source.Services
{
  [RuntimeService(typeof (SubtitlesService))]
  public class SubtitlesService
  {
    public Action<IEntity, string, AudioState, UnityEngine.Object> AddSubtitlesEvent;
    public Action<IEntity> RemoveSubtitlesEvent;

    public bool SubtitlesEnabled { get; set; }

    public bool DialogSubtitlesEnabled { get; set; }

    public void AddSubtitles(IEntity actor, string tag, AudioState state, UnityEngine.Object context)
    {
      Action<IEntity, string, AudioState, UnityEngine.Object> addSubtitlesEvent = AddSubtitlesEvent;
      if (addSubtitlesEvent == null)
        return;
      addSubtitlesEvent(actor, tag, state, context);
    }

    public void RemoveSubtitles(IEntity actor)
    {
      Action<IEntity> removeSubtitlesEvent = RemoveSubtitlesEvent;
      if (removeSubtitlesEvent == null)
        return;
      removeSubtitlesEvent(actor);
    }
  }
}
