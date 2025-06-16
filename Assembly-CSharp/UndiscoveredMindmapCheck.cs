using Engine.Common.Services;
using Engine.Impl.UI.Controls;
using Engine.Source.Services;
using UnityEngine;

public class UndiscoveredMindmapCheck : MonoBehaviour {
	[SerializeField] private HideableView view;
	private MMService service;

	private void OnDisable() {
		if (service == null)
			return;
		service.ChangeUndiscoveredEvent -= UpdateView;
		service = null;
	}

	private void OnEnable() {
		if (view == null)
			return;
		service = ServiceLocator.GetService<MMService>();
		if (service == null)
			return;
		UpdateView();
		view.SkipAnimation();
		service.ChangeUndiscoveredEvent += UpdateView;
	}

	private void UpdateView() {
		view.Visible = service.HasUndiscovered();
	}
}