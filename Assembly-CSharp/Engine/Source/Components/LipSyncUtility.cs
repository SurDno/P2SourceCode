// Decompiled with JetBrains decompiler
// Type: Engine.Source.Components.LipSyncUtility
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using Engine.Common.Commons;
using Engine.Common.Services;
using Engine.Impl.Services;
using Engine.Source.Commons;
using System;
using System.Collections.Generic;
using System.Linq;

#nullable disable
namespace Engine.Source.Components
{
  public static class LipSyncUtility
  {
    public static LipSyncInfo GetLipSyncInfo(ILipSyncObject lipSync)
    {
      LipSyncObject lipSyncObject = (LipSyncObject) lipSync;
      if (lipSyncObject == null)
        return (LipSyncInfo) null;
      LanguageEnum language = ServiceLocator.GetService<LocalizationService>().CurrentLipSyncLanguage;
      LipSyncLanguage lipSyncLanguage = lipSyncObject.Languages.FirstOrDefault<LipSyncLanguage>((Func<LipSyncLanguage, bool>) (o => o.Language == language));
      return lipSyncLanguage == null ? (LipSyncInfo) null : lipSyncLanguage.LipSyncs.Random<LipSyncInfo>();
    }

    public static ILipSyncObject RandomUniform(this IEnumerable<ILipSyncObject> lipSyncs)
    {
      LanguageEnum language = ServiceLocator.GetService<LocalizationService>().CurrentLipSyncLanguage;
      int max = 0;
      foreach (LipSyncObject lipSync in lipSyncs)
      {
        LipSyncLanguage lipSyncLanguage = lipSync.Languages.FirstOrDefault<LipSyncLanguage>((Func<LipSyncLanguage, bool>) (o => o.Language == language));
        max += lipSyncLanguage.LipSyncs.Count;
      }
      if (max == 0)
        return (ILipSyncObject) null;
      int num = UnityEngine.Random.Range(0, max);
      foreach (ILipSyncObject lipSync in lipSyncs)
      {
        LipSyncLanguage lipSyncLanguage = ((LipSyncObject) lipSync).Languages.FirstOrDefault<LipSyncLanguage>((Func<LipSyncLanguage, bool>) (o => o.Language == language));
        max -= lipSyncLanguage.LipSyncs.Count;
        if (max <= num)
          return lipSync;
      }
      throw new Exception("impossible");
    }

    public static LipSyncInfo GetLipSyncInfo(
      ILipSyncObject lipSync,
      int lastIndex,
      out int newIndex)
    {
      LipSyncObject lipSyncObject = (LipSyncObject) lipSync;
      if (lipSyncObject == null)
      {
        newIndex = 0;
        return (LipSyncInfo) null;
      }
      LanguageEnum language = ServiceLocator.GetService<LocalizationService>().CurrentLipSyncLanguage;
      LipSyncLanguage lipSyncLanguage = lipSyncObject.Languages.FirstOrDefault<LipSyncLanguage>((Func<LipSyncLanguage, bool>) (o => o.Language == language));
      if (lipSyncLanguage == null || lipSyncLanguage.LipSyncs.Count == 0)
      {
        newIndex = 0;
        return (LipSyncInfo) null;
      }
      if (lipSyncLanguage.LipSyncs.Count == 1)
      {
        newIndex = 0;
        return lipSyncLanguage.LipSyncs[0];
      }
      int count = lipSyncLanguage.LipSyncs.Count;
      newIndex = (lastIndex + UnityEngine.Random.Range(1, count)) % count;
      return lipSyncLanguage.LipSyncs[newIndex];
    }
  }
}
