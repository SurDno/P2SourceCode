using System.Collections;
using System.Diagnostics;
using Engine.Impl.UI.Menu.Main;
using UnityEngine;
using UnityEngine.SceneManagement;
using Debug = UnityEngine.Debug;

public class MainSceneLoader : MonoBehaviour {
	private void Start() {
		StartCoroutine(LoadMainMenu());
	}

	private IEnumerator LoadMainMenu() {
		yield return null;
		var mainName = "Main.unity".Replace(".unity", "");
		LoadWindow.Instance.Progress = 0.0f;
		LoadWindow.Instance.ShowProgress = true;
		yield return null;
		var prev = 0.0f;
		var sw = new Stopwatch();
		sw.Restart();
		var asyncOperation = SceneManager.LoadSceneAsync(mainName, LoadSceneMode.Additive);
		while (!asyncOperation.isDone) {
			var value = asyncOperation.progress;
			if (prev < (double)value) {
				prev = value;
				LoadWindow.Instance.Progress =
					value * ScriptableObjectInstance<GameSettingsData>.Instance.MaxLoaderProgress;
			}

			yield return null;
		}

		sw.Stop();
		Debug.Log(ObjectInfoUtility.GetStream().Append("Load main scene, elapsed : ").Append(sw.Elapsed));
		var mainScene = SceneManager.GetSceneByName(mainName);
		var loaderScene = gameObject.scene;
		var gameObjectArray = loaderScene.GetRootGameObjects();
		for (var index = 0; index < gameObjectArray.Length; ++index) {
			var root = gameObjectArray[index];
			if (!(root.name == "TempLinkPrefabsFromResources")) {
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