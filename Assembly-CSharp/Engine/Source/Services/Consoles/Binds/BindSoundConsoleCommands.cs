using System;
using System.Collections.Generic;
using System.Linq;
using Cofe.Meta;

namespace Engine.Source.Services.Consoles.Binds
{
  [Initialisable]
  public static class BindSoundConsoleCommands
  {
    [Initialise]
    private static void Initialise()
    {
      EnumConsoleCommand.AddBind("-mixers", (Func<string>) (() =>
      {
        IEnumerable<AudioMixer> audioMixers = ((IEnumerable<AudioSource>) UnityEngine.Object.FindObjectsOfType<AudioSource>()).Select((Func<AudioSource, AudioMixerGroup>) (o => o.outputAudioMixerGroup)).Where((Func<AudioMixerGroup, bool>) (o => (UnityEngine.Object) o != (UnityEngine.Object) null)).Select((Func<AudioMixerGroup, AudioMixer>) (o => o.audioMixer)).Where((Func<AudioMixer, bool>) (o => (UnityEngine.Object) o != (UnityEngine.Object) null)).Distinct();
        string str = "\nMixers :\n";
        foreach (AudioMixer audioMixer in audioMixers)
          str = str + audioMixer.name + "\n";
        return str;
      }));
    }
  }
}
