// Decompiled with JetBrains decompiler
// Type: PlagueIntroSceneLoader
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using UnityEngine;
using UnityEngine.SceneManagement;

#nullable disable
public class PlagueIntroSceneLoader : MonoBehaviour
{
  private void Start()
  {
    SceneManager.GetSceneByName("PlagueIntro_Riot");
    if (false)
      SceneManager.LoadScene("PlagueIntro_Riot", LoadSceneMode.Additive);
    SceneManager.GetSceneByName("PlagueIntro_FlamethrowerTest");
    if (false)
      SceneManager.LoadScene("PlagueIntro_FlamethrowerTest", LoadSceneMode.Additive);
    SceneManager.GetSceneByName("PlagueIntro_Rats");
    if (false)
      SceneManager.LoadScene("PlagueIntro_Rats", LoadSceneMode.Additive);
    SceneManager.GetSceneByName("PlagueIntro_Flamethrowers");
    if (true)
      return;
    SceneManager.LoadScene("PlagueIntro_Flamethrowers", LoadSceneMode.Additive);
  }
}
