using System.Reflection;
using Engine.Common.Services;
using Engine.Impl.UI.Menu.Main;
using UnityEngine;

namespace Engine.Source.Services;

[RuntimeService(typeof(LoadGameProgressService), typeof(ILoadProgress))]
public class LoadGameProgressService : ILoadProgress {
	public void BeginLoadGame() {
		Debug.Log(ObjectInfoUtility.GetStream().Append(nameof(LoadGameProgressService)).Append(" : ")
			.Append(MethodBase.GetCurrentMethod().Name));
		LoadWindow.Instance.Progress = 0.0f;
		LoadWindow.Instance.ShowProgress = true;
	}

	public void OnBeforeLoadData() {
		Debug.Log(ObjectInfoUtility.GetStream().Append(nameof(LoadGameProgressService)).Append(" : ")
			.Append(MethodBase.GetCurrentMethod().Name));
		LoadWindow.Instance.Progress = 1f / 43f;
	}

	public void OnAfterLoadData() {
		Debug.Log(ObjectInfoUtility.GetStream().Append(nameof(LoadGameProgressService)).Append(" : ")
			.Append(MethodBase.GetCurrentMethod().Name));
		LoadWindow.Instance.Progress = 0.302325577f;
	}

	public void OnBuildHierarchy() {
		Debug.Log(ObjectInfoUtility.GetStream().Append(nameof(LoadGameProgressService)).Append(" : ")
			.Append(MethodBase.GetCurrentMethod().Name));
		LoadWindow.Instance.Progress = 0.372093022f;
	}

	public void OnLoadDataComplete() {
		Debug.Log(ObjectInfoUtility.GetStream().Append(nameof(LoadGameProgressService)).Append(" : ")
			.Append(MethodBase.GetCurrentMethod().Name));
		LoadWindow.Instance.Progress = 0.372093022f;
	}

	public void OnBeforeCreateHierarchy() {
		Debug.Log(ObjectInfoUtility.GetStream().Append(nameof(LoadGameProgressService)).Append(" : ")
			.Append(MethodBase.GetCurrentMethod().Name));
		LoadWindow.Instance.Progress = 0.372093022f;
	}

	public void OnAfterCreateHierarchy() {
		Debug.Log(ObjectInfoUtility.GetStream().Append(nameof(LoadGameProgressService)).Append(" : ")
			.Append(MethodBase.GetCurrentMethod().Name));
		LoadWindow.Instance.Progress = 0.558139563f;
	}

	public void OnLoadComplete() {
		Debug.Log(ObjectInfoUtility.GetStream().Append(nameof(LoadGameProgressService)).Append(" : ")
			.Append(MethodBase.GetCurrentMethod().Name));
		LoadWindow.Instance.Progress = 0.627907f;
	}

	public void EndLoadGame() {
		Debug.Log(ObjectInfoUtility.GetStream().Append(nameof(LoadGameProgressService)).Append(" : ")
			.Append(MethodBase.GetCurrentMethod().Name));
		LoadWindow.Instance.Progress = 0.6976744f;
	}

	public void PlayerReady() {
		Debug.Log(ObjectInfoUtility.GetStream().Append(nameof(LoadGameProgressService)).Append(" : ")
			.Append(MethodBase.GetCurrentMethod().Name));
		LoadWindow.Instance.Progress = 1f;
	}

	public void LoadGameComplete() {
		Debug.Log(ObjectInfoUtility.GetStream().Append(nameof(LoadGameProgressService)).Append(" : ")
			.Append(MethodBase.GetCurrentMethod().Name));
		LoadWindow.Instance.ShowProgress = false;
		LoadWindow.Instance.Progress = 0.0f;
	}
}