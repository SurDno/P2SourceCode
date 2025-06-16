using System;
using System.Diagnostics;
using UnityEngine.SceneManagement;
using Debug = UnityEngine.Debug;

namespace AssetDatabases;

public class AsyncLoadScene : IAsyncLoad {
	private Scene scene;
	private string path;
	private Stopwatch stopwatch;

	public object Asset => scene;

	public bool IsDone { get; private set; }

	public AsyncLoadScene(string path) {
		this.path = path;
		stopwatch = new Stopwatch();
		stopwatch.Restart();
		SceneController.AddHandler(path, OnSceneLoaded);
	}

	private void OnSceneLoaded(Scene scene) {
		this.scene = scene;
		IsDone = true;
		stopwatch.Stop();
		var elapsed = stopwatch.Elapsed;
		Debug.Log(ObjectInfoUtility.GetStream().Append("[Loader]").Append(" Scene loaded, path : ").Append(path)
			.Append(" , elapsed : ").Append(elapsed));
	}
}