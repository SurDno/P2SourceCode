// Decompiled with JetBrains decompiler
// Type: MainSceneLoader
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using Engine.Impl.UI.Menu.Main;
using System.Collections;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.SceneManagement;

#nullable disable
public class MainSceneLoader : MonoBehaviour
{
  private void Start() => this.StartCoroutine(this.LoadMainMenu());

  private IEnumerator LoadMainMenu()
  {
    yield return (object) null;
    string mainName = "Main.unity".Replace(".unity", "");
    LoadWindow.Instance.Progress = 0.0f;
    LoadWindow.Instance.ShowProgress = true;
    yield return (object) null;
    float prev = 0.0f;
    Stopwatch sw = new Stopwatch();
    sw.Restart();
    AsyncOperation asyncOperation = SceneManager.LoadSceneAsync(mainName, LoadSceneMode.Additive);
    while (!asyncOperation.isDone)
    {
      float value = asyncOperation.progress;
      if ((double) prev < (double) value)
      {
        prev = value;
        LoadWindow.Instance.Progress = value * ScriptableObjectInstance<GameSettingsData>.Instance.MaxLoaderProgress;
      }
      yield return (object) null;
    }
    sw.Stop();
    UnityEngine.Debug.Log((object) ObjectInfoUtility.GetStream().Append("Load main scene, elapsed : ").Append((object) sw.Elapsed));
    Scene mainScene = SceneManager.GetSceneByName(mainName);
    Scene loaderScene = this.gameObject.scene;
    GameObject[] gameObjectArray = loaderScene.GetRootGameObjects();
    for (int index = 0; index < gameObjectArray.Length; ++index)
    {
      GameObject root = gameObjectArray[index];
      if (!(root.name == "TempLinkPrefabsFromResources"))
      {
        SceneManager.MoveGameObjectToScene(root, mainScene);
        root = (GameObject) null;
      }
    }
    gameObjectArray = (GameObject[]) null;
    SceneManager.SetActiveScene(mainScene);
    yield return (object) SceneManager.UnloadSceneAsync(loaderScene);
    Object.Destroy((Object) this.gameObject);
  }
}
