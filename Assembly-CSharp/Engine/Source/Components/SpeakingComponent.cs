using System;
using System.Collections.Generic;
using Engine.Common;
using Engine.Common.Commons;
using Engine.Common.Components;
using Engine.Common.Components.Speaking;
using Engine.Common.Generator;
using Engine.Common.Types;
using Engine.Impl.Services.Factories;
using Engine.Source.Commons;
using Inspectors;

namespace Engine.Source.Components;

[Required(typeof(LipSyncComponent))]
[Factory(typeof(ISpeakingComponent))]
[GenerateProxy(TypeEnum.Cloneable | TypeEnum.Copyable | TypeEnum.DataRead | TypeEnum.DataWrite | TypeEnum.StateSave |
               TypeEnum.StateLoad)]
public class SpeakingComponent : EngineComponent, ISpeakingComponent, IComponent, INeedSave {
	[StateSaveProxy]
	[StateLoadProxy]
	[DataReadProxy]
	[DataWriteProxy]
	[CopyableProxy()]
	[Inspected(Mutable = true, Mode = ExecuteMode.Edit)]
	protected bool isEnabled = true;

	[StateSaveProxy(MemberEnum.CustomListReference)] [StateLoadProxy(MemberEnum.CustomListReference)] [Inspected]
	protected List<ILipSyncObject> initialPhrases = new();

	private bool speakAvailable;

	public event Action OnBeginTalking;

	public event Action<ulong> OnSpeechReply;

	public event Action<LocalizedText, List<DialogString>> OnBeginSpeech;

	public event Action OnExitTalking;

	public event Action<bool> OnSpeakAvailableChange;

	[Inspected(Mutable = true)]
	public bool IsEnabled {
		get => isEnabled;
		set => isEnabled = value;
	}

	[StateSaveProxy]
	[StateLoadProxy]
	[Inspected]
	public bool SpeakAvailable {
		get => speakAvailable;
		set {
			if (speakAvailable == value)
				return;
			speakAvailable = value;
			var speakAvailableChange = OnSpeakAvailableChange;
			if (speakAvailableChange == null)
				return;
			speakAvailableChange(value);
		}
	}

	public IEnumerable<ILipSyncObject> InitialPhrases => initialPhrases;

	public void AddInitialPhrase(ILipSyncObject lipsync) {
		initialPhrases.Add(lipsync);
	}

	public void RemoveInitialPhrase(ILipSyncObject lipsync) {
		initialPhrases.Remove(lipsync);
	}

	public void ClearInitialPhrases() {
		initialPhrases.Clear();
	}

	public bool NeedSave => true;

	public void FireBeginTalking() {
		var onBeginTalking = OnBeginTalking;
		if (onBeginTalking == null)
			return;
		onBeginTalking();
	}

	public void FireSpeechReply(ulong reply) {
		var onSpeechReply = OnSpeechReply;
		if (onSpeechReply == null)
			return;
		onSpeechReply(reply);
	}

	public void Speech(LocalizedText speech, List<DialogString> replies) {
		var onBeginSpeech = OnBeginSpeech;
		if (onBeginSpeech == null)
			return;
		onBeginSpeech(speech, replies);
	}

	public void ExitTalking() {
		var onExitTalking = OnExitTalking;
		if (onExitTalking == null)
			return;
		onExitTalking();
	}
}