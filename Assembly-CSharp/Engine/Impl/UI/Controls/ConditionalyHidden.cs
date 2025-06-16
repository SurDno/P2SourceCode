using System;
using UnityEngine;

namespace Engine.Impl.UI.Controls;

public class ConditionalyHidden : MonoBehaviour {
	[SerializeField] private HideableView hideableView;
	[SerializeField] private bool defaultVisibility;
	[SerializeField] private Item[] items = new Item[0];

	private void Start() {
		UpdateVisibility();
		foreach (var obj in items) {
			var hideableView = obj.GetHideableView();
			if (hideableView != null) {
				hideableView.OnChangeEvent += UpdateVisibility;
				hideableView.OnSkipAnimationEvent += SkipAnimation;
			}
		}
	}

	private void OnDestroy() {
		foreach (var obj in items) {
			var hideableView = obj.GetHideableView();
			if (hideableView != null) {
				hideableView.OnChangeEvent -= UpdateVisibility;
				hideableView.OnSkipAnimationEvent -= SkipAnimation;
			}
		}
	}

	private void SkipAnimation() {
		if (!(hideableView != null))
			return;
		hideableView.SkipAnimation();
	}

	private void UpdateVisibility() {
		var defaultVisibility = this.defaultVisibility;
		for (var index = 0; index < items.Length; ++index) {
			var hideableView = items[index].GetHideableView();
			if (!(hideableView == null)) {
				var flag = hideableView.Visible;
				if (items[index].Negated)
					flag = !flag;
				switch (items[index].Operation) {
					case Item.OperationEnum.Or:
						defaultVisibility |= flag;
						break;
					case Item.OperationEnum.And:
						defaultVisibility &= flag;
						break;
					case Item.OperationEnum.ExclusiveOr:
						defaultVisibility ^= flag;
						break;
				}
			}
		}

		if (!(this.hideableView != null))
			return;
		this.hideableView.Visible = defaultVisibility;
	}

	[Serializable]
	public struct Item {
		public GameObject Reference;
		public OperationEnum Operation;
		public bool Negated;

		public HideableView GetHideableView() {
			return Reference?.GetComponent<HideableView>();
		}

		public enum OperationEnum {
			Or,
			And,
			ExclusiveOr
		}
	}
}