using System;
using System.Collections.Generic;
using System.Linq;
using Engine.Common;
using Engine.Common.Components;
using Engine.Common.Components.Parameters;
using Engine.Common.Components.Storable;
using Engine.Common.Services;
using Engine.Impl.UI.Controls;
using Engine.Source.Components;
using Engine.Source.Components.Utilities;
using Engine.Source.Services.Inputs;
using Engine.Source.UI;
using InputServices;
using Inspectors;
using UnityEngine;
using UnityEngine.UI;

namespace Engine.Impl.UI.Menu.Protagonist.Inventory;

public class CraftMixtureWindow : SelectableInventoryWindow, ICraftMixtureWindow, IWindow {
	[SerializeField] private HideableView craftButtonInteractable;
	[SerializeField] private Button craftButton;
	[SerializeField] private ItemView resultView;
	[SerializeField] private float resultShowTime = 1f;
	[SerializeField] private AudioClip craftAudio;
	[SerializeField] private GameObject craftButtonConsole;
	private Text craftBrewText;
	private bool isCraftAvailable;

	[Inspected] public IStorageComponent Target { get; set; }

	protected override void CraftWindowSubscribe() {
		base.CraftWindowSubscribe();
		ServiceLocator.GetService<GameActionService>().AddListener(GameActionType.Submit, ConsoleController);
	}

	protected override void CraftWindowUnsubscribe() {
		base.CraftWindowUnsubscribe();
		ServiceLocator.GetService<GameActionService>().RemoveListener(GameActionType.Submit, ConsoleController);
	}

	protected override void OnEnable() {
		base.OnEnable();
		craftBrewText = craftButton.GetComponentInChildren<Text>();
		actors.Clear();
		actors.Add(Actor);
		Build2();
		CreateContainers();
	}

	protected override void OnDisable() {
		DisableCraftButtons();
		ServiceLocator.GetService<GameActionService>()
			.RemoveListener(GameActionType.CraftMixture, WithoutJoystickCancelListener);
		CraftWindowUnsubscribe();
		base.OnDisable();
	}

	protected override void OnJoystick(bool joystick) {
		base.OnJoystick(joystick);
		if (!joystick) {
			ServiceLocator.GetService<GameActionService>()
				.AddListener(GameActionType.CraftMixture, WithoutJoystickCancelListener);
			foreach (var ingredientSelector in ingredientSelectors)
				ingredientSelector.SetSelection(false);
			PredictCraftRecipe();
			var componentInChildren1 = ingredientSelectors[0].GetComponentInChildren<Text>();
			componentInChildren1.transform.localPosition =
				new Vector3(componentInChildren1.transform.localPosition.x, 190f);
			var componentInChildren2 = ingredientSelectors[1].GetComponentInChildren<Text>();
			componentInChildren2.transform.localPosition =
				new Vector3(componentInChildren2.transform.localPosition.x, -190f);
		} else {
			ServiceLocator.GetService<GameActionService>()
				.RemoveListener(GameActionType.CraftMixture, WithoutJoystickCancelListener);
			PredictCraftRecipe();
			var componentInChildren3 = ingredientSelectors[0].GetComponentInChildren<Text>();
			componentInChildren3.transform.localPosition =
				new Vector3(componentInChildren3.transform.localPosition.x, 0.0f);
			var componentInChildren4 = ingredientSelectors[1].GetComponentInChildren<Text>();
			componentInChildren4.transform.localPosition =
				new Vector3(componentInChildren4.transform.localPosition.x, 0.0f);
		}

		if (isCraftAvailable)
			craftButtonConsole.SetActive(joystick);
		else
			craftButtonConsole.SetActive(false);
	}

	protected override bool ConsoleController(GameActionType type, bool down) {
		if (!InputService.Instance.JoystickUsed)
			return false;
		if ((type == GameActionType.Submit) & down && craftButton.gameObject.activeInHierarchy) {
			craftButton.onClick.Invoke();
			HideInfoWindow();
			return true;
		}

		base.ConsoleController(type, down);
		return false;
	}

	protected override void OnItemClick(IStorableComponent storable) {
		if (storable.Storage == Actor) {
			foreach (var ingredientSelector in ingredientSelectors)
				if (ingredientSelector.Item == storable) {
					if (ingredientSelector.AvoidNull)
						return;
					ingredientSelector.Item = null;
					return;
				}

			foreach (var ingredientSelector in ingredientSelectors)
				if (ingredientSelector.Item == null)
					foreach (var group in ingredientSelector.Groups)
						if (storable.Groups.Contains(group)) {
							ingredientSelector.Item = storable;
							return;
						}

			foreach (var ingredientSelector in ingredientSelectors) {
				foreach (var group in ingredientSelector.Groups)
					if (storable.Groups.Contains(group)) {
						ingredientSelector.Item = storable;
						return;
					}
			}
		} else {
			var craftTimeParameter = CraftHelper.GetCraftTimeParameter(storable);
			if (craftTimeParameter != null && craftTimeParameter.Value >= TimeSpan.Zero)
				return;
			MoveItem(storable, Actor);
			StorableComponentUtility.PlayTakeSound(storable);
			OnInvalidate();
		}
	}

	protected override void Clear() {
		foreach (var ingredientSelector in ingredientSelectors)
			ingredientSelector.Storage = null;
		base.Clear();
	}

	protected override void OnInvalidate() {
		base.OnInvalidate();
		PredictCraftRecipe();
		if (!InputService.Instance.JoystickUsed)
			return;
		HideInfoWindow();
	}

	private void ClearResultView() {
		resultView.Storable = null;
	}

	private void CreateContainers() {
		foreach (var ingredientSelector in ingredientSelectors)
			ingredientSelector.Storage = Actor;
	}

	private void Craft() {
		DisableCraftButtons();
		var ingredients = new List<IStorableComponent>();
		for (var index = 0; index < ingredientSelectors.Length; ++index) {
			var storableComponent = ingredientSelectors[index].Item;
			if (storableComponent != null)
				ingredients.Add(storableComponent);
		}

		var craftResult = CraftHelper.GetCraftResult(ingredients, out var _);
		if (craftResult != null) {
			foreach (var storableComponent in ingredients)
				if (storableComponent.Count > 1)
					--storableComponent.Count;
				else
					storableComponent.Owner.Dispose();
			var entity = ServiceLocator.GetService<IFactory>().Instantiate(craftResult.Owner);
			ServiceLocator.GetService<ISimulation>().Add(entity, ServiceLocator.GetService<ISimulation>().Storables);
			var component = entity.GetComponent<IStorableComponent>();
			Actor.AddItemOrDrop(component, null);
			CancelInvoke("ClearResultView");
			resultView.Storable = (StorableComponent)component;
			Invoke("ClearResultView", resultShowTime);
			if (craftAudio != null)
				PlayAudio(craftAudio);
		}

		OnInvalidate();
	}

	private void PredictCraftRecipe() {
		DisableCraftButtons();
		craftButtonConsole.SetActive(false);
		var ingredients = new List<IStorableComponent>();
		for (var index = 0; index < ingredientSelectors.Length; ++index) {
			var storableComponent = ingredientSelectors[index].Item;
			if (storableComponent != null)
				ingredients.Add(storableComponent);
		}

		if (CraftHelper.GetCraftResult(ingredients, out var _) != null) {
			isCraftAvailable = true;
			if (InputService.Instance.JoystickUsed)
				craftButtonConsole.SetActive(true);
			EnableCraftButtons();
		} else
			isCraftAvailable = false;
	}

	private void EnableCraftButtons() {
		craftButtonInteractable.Visible = true;
	}

	private void DisableCraftButtons() {
		craftButtonInteractable.Visible = false;
	}

	public override void Initialize() {
		RegisterLayer<ICraftMixtureWindow>(this);
		foreach (var ingredientSelector in ingredientSelectors) {
			ingredientSelector.ChangeItemEvent += OnIngredientChange;
			ingredientSelector.ValidateItemEvent += OnValidateIngredient;
		}

		craftButton.onClick.AddListener(Craft);
		base.Initialize();
	}

	public override Type GetWindowType() {
		return typeof(ICraftMixtureWindow);
	}

	private bool OnValidateIngredient(ItemSelector itemSelector, IStorableComponent ingredient) {
		for (var index = 0; index < ingredientSelectors.Length; ++index) {
			var ingredientSelector = ingredientSelectors[index];
			var storableComponent = ingredientSelector.Item;
			if (itemSelector == ingredientSelector && ingredient == storableComponent)
				return true;
			if (storableComponent != null && StorageUtility.GetItemId(ingredient.Owner) ==
			    StorageUtility.GetItemId(storableComponent.Owner))
				return false;
		}

		return true;
	}

	private void OnIngredientChange(
		ItemSelector itemSelector,
		IStorableComponent prevIngredient,
		IStorableComponent newIngredient) {
		if (prevIngredient != null)
			StorableComponentUtility.PlayPutSound(prevIngredient);
		if (newIngredient != null)
			StorableComponentUtility.PlayTakeSound(newIngredient);
		OnInvalidate();
	}

	protected override bool ItemIsInteresting(IStorableComponent item) {
		var flag1 = false;
		var flag2 = false;
		foreach (var ingredientSelector in ingredientSelectors) {
			if (item == ingredientSelector.Item)
				return !ingredientSelector.AvoidNull;
			foreach (var group in ingredientSelector.Groups)
				if (item.Groups.Contains(group))
					flag1 = true;
			if (ingredientSelector.Item != null && StorageUtility.GetItemId(item.Owner) ==
			    StorageUtility.GetItemId(ingredientSelector.Item.Owner))
				flag2 = true;
		}

		return flag1 && !flag2;
	}

	protected override bool ItemIsSelected(IStorableComponent storable) {
		foreach (var ingredientSelector in ingredientSelectors)
			if (ingredientSelector.Item == storable)
				return true;
		return false;
	}
}