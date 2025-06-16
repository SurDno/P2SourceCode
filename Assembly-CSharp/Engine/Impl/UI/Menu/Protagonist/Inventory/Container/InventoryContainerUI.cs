using System;
using System.Collections.Generic;
using Engine.Common.Components;
using Engine.Impl.UI.Menu.Protagonist.Inventory.Grid;
using Engine.Source.Inventory;
using Engine.Source.UI.Controls;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace Engine.Impl.UI.Menu.Protagonist.Inventory.Container;

public abstract class InventoryContainerUI : UIControl {
	protected Dictionary<Cell, InventoryCellUI> cells = new();

	[SerializeField] [FormerlySerializedAs("_Content")]
	protected UIControl content;

	[SerializeField] [FormerlySerializedAs("_Grid")]
	protected UIControl grid;

	[SerializeField] [FormerlySerializedAs("_ImageBackground")]
	protected Image imageBackground;

	[SerializeField] protected Image imageForeground;
	[SerializeField] protected Color iconAvailiableColor;
	[SerializeField] protected Color iconNotAvailiableColor;
	[SerializeField] protected Image imageIcon;
	[SerializeField] protected Image imageDisease;
	[SerializeField] protected Image imageLock;
	[SerializeField] protected HoldableButton2 button;

	[SerializeField] [FormerlySerializedAs("_Storables")]
	protected UIControl storables;

	[SerializeField] private bool clickEnabled = true;

	public bool ClickEnabled {
		get => clickEnabled;
		set => clickEnabled = value;
	}

	public event Action<InventoryContainerUI> OpenBegin;

	public event Action<InventoryContainerUI, bool> OpenEnd;

	protected void FireOpenBegin() {
		var openBegin = OpenBegin;
		if (openBegin == null)
			return;
		openBegin(this);
	}

	protected void FireOpenEnd(bool complete) {
		var openEnd = OpenEnd;
		if (openEnd == null)
			return;
		openEnd(this, complete);
	}

	public IInventoryComponent InventoryContainer { get; protected set; }

	public IDictionary<Cell, InventoryCellUI> Cells => cells;

	public UIControl Content => content;

	public UIControl Grid => grid;

	public UIControl Storables => storables;

	public Image ImageBackground => imageBackground;

	public Image ImageForeground => imageForeground;

	public Image ImageIcon => imageIcon;

	public Image ImageDisease => imageDisease;

	public Image ImageLock => imageLock;

	public HoldableButton2 Button => button;

	public void SetIconEnabled(bool b) {
		imageIcon.color = b ? iconAvailiableColor : iconNotAvailiableColor;
	}

	public void SetLockEnabled(bool b) {
		imageLock.color = b ? iconAvailiableColor : iconNotAvailiableColor;
	}
}