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
    private readonly SRList<object> _itemList = [];
    private readonly SRList<int> _visibleItemList = [];
    private bool _isDirty;
    private SRList<Row> _rowCache = [];
    private ScrollRect _scrollRect;
    private int _selectedIndex;
    private object _selectedItem;
    [SerializeField]
    private SelectedItemChangedEvent _selectedItemChanged;
    private int _visibleItemCount;
    private SRList<Row> _visibleRows = [];
    public bool EnableSelection = true;
    public RectTransform ItemPrefab;
    public int RowPadding = 2;
    public float Spacing;
    public bool StickToBottom = true;
    private float _itemHeight = -1f;

    public SelectedItemChangedEvent SelectedItemChanged
    {
      get => _selectedItemChanged;
      set => _selectedItemChanged = value;
    }

    public object SelectedItem
    {
      get => _selectedItem;
      set
      {
        if (_selectedItem == value || !EnableSelection)
          return;
        int num = value == null ? -1 : _itemList.IndexOf(value);
        if (value != null && num < 0)
          throw new InvalidOperationException("Cannot select item not present in layout");
        if (_selectedItem != null)
          InvalidateItem(_selectedIndex);
        _selectedItem = value;
        _selectedIndex = num;
        if (_selectedItem != null)
          InvalidateItem(_selectedIndex);
        SetDirty();
        if (_selectedItemChanged == null)
          return;
        _selectedItemChanged.Invoke(_selectedItem);
      }
    }

    public override float minHeight => (float) (_itemList.Count * (double) ItemHeight + padding.top + padding.bottom + Spacing * (double) _itemList.Count);

    public void OnPointerClick(PointerEventData eventData)
    {
      if (!EnableSelection)
        return;
      GameObject gameObject = eventData.pointerPressRaycast.gameObject;
      if (gameObject == null)
        return;
      int index = Mathf.FloorToInt(Mathf.Abs(rectTransform.InverseTransformPoint(gameObject.transform.position).y) / ItemHeight);
      if (index >= 0 && index < _itemList.Count)
        SelectedItem = _itemList[index];
      else
        SelectedItem = null;
    }

    protected override void Awake()
    {
      base.Awake();
      ScrollRect.onValueChanged.AddListener(OnScrollRectValueChanged);
      if (!(ItemPrefab.GetComponent(typeof (IVirtualView)) == null))
        return;
      Debug.LogWarning("[VirtualVerticalLayoutGroup] ItemPrefab does not have a component inheriting from IVirtualView, so no data binding can occur");
    }

    private void OnScrollRectValueChanged(Vector2 d)
    {
      if (d.y < 0.0 || d.y > 1.0)
        _scrollRect.verticalNormalizedPosition = Mathf.Clamp01(d.y);
      SetDirty();
    }

    protected override void Start()
    {
      base.Start();
      ScrollUpdate();
    }

    protected override void OnEnable()
    {
      base.OnEnable();
      SetDirty();
    }

    protected void Update()
    {
      if (!AlignBottom && !AlignTop)
      {
        Debug.LogWarning("[VirtualVerticalLayoutGroup] Only Lower or Upper alignment is supported.", this);
        childAlignment = TextAnchor.UpperLeft;
      }
      if (SelectedItem != null && !_itemList.Contains(SelectedItem))
        SelectedItem = null;
      if (!_isDirty)
        return;
      _isDirty = false;
      ScrollUpdate();
    }

    protected void InvalidateItem(int itemIndex)
    {
      if (!_visibleItemList.Contains(itemIndex))
        return;
      _visibleItemList.Remove(itemIndex);
      for (int index = 0; index < _visibleRows.Count; ++index)
      {
        if (_visibleRows[index].Index == itemIndex)
        {
          RecycleRow(_visibleRows[index]);
          _visibleRows.RemoveAt(index);
          break;
        }
      }
    }

    protected void RefreshIndexCache()
    {
      for (int index = 0; index < _visibleRows.Count; ++index)
        _visibleRows[index].Index = _itemList.IndexOf(_visibleRows[index].Data);
    }

    protected void ScrollUpdate()
    {
      if (!Application.isPlaying)
        return;
      float y = rectTransform.anchoredPosition.y;
      float height = ((RectTransform) ScrollRect.transform).rect.height;
      int num1 = Mathf.FloorToInt(y / (ItemHeight + Spacing));
      int num2 = Mathf.CeilToInt((float) ((y + (double) height) / (ItemHeight + (double) Spacing)));
      int b1 = num1 - RowPadding;
      int b2 = num2 + RowPadding;
      int num3 = Mathf.Max(0, b1);
      int num4 = Mathf.Min(_itemList.Count, b2);
      bool flag = false;
      for (int index = 0; index < _visibleRows.Count; ++index)
      {
        Row visibleRow = _visibleRows[index];
        if (visibleRow.Index < num3 || visibleRow.Index > num4)
        {
          _visibleItemList.Remove(visibleRow.Index);
          _visibleRows.Remove(visibleRow);
          RecycleRow(visibleRow);
          flag = true;
        }
      }
      for (int forIndex = num3; forIndex < num4 && forIndex < _itemList.Count; ++forIndex)
      {
        if (!_visibleItemList.Contains(forIndex))
        {
          _visibleRows.Add(GetRow(forIndex));
          _visibleItemList.Add(forIndex);
          flag = true;
        }
      }
      if (flag || _visibleItemCount != _visibleRows.Count)
        LayoutRebuilder.MarkLayoutForRebuild(rectTransform);
      _visibleItemCount = _visibleRows.Count;
    }

    public override void CalculateLayoutInputVertical()
    {
      SetLayoutInputForAxis(minHeight, minHeight, -1f, 1);
    }

    public override void SetLayoutHorizontal()
    {
      float size = rectTransform.rect.width - padding.left - padding.right;
      for (int index = 0; index < _visibleRows.Count; ++index)
        SetChildAlongAxis(_visibleRows[index].Rect, 0, padding.left, size);
      for (int index = 0; index < _rowCache.Count; ++index)
        SetChildAlongAxis(_rowCache[index].Rect, 0, -size - padding.left, size);
    }

    public override void SetLayoutVertical()
    {
      if (!Application.isPlaying)
        return;
      for (int index = 0; index < _visibleRows.Count; ++index)
      {
        Row visibleRow = _visibleRows[index];
        SetChildAlongAxis(visibleRow.Rect, 1, (float) (visibleRow.Index * (double) ItemHeight + padding.top + Spacing * (double) visibleRow.Index), ItemHeight);
      }
    }

    private new void SetDirty()
    {
      base.SetDirty();
      if (!IsActive())
        return;
      _isDirty = true;
    }

    public void AddItem(object item)
    {
      _itemList.Add(item);
      SetDirty();
      if (!StickToBottom || !Mathf.Approximately(ScrollRect.verticalNormalizedPosition, 0.0f))
        return;
      ScrollRect.normalizedPosition = new Vector2(0.0f, 0.0f);
    }

    public void RemoveItem(object item)
    {
      if (SelectedItem == item)
        SelectedItem = null;
      InvalidateItem(_itemList.IndexOf(item));
      _itemList.Remove(item);
      RefreshIndexCache();
      SetDirty();
    }

    public void ClearItems()
    {
      for (int index = _visibleRows.Count - 1; index >= 0; --index)
        InvalidateItem(_visibleRows[index].Index);
      _itemList.Clear();
      SetDirty();
    }

    private ScrollRect ScrollRect
    {
      get
      {
        if (_scrollRect == null)
          _scrollRect = GetComponentInParent<ScrollRect>();
        return _scrollRect;
      }
    }

    private bool AlignBottom => childAlignment == TextAnchor.LowerRight || childAlignment == TextAnchor.LowerCenter || childAlignment == TextAnchor.LowerLeft;

    private bool AlignTop => childAlignment == TextAnchor.UpperLeft || childAlignment == TextAnchor.UpperCenter || childAlignment == TextAnchor.UpperRight;

    private float ItemHeight
    {
      get
      {
        if (_itemHeight <= 0.0)
        {
          _itemHeight = !(ItemPrefab.GetComponent(typeof (ILayoutElement)) is ILayoutElement component) ? ItemPrefab.rect.height : component.preferredHeight;
          if (_itemHeight.ApproxZero())
          {
            Debug.LogWarning("[VirtualVerticalLayoutGroup] ItemPrefab must have a preferred size greater than 0");
            _itemHeight = 10f;
          }
        }
        return _itemHeight;
      }
    }

    private Row GetRow(int forIndex)
    {
      if (_rowCache.Count == 0)
      {
        Row row = CreateRow();
        PopulateRow(forIndex, row);
        return row;
      }
      object obj = _itemList[forIndex];
      Row row1 = null;
      Row row2 = null;
      int num = forIndex % 2;
      for (int index = 0; index < _rowCache.Count; ++index)
      {
        row1 = _rowCache[index];
        if (row1.Data == obj)
        {
          _rowCache.RemoveAt(index);
          PopulateRow(forIndex, row1);
          break;
        }
        if (row1.Index % 2 == num)
          row2 = row1;
        row1 = null;
      }
      if (row1 == null && row2 != null)
      {
        _rowCache.Remove(row2);
        row1 = row2;
        PopulateRow(forIndex, row1);
      }
      else if (row1 == null)
      {
        row1 = _rowCache.PopLast();
        PopulateRow(forIndex, row1);
      }
      return row1;
    }

    private void RecycleRow(Row row) => _rowCache.Add(row);

    private void PopulateRow(int index, Row row)
    {
      row.Index = index;
      row.Data = _itemList[index];
      row.View.SetDataContext(_itemList[index]);
    }

    private Row CreateRow()
    {
      Row row = new Row();
      RectTransform rectTransform = SRInstantiate.Instantiate(ItemPrefab);
      row.Rect = rectTransform;
      row.View = rectTransform.GetComponent(typeof (IVirtualView)) as IVirtualView;
      rectTransform.SetParent(this.rectTransform, false);
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
