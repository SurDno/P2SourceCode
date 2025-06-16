using System;
using System.Collections.Generic;
using System.Linq;
using Cofe.Meta;
using UnityEngine;
using UnityEngine.Audio;
using Object = UnityEngine.Object;

namespace Engine.Source.Services.Consoles.Binds;

[Initialisable]
public static class BindSoundConsoleCommands {
	[Initialise]
	private static void Initialise() {
		EnumConsoleCommand.AddBind("-mixers", (Func<string>)(() => {
			var audioMixers = Object.FindObjectsOfType<AudioSource>().Select(o => o.outputAudioMixerGroup)
				.Where(o => o != null).Select(o => o.audioMixer).Where(o => o != null).Distinct();
			var str = "\nMixers :\n";
			foreach (var audioMixer in audioMixers)
				str = str + audioMixer.name + "\n";
			return str;
		}));
	}
}