// Decompiled with JetBrains decompiler
// Type: Engine.Impl.UI.Menu.Protagonist.Inventory.Windows.InfoGraphic
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using Engine.Common.Components;
using Engine.Common.Services;
using Engine.Impl.Services;
using Engine.Source.Components;
using System;
using System.Globalization;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

#nullable disable
namespace Engine.Impl.UI.Menu.Protagonist.Inventory.Windows
{
  [DisallowMultipleComponent]
  public class InfoGraphic : UIControl
  {
    private float price = float.NaN;
    private IStorableComponent target;
    [SerializeField]
    [FormerlySerializedAs("_Unity_Background_Default")]
    private Image unityBackgroundDefault = (Image) null;
    [SerializeField]
    [FormerlySerializedAs("_Unity_Background_Price")]
    private Image unityBackgroundPrice = (Image) null;
    [SerializeField]
    [FormerlySerializedAs("_Unity_Image")]
    private Image unityImage = (Image) null;
    [SerializeField]
    [FormerlySerializedAs("_Unity_Information")]
    private Text unityInformation = (Text) null;
    [SerializeField]
    [FormerlySerializedAs("_Unity_Name")]
    private Text unityName = (Text) null;
    [SerializeField]
    [FormerlySerializedAs("_Unity_Price")]
    private Text unityPrice = (Text) null;

    public IStorableComponent Target
    {
      get => this.target;
      set
      {
        if (this.target == value)
          return;
        this.target = value;
        if (this.target == null || this.target.IsDisposed)
          return;
        LocalizationService service = ServiceLocator.GetService<LocalizationService>();
        this.unityName.text = service.GetText(this.target.Title);
        this.unityInformation.text = service.GetText(this.target.Tooltip);
        this.unityImage.sprite = ((StorableComponent) this.target).Placeholder.ImageInformation.Value;
        if ((UnityEngine.Object) this.unityImage.sprite == (UnityEngine.Object) null)
          this.unityImage.color = Color.black;
        else
          this.unityImage.color = Color.white;
      }
    }

    public float Price
    {
      get => this.price;
      set
      {
        this.price = (float) Math.Round((double) value);
        if (!((UnityEngine.Object) this.unityPrice != (UnityEngine.Object) null))
          return;
        this.unityPrice.text = value.ToString((IFormatProvider) CultureInfo.InvariantCulture);
      }
    }

    public static InfoGraphic Instantiate(bool withPrice, GameObject prefab)
    {
      GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(prefab);
      gameObject.name = prefab.name;
      InfoGraphic component = gameObject.GetComponent<InfoGraphic>();
      if (withPrice)
      {
        component.unityBackgroundPrice.gameObject.SetActive(true);
        component.unityBackgroundDefault.gameObject.SetActive(false);
        if ((UnityEngine.Object) component.unityPrice != (UnityEngine.Object) null)
          component.unityPrice.gameObject.SetActive(true);
      }
      else
      {
        component.unityBackgroundPrice.gameObject.SetActive(false);
        component.unityBackgroundDefault.gameObject.SetActive(true);
        if ((UnityEngine.Object) component.unityPrice != (UnityEngine.Object) null)
          component.unityPrice.gameObject.SetActive(false);
      }
      return component;
    }
  }
}
