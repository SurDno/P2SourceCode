using System.Collections;
using System.Diagnostics;
using Engine.Impl.UI.Menu.Main;

public class MainSceneLoader : MonoBehaviour
{
  private void Start() => this.StartCoroutine(LoadMainMenu());

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
    UnityEngine.Debug.Log((object) ObjectInfoUtility.GetStream().Append("Load main scene, elapsed : ").Append(sw.Elapsed));
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
