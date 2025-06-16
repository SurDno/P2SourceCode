// Decompiled with JetBrains decompiler
// Type: Engine.Impl.UI.Menu.Protagonist.Inventory.Container.InventoryContainerUI
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using Engine.Common.Components;
using Engine.Impl.UI.Menu.Protagonist.Inventory.Grid;
using Engine.Source.Inventory;
using Engine.Source.UI.Controls;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

#nullable disable
namespace Engine.Impl.UI.Menu.Protagonist.Inventory.Container
{
  public abstract class InventoryContainerUI : UIControl
  {
    protected Dictionary<Cell, InventoryCellUI> cells = new Dictionary<Cell, InventoryCellUI>();
    [SerializeField]
    [FormerlySerializedAs("_Content")]
    protected UIControl content = (UIControl) null;
    [SerializeField]
    [FormerlySerializedAs("_Grid")]
    protected UIControl grid = (UIControl) null;
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
    protected UIControl storables = (UIControl) null;
    [SerializeField]
    private bool clickEnabled = true;

    public bool ClickEnabled
    {
      get => this.clickEnabled;
      set => this.clickEnabled = value;
    }

    public event Action<InventoryContainerUI> OpenBegin;

    public event Action<InventoryContainerUI, bool> OpenEnd;

    protected void FireOpenBegin()
    {
      Action<InventoryContainerUI> openBegin = this.OpenBegin;
      if (openBegin == null)
        return;
      openBegin(this);
    }

    protected void FireOpenEnd(bool complete)
    {
      Action<InventoryContainerUI, bool> openEnd = this.OpenEnd;
      if (openEnd == null)
        return;
      openEnd(this, complete);
    }

    public IInventoryComponent InventoryContainer { get; protected set; }

    public IDictionary<Cell, InventoryCellUI> Cells
    {
      get => (IDictionary<Cell, InventoryCellUI>) this.cells;
    }

    public UIControl Content => this.content;

    public UIControl Grid => this.grid;

    public UIControl Storables => this.storables;

    public Image ImageBackground => this.imageBackground;

    public Image ImageForeground => this.imageForeground;

    public Image ImageIcon => this.imageIcon;

    public Image ImageDisease => this.imageDisease;

    public Image ImageLock => this.imageLock;

    public HoldableButton2 Button => this.button;

    public void SetIconEnabled(bool b)
    {
      this.imageIcon.color = b ? this.iconAvailiableColor : this.iconNotAvailiableColor;
    }

    public void SetLockEnabled(bool b)
    {
      this.imageLock.color = b ? this.iconAvailiableColor : this.iconNotAvailiableColor;
    }
  }
}
