// Decompiled with JetBrains decompiler
// Type: ItemCraftTimeView
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using Engine.Common.Components;
using Engine.Common.Components.Parameters;
using Engine.Common.Services;
using Engine.Impl.Services;
using Engine.Impl.UI.Controls;
using Engine.Source.Components;
using System;
using UnityEngine;
using UnityEngine.UI;

#nullable disable
public class ItemCraftTimeView : ItemView
{
  [SerializeField]
  private Text text;
  [SerializeField]
  private HideableView busyStatus;
  private StorableComponent storable;
  private int up = 0;

  public bool IsItemReady { get; private set; } = false;

  public event Action OnItemReady;

  public override StorableComponent Storable
  {
    get => this.storable;
    set
    {
      if (this.storable == value)
        return;
      this.storable = value;
      this.UpdateText();
    }
  }

  private void OnEnable()
  {
    this.UpdateText();
    this.up = 0;
  }

  private void Update()
  {
    ++this.up;
    if (this.up < 10)
      return;
    this.up = 0;
    this.UpdateText();
  }

  private void UpdateText()
  {
    if ((UnityEngine.Object) this.text == (UnityEngine.Object) null)
      return;
    this.text.text = this.GetTimerString(this.storable);
  }

  private string GetTimerString(StorableComponent item)
  {
    string timerString = (string) null;
    if (item != null)
    {
      IParameter<TimeSpan> craftTimeParameter = CraftHelper.GetCraftTimeParameter((IStorableComponent) item);
      if (craftTimeParameter != null)
      {
        craftTimeParameter.Value = craftTimeParameter.MaxValue - ServiceLocator.GetService<TimeService>().GameTime;
        if (craftTimeParameter.Value > TimeSpan.Zero)
        {
          TimeSpan timeSpan = craftTimeParameter.Value;
          timerString = string.Format("{0:D2}:{1:D2}", (object) timeSpan.Hours, (object) timeSpan.Minutes);
          if ((UnityEngine.Object) this.busyStatus != (UnityEngine.Object) null)
            this.busyStatus.Visible = true;
          if (this.IsItemReady)
            this.IsItemReady = false;
        }
        else
        {
          timerString = string.Empty;
          if ((UnityEngine.Object) this.busyStatus != (UnityEngine.Object) null)
            this.busyStatus.Visible = false;
          this.IsItemReady = true;
          Action onItemReady = this.OnItemReady;
          if (onItemReady != null)
            onItemReady();
        }
      }
    }
    else
    {
      timerString = string.Empty;
      if ((UnityEngine.Object) this.busyStatus != (UnityEngine.Object) null)
        this.busyStatus.Visible = false;
      this.IsItemReady = false;
    }
    return timerString;
  }
}
