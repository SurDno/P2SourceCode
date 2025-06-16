using System.Collections;
using System.Diagnostics;
using Engine.Impl.UI.Menu.Main;
using UnityEngine;
using UnityEngine.SceneManagement;
using Debug = UnityEngine.Debug;

public class MainSceneLoader : MonoBehaviour
{
  private void Start() => StartCoroutine(LoadMainMenu());

  private IEnumerator LoadMainMenu()
  {
    yield return null;
    string mainName = "Main.unity".Replace(".unity", "");
    LoadWindow.Instance.Progress = 0.0f;
    LoadWindow.Instance.ShowProgress = true;
    yield return null;
    float prev = 0.0f;
    Stopwatch sw = new Stopwatch();
    sw.Restart();
    AsyncOperation asyncOperation = SceneManager.LoadSceneAsync(mainName, LoadSceneMode.Additive);
    while (!asyncOperation.isDone)
    {
      float value = asyncOperation.progress;
      if (prev < (double) value)
      {
        prev = value;
        LoadWindow.Instance.Progress = value * ScriptableObjectInstance<GameSettingsData>.Instance.MaxLoaderProgress;
      }
      yield return null;
    }
    sw.Stop();
    Debug.Log(ObjectInfoUtility.GetStream().Append("Load main scene, elapsed : ").Append(sw.Elapsed));
    Scene mainScene = SceneManager.GetSceneByName(mainName);
    Scene loaderScene = gameObject.scene;
    GameObject[] gameObjectArray = loaderScene.GetRootGameObjects();
    for (int index = 0; index < gameObjectArray.Length; ++index)
    {
      GameObject root = gameObjectArray[index];
      if (!(root.name == "TempLinkPrefabsFromResources"))
      {
        SceneManager.MoveGameObjectToScene(root, mainScene);
        root = null;
      }
    }
    gameObjectArray = null;
    SceneManager.SetActiveScene(mainScene);
    yield return SceneManager.UnloadSceneAsync(loaderScene);
    Destroy(gameObject);
  }
}
