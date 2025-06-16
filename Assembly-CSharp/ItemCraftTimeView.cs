using System;
using Engine.Common.Components.Parameters;
using Engine.Common.Services;
using Engine.Impl.Services;
using Engine.Impl.UI.Controls;
using Engine.Source.Components;
using UnityEngine;
using UnityEngine.UI;

public class ItemCraftTimeView : ItemView
{
  [SerializeField]
  private Text text;
  [SerializeField]
  private HideableView busyStatus;
  private StorableComponent storable;
  private int up;

  public bool IsItemReady { get; private set; }

  public event Action OnItemReady;

  public override StorableComponent Storable
  {
    get => storable;
    set
    {
      if (storable == value)
        return;
      storable = value;
      UpdateText();
    }
  }

  private void OnEnable()
  {
    UpdateText();
    up = 0;
  }

  private void Update()
  {
    ++up;
    if (up < 10)
      return;
    up = 0;
    UpdateText();
  }

  private void UpdateText()
  {
    if (text == null)
      return;
    text.text = GetTimerString(storable);
  }

  private string GetTimerString(StorableComponent item)
  {
    string timerString = null;
    if (item != null)
    {
      IParameter<TimeSpan> craftTimeParameter = CraftHelper.GetCraftTimeParameter(item);
      if (craftTimeParameter != null)
      {
        craftTimeParameter.Value = craftTimeParameter.MaxValue - ServiceLocator.GetService<TimeService>().GameTime;
        if (craftTimeParameter.Value > TimeSpan.Zero)
        {
          TimeSpan timeSpan = craftTimeParameter.Value;
          timerString = string.Format("{0:D2}:{1:D2}", timeSpan.Hours, timeSpan.Minutes);
          if (busyStatus != null)
            busyStatus.Visible = true;
          if (IsItemReady)
            IsItemReady = false;
        }
        else
        {
          timerString = string.Empty;
          if (busyStatus != null)
            busyStatus.Visible = false;
          IsItemReady = true;
          Action onItemReady = OnItemReady;
          if (onItemReady != null)
            onItemReady();
        }
      }
    }
    else
    {
      timerString = string.Empty;
      if (busyStatus != null)
        busyStatus.Visible = false;
      IsItemReady = false;
    }
    return timerString;
  }
}
