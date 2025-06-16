using Cofe.Meta;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Audio;

namespace Engine.Source.Services.Consoles.Binds
{
  [Initialisable]
  public static class BindSoundConsoleCommands
  {
    [Cofe.Meta.Initialise]
    private static void Initialise()
    {
      EnumConsoleCommand.AddBind("-mixers", (Func<string>) (() =>
      {
        IEnumerable<AudioMixer> audioMixers = ((IEnumerable<AudioSource>) UnityEngine.Object.FindObjectsOfType<AudioSource>()).Select<AudioSource, AudioMixerGroup>((Func<AudioSource, AudioMixerGroup>) (o => o.outputAudioMixerGroup)).Where<AudioMixerGroup>((Func<AudioMixerGroup, bool>) (o => (UnityEngine.Object) o != (UnityEngine.Object) null)).Select<AudioMixerGroup, AudioMixer>((Func<AudioMixerGroup, AudioMixer>) (o => o.audioMixer)).Where<AudioMixer>((Func<AudioMixer, bool>) (o => (UnityEngine.Object) o != (UnityEngine.Object) null)).Distinct<AudioMixer>();
        string str = "\nMixers :\n";
        foreach (AudioMixer audioMixer in audioMixers)
          str = str + audioMixer.name + "\n";
        return str;
      }));
    }
  }
}
