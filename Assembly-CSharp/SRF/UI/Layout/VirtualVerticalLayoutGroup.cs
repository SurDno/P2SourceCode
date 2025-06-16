using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace SRF.UI.Layout
{
  [AddComponentMenu("SRF/UI/Layout/VerticalLayoutGroup (Virtualizing)")]
  public class VirtualVerticalLayoutGroup : LayoutGroup, IPointerClickHandler, IEventSystemHandler
  {
    private readonly SRList<object> _itemList = new SRList<object>();
    private readonly SRList<int> _visibleItemList = new SRList<int>();
    private bool _isDirty = false;
    private SRList<VirtualVerticalLayoutGroup.Row> _rowCache = new SRList<VirtualVerticalLayoutGroup.Row>();
    private ScrollRect _scrollRect;
    private int _selectedIndex;
    private object _selectedItem;
    [SerializeField]
    private VirtualVerticalLayoutGroup.SelectedItemChangedEvent _selectedItemChanged;
    private int _visibleItemCount;
    private SRList<VirtualVerticalLayoutGroup.Row> _visibleRows = new SRList<VirtualVerticalLayoutGroup.Row>();
    public bool EnableSelection = true;
    public RectTransform ItemPrefab;
    public int RowPadding = 2;
    public float Spacing;
    public bool StickToBottom = true;
    private float _itemHeight = -1f;

    public VirtualVerticalLayoutGroup.SelectedItemChangedEvent SelectedItemChanged
    {
      get => this._selectedItemChanged;
      set => this._selectedItemChanged = value;
    }

    public object SelectedItem
    {
      get => this._selectedItem;
      set
      {
        if (this._selectedItem == value || !this.EnableSelection)
          return;
        int num = value == null ? -1 : this._itemList.IndexOf(value);
        if (value != null && num < 0)
          throw new InvalidOperationException("Cannot select item not present in layout");
        if (this._selectedItem != null)
          this.InvalidateItem(this._selectedIndex);
        this._selectedItem = value;
        this._selectedIndex = num;
        if (this._selectedItem != null)
          this.InvalidateItem(this._selectedIndex);
        this.SetDirty();
        if (this._selectedItemChanged == null)
          return;
        this._selectedItemChanged.Invoke(this._selectedItem);
      }
    }

    public override float minHeight
    {
      get
      {
        return (float) ((double) this._itemList.Count * (double) this.ItemHeight + (double) this.padding.top + (double) this.padding.bottom + (double) this.Spacing * (double) this._itemList.Count);
      }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
      if (!this.EnableSelection)
        return;
      GameObject gameObject = eventData.pointerPressRaycast.gameObject;
      if ((UnityEngine.Object) gameObject == (UnityEngine.Object) null)
        return;
      int index = Mathf.FloorToInt(Mathf.Abs(this.rectTransform.InverseTransformPoint(gameObject.transform.position).y) / this.ItemHeight);
      if (index >= 0 && index < this._itemList.Count)
        this.SelectedItem = this._itemList[index];
      else
        this.SelectedItem = (object) null;
    }

    protected override void Awake()
    {
      base.Awake();
      this.ScrollRect.onValueChanged.AddListener(new UnityAction<Vector2>(this.OnScrollRectValueChanged));
      if (!((UnityEngine.Object) this.ItemPrefab.GetComponent(typeof (IVirtualView)) == (UnityEngine.Object) null))
        return;
      Debug.LogWarning((object) "[VirtualVerticalLayoutGroup] ItemPrefab does not have a component inheriting from IVirtualView, so no data binding can occur");
    }

    private void OnScrollRectValueChanged(Vector2 d)
    {
      if ((double) d.y < 0.0 || (double) d.y > 1.0)
        this._scrollRect.verticalNormalizedPosition = Mathf.Clamp01(d.y);
      this.SetDirty();
    }

    protected override void Start()
    {
      base.Start();
      this.ScrollUpdate();
    }

    protected override void OnEnable()
    {
      base.OnEnable();
      this.SetDirty();
    }

    protected void Update()
    {
      if (!this.AlignBottom && !this.AlignTop)
      {
        Debug.LogWarning((object) "[VirtualVerticalLayoutGroup] Only Lower or Upper alignment is supported.", (UnityEngine.Object) this);
        this.childAlignment = TextAnchor.UpperLeft;
      }
      if (this.SelectedItem != null && !this._itemList.Contains(this.SelectedItem))
        this.SelectedItem = (object) null;
      if (!this._isDirty)
        return;
      this._isDirty = false;
      this.ScrollUpdate();
    }

    protected void InvalidateItem(int itemIndex)
    {
      if (!this._visibleItemList.Contains(itemIndex))
        return;
      this._visibleItemList.Remove(itemIndex);
      for (int index = 0; index < this._visibleRows.Count; ++index)
      {
        if (this._visibleRows[index].Index == itemIndex)
        {
          this.RecycleRow(this._visibleRows[index]);
          this._visibleRows.RemoveAt(index);
          break;
        }
      }
    }

    protected void RefreshIndexCache()
    {
      for (int index = 0; index < this._visibleRows.Count; ++index)
        this._visibleRows[index].Index = this._itemList.IndexOf(this._visibleRows[index].Data);
    }

    protected void ScrollUpdate()
    {
      if (!Application.isPlaying)
        return;
      float y = this.rectTransform.anchoredPosition.y;
      float height = ((RectTransform) this.ScrollRect.transform).rect.height;
      int num1 = Mathf.FloorToInt(y / (this.ItemHeight + this.Spacing));
      int num2 = Mathf.CeilToInt((float) (((double) y + (double) height) / ((double) this.ItemHeight + (double) this.Spacing)));
      int b1 = num1 - this.RowPadding;
      int b2 = num2 + this.RowPadding;
      int num3 = Mathf.Max(0, b1);
      int num4 = Mathf.Min(this._itemList.Count, b2);
      bool flag = false;
      for (int index = 0; index < this._visibleRows.Count; ++index)
      {
        VirtualVerticalLayoutGroup.Row visibleRow = this._visibleRows[index];
        if (visibleRow.Index < num3 || visibleRow.Index > num4)
        {
          this._visibleItemList.Remove(visibleRow.Index);
          this._visibleRows.Remove(visibleRow);
          this.RecycleRow(visibleRow);
          flag = true;
        }
      }
      for (int forIndex = num3; forIndex < num4 && forIndex < this._itemList.Count; ++forIndex)
      {
        if (!this._visibleItemList.Contains(forIndex))
        {
          this._visibleRows.Add(this.GetRow(forIndex));
          this._visibleItemList.Add(forIndex);
          flag = true;
        }
      }
      if (flag || this._visibleItemCount != this._visibleRows.Count)
        LayoutRebuilder.MarkLayoutForRebuild(this.rectTransform);
      this._visibleItemCount = this._visibleRows.Count;
    }

    public override void CalculateLayoutInputVertical()
    {
      this.SetLayoutInputForAxis(this.minHeight, this.minHeight, -1f, 1);
    }

    public override void SetLayoutHorizontal()
    {
      float size = this.rectTransform.rect.width - (float) this.padding.left - (float) this.padding.right;
      for (int index = 0; index < this._visibleRows.Count; ++index)
        this.SetChildAlongAxis(this._visibleRows[index].Rect, 0, (float) this.padding.left, size);
      for (int index = 0; index < this._rowCache.Count; ++index)
        this.SetChildAlongAxis(this._rowCache[index].Rect, 0, -size - (float) this.padding.left, size);
    }

    public override void SetLayoutVertical()
    {
      if (!Application.isPlaying)
        return;
      for (int index = 0; index < this._visibleRows.Count; ++index)
      {
        VirtualVerticalLayoutGroup.Row visibleRow = this._visibleRows[index];
        this.SetChildAlongAxis(visibleRow.Rect, 1, (float) ((double) visibleRow.Index * (double) this.ItemHeight + (double) this.padding.top + (double) this.Spacing * (double) visibleRow.Index), this.ItemHeight);
      }
    }

    private new void SetDirty()
    {
      base.SetDirty();
      if (!this.IsActive())
        return;
      this._isDirty = true;
    }

    public void AddItem(object item)
    {
      this._itemList.Add(item);
      this.SetDirty();
      if (!this.StickToBottom || !Mathf.Approximately(this.ScrollRect.verticalNormalizedPosition, 0.0f))
        return;
      this.ScrollRect.normalizedPosition = new Vector2(0.0f, 0.0f);
    }

    public void RemoveItem(object item)
    {
      if (this.SelectedItem == item)
        this.SelectedItem = (object) null;
      this.InvalidateItem(this._itemList.IndexOf(item));
      this._itemList.Remove(item);
      this.RefreshIndexCache();
      this.SetDirty();
    }

    public void ClearItems()
    {
      for (int index = this._visibleRows.Count - 1; index >= 0; --index)
        this.InvalidateItem(this._visibleRows[index].Index);
      this._itemList.Clear();
      this.SetDirty();
    }

    private ScrollRect ScrollRect
    {
      get
      {
        if ((UnityEngine.Object) this._scrollRect == (UnityEngine.Object) null)
          this._scrollRect = this.GetComponentInParent<ScrollRect>();
        return this._scrollRect;
      }
    }

    private bool AlignBottom
    {
      get
      {
        return this.childAlignment == TextAnchor.LowerRight || this.childAlignment == TextAnchor.LowerCenter || this.childAlignment == TextAnchor.LowerLeft;
      }
    }

    private bool AlignTop
    {
      get
      {
        return this.childAlignment == TextAnchor.UpperLeft || this.childAlignment == TextAnchor.UpperCenter || this.childAlignment == TextAnchor.UpperRight;
      }
    }

    private float ItemHeight
    {
      get
      {
        if ((double) this._itemHeight <= 0.0)
        {
          this._itemHeight = !(this.ItemPrefab.GetComponent(typeof (ILayoutElement)) is ILayoutElement component) ? this.ItemPrefab.rect.height : component.preferredHeight;
          if (this._itemHeight.ApproxZero())
          {
            Debug.LogWarning((object) "[VirtualVerticalLayoutGroup] ItemPrefab must have a preferred size greater than 0");
            this._itemHeight = 10f;
          }
        }
        return this._itemHeight;
      }
    }

    private VirtualVerticalLayoutGroup.Row GetRow(int forIndex)
    {
      if (this._rowCache.Count == 0)
      {
        VirtualVerticalLayoutGroup.Row row = this.CreateRow();
        this.PopulateRow(forIndex, row);
        return row;
      }
      object obj = this._itemList[forIndex];
      VirtualVerticalLayoutGroup.Row row1 = (VirtualVerticalLayoutGroup.Row) null;
      VirtualVerticalLayoutGroup.Row row2 = (VirtualVerticalLayoutGroup.Row) null;
      int num = forIndex % 2;
      for (int index = 0; index < this._rowCache.Count; ++index)
      {
        row1 = this._rowCache[index];
        if (row1.Data == obj)
        {
          this._rowCache.RemoveAt(index);
          this.PopulateRow(forIndex, row1);
          break;
        }
        if (row1.Index % 2 == num)
          row2 = row1;
        row1 = (VirtualVerticalLayoutGroup.Row) null;
      }
      if (row1 == null && row2 != null)
      {
        this._rowCache.Remove(row2);
        row1 = row2;
        this.PopulateRow(forIndex, row1);
      }
      else if (row1 == null)
      {
        row1 = this._rowCache.PopLast<VirtualVerticalLayoutGroup.Row>();
        this.PopulateRow(forIndex, row1);
      }
      return row1;
    }

    private void RecycleRow(VirtualVerticalLayoutGroup.Row row) => this._rowCache.Add(row);

    private void PopulateRow(int index, VirtualVerticalLayoutGroup.Row row)
    {
      row.Index = index;
      row.Data = this._itemList[index];
      row.View.SetDataContext(this._itemList[index]);
    }

    private VirtualVerticalLayoutGroup.Row CreateRow()
    {
      VirtualVerticalLayoutGroup.Row row = new VirtualVerticalLayoutGroup.Row();
      RectTransform rectTransform = SRInstantiate.Instantiate<RectTransform>(this.ItemPrefab);
      row.Rect = rectTransform;
      row.View = rectTransform.GetComponent(typeof (IVirtualView)) as IVirtualView;
      rectTransform.SetParent((Transform) this.rectTransform, false);
      return row;
    }

    [Serializable]
    public class SelectedItemChangedEvent : UnityEvent<object>
    {
    }

    [Serializable]
    private class Row
    {
      public object Data;
      public int Index;
      public RectTransform Rect;
      public IVirtualView View;
    }
  }
}
