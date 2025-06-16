using System;
using Engine.Common.Components;
using UnityEngine;
using UnityEngine.UI;
using Object = UnityEngine.Object;

namespace Engine.Impl.UI.Menu.Protagonist.Inventory.Windows;

[DisallowMultipleComponent]
public class ContextMenuWindow : UIControl {
	[SerializeField] private Button buttonInvestigate;
	[SerializeField] private Button buttonDrop;
	[SerializeField] private Button buttonWear;
	[SerializeField] private Button buttonUse;
	private IStorableComponent target;

	public event Action<IStorableComponent> OnButtonInvestigate;

	public event Action<IStorableComponent> OnButtonDrop;

	public event Action<IStorableComponent> OnButtonWear;

	public event Action<IStorableComponent> OnButtonUse;

	public IStorableComponent Target {
		get => target;
		set {
			if (target == value)
				return;
			target = value;
			buttonUse.gameObject.SetActive(StorableComponentUtility.IsUsable(target));
			buttonWear.gameObject.SetActive(StorableComponentUtility.IsWearable(target));
		}
	}

	public static ContextMenuWindow Instantiate(GameObject prefab) {
		var gameObject = Object.Instantiate(prefab);
		gameObject.name = prefab.name;
		var component = gameObject.GetComponent<ContextMenuWindow>();
		component.buttonInvestigate.onClick.AddListener(component.Investigate);
		component.buttonDrop.onClick.AddListener(component.Drop);
		component.buttonUse.onClick.AddListener(component.Use);
		component.buttonWear.onClick.AddListener(component.Wear);
		return component;
	}

	public void Investigate() {
		if (target == null || OnButtonInvestigate == null)
			return;
		OnButtonInvestigate(target);
	}

	public void Drop() {
		if (target == null || OnButtonDrop == null)
			return;
		OnButtonDrop(target);
	}

	public void Wear() {
		if (target == null || OnButtonWear == null || !StorableComponentUtility.IsWearable(target))
			return;
		OnButtonWear(target);
	}

	public void Use() {
		if (target == null || OnButtonUse == null || !StorableComponentUtility.IsUsable(target))
			return;
		OnButtonUse(target);
	}
}