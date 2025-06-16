using System;
using System.Collections.Generic;
using System.Linq;
using Engine.Common.Commons;
using Engine.Common.Services;
using Engine.Impl.Services;
using Engine.Source.Commons;
using Random = UnityEngine.Random;

namespace Engine.Source.Components
{
  public static class LipSyncUtility
  {
    public static LipSyncInfo GetLipSyncInfo(ILipSyncObject lipSync)
    {
      LipSyncObject lipSyncObject = (LipSyncObject) lipSync;
      if (lipSyncObject == null)
        return null;
      LanguageEnum language = ServiceLocator.GetService<LocalizationService>().CurrentLipSyncLanguage;
      LipSyncLanguage lipSyncLanguage = lipSyncObject.Languages.FirstOrDefault(o => o.Language == language);
      return lipSyncLanguage == null ? null : lipSyncLanguage.LipSyncs.Random();
    }

    public static ILipSyncObject RandomUniform(this IEnumerable<ILipSyncObject> lipSyncs)
    {
      LanguageEnum language = ServiceLocator.GetService<LocalizationService>().CurrentLipSyncLanguage;
      int max = 0;
      foreach (LipSyncObject lipSync in lipSyncs)
      {
        LipSyncLanguage lipSyncLanguage = lipSync.Languages.FirstOrDefault(o => o.Language == language);
        max += lipSyncLanguage.LipSyncs.Count;
      }
      if (max == 0)
        return null;
      int num = Random.Range(0, max);
      foreach (ILipSyncObject lipSync in lipSyncs)
      {
        LipSyncLanguage lipSyncLanguage = ((LipSyncObject) lipSync).Languages.FirstOrDefault(o => o.Language == language);
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
        return null;
      }
      LanguageEnum language = ServiceLocator.GetService<LocalizationService>().CurrentLipSyncLanguage;
      LipSyncLanguage lipSyncLanguage = lipSyncObject.Languages.FirstOrDefault(o => o.Language == language);
      if (lipSyncLanguage == null || lipSyncLanguage.LipSyncs.Count == 0)
      {
        newIndex = 0;
        return null;
      }
      if (lipSyncLanguage.LipSyncs.Count == 1)
      {
        newIndex = 0;
        return lipSyncLanguage.LipSyncs[0];
      }
      int count = lipSyncLanguage.LipSyncs.Count;
      newIndex = (lastIndex + Random.Range(1, count)) % count;
      return lipSyncLanguage.LipSyncs[newIndex];
    }
  }
}
