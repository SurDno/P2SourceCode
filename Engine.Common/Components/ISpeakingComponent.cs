using System;
using System.Collections.Generic;
using Engine.Common.Commons;
using Engine.Common.Components.Speaking;
using Engine.Common.Types;

namespace Engine.Common.Components
{
  public interface ISpeakingComponent : IComponent
  {
    bool IsEnabled { get; }

    bool SpeakAvailable { get; set; }

    IEnumerable<ILipSyncObject> InitialPhrases { get; }

    event Action OnBeginTalking;

    event Action<ulong> OnSpeechReply;

    void Speech(LocalizedText speech, List<DialogString> replies);

    void ExitTalking();

    void AddInitialPhrase(ILipSyncObject lipsync);

    void RemoveInitialPhrase(ILipSyncObject lipsync);

    void ClearInitialPhrases();
  }
}
