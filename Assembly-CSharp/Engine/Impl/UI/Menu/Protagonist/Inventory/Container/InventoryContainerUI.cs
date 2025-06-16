using System;
using System.Collections.Generic;
using Engine.Common.Components;
using Engine.Impl.UI.Menu.Protagonist.Inventory.Grid;
using Engine.Source.Inventory;
using Engine.Source.UI.Controls;

namespace Engine.Impl.UI.Menu.Protagonist.Inventory.Container
{
  public abstract class InventoryContainerUI : UIControl
  {
    protected Dictionary<Cell, InventoryCellUI> cells = new Dictionary<Cell, InventoryCellUI>();
    [SerializeField]
    [FormerlySerializedAs("_Content")]
    protected UIControl content = null;
    [SerializeField]
    [FormerlySerializedAs("_Grid")]
    protected UIControl grid = null;
    [SerializeField]
    [FormerlySerializedAs("_ImageBackground")]
    protected Image imageBackground = (Image) null;
    [SerializeField]
    protected Image imageForeground = (Image) null;
    [SerializeField]
    protected Color iconAvailiableColor;
    [SerializeField]
    protected Color iconNotAvailiableColor;
    [SerializeField]
    protected Image imageIcon = (Image) null;
    [SerializeField]
    protected Image imageDisease = (Image) null;
    [SerializeField]
    protected Image imageLock = (Image) null;
    [SerializeField]
    protected HoldableButton2 button;
    [SerializeField]
    [FormerlySerializedAs("_Storables")]
    protected UIControl storables = null;
    [SerializeField]
    private bool clickEnabled = true;

    public bool ClickEnabled
    {
      get => clickEnabled;
      set => clickEnabled = value;
    }

    public event Action<InventoryContainerUI> OpenBegin;

    public event Action<InventoryContainerUI, bool> OpenEnd;

    protected void FireOpenBegin()
    {
      Action<InventoryContainerUI> openBegin = OpenBegin;
      if (openBegin == null)
        return;
      openBegin(this);
    }

    protected void FireOpenEnd(bool complete)
    {
      Action<InventoryContainerUI, bool> openEnd = OpenEnd;
      if (openEnd == null)
        return;
      openEnd(this, complete);
    }

    public IInventoryComponent InventoryContainer { get; protected set; }

    public IDictionary<Cell, InventoryCellUI> Cells
    {
      get => cells;
    }

    public UIControl Content => content;

    public UIControl Grid => grid;

    public UIControl Storables => storables;

    public Image ImageBackground => imageBackground;

    public Image ImageForeground => imageForeground;

    public Image ImageIcon => imageIcon;

    public Image ImageDisease => imageDisease;

    public Image ImageLock => imageLock;

    public HoldableButton2 Button => button;

    public void SetIconEnabled(bool b)
    {
      imageIcon.color = b ? iconAvailiableColor : iconNotAvailiableColor;
    }

    public void SetLockEnabled(bool b)
    {
      imageLock.color = b ? iconAvailiableColor : iconNotAvailiableColor;
    }
  }
}
