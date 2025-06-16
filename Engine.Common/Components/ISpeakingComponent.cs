// Decompiled with JetBrains decompiler
// Type: Engine.Common.Components.ISpeakingComponent
// Assembly: Engine.Common, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3568A167-18A7-4983-8BC2-C25824901591
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Engine.Common.dll

using Engine.Common.Commons;
using Engine.Common.Components.Speaking;
using Engine.Common.Types;
using System;
using System.Collections.Generic;

#nullable disable
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
