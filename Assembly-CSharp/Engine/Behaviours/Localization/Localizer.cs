using Engine.Common.Services;
using Engine.Impl.Services;
using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace Engine.Behaviours.Localization
{
  [DisallowMultipleComponent]
  public class Localizer : UIBehaviour
  {
    [SerializeField]
    [FormerlySerializedAs("_Signature")]
    private string signature;
    [SerializeField]
    [FormerlySerializedAs("convertToUpper")]
    private bool smallCaps = true;
    [SerializeField]
    private bool allCaps = false;
    private Text textField;

    public string Signature
    {
      get => this.signature;
      set
      {
        this.signature = value;
        if (!this.isActiveAndEnabled)
          return;
        this.Build();
      }
    }

    protected override void OnEnable()
    {
      base.OnEnable();
      this.Build();
      ServiceLocator.GetService<LocalizationService>().LocalizationChanged -= new Action(this.OnLocalizationChanged);
      ServiceLocator.GetService<LocalizationService>().LocalizationChanged += new Action(this.OnLocalizationChanged);
    }

    protected override void OnDisable()
    {
      base.OnDisable();
      ServiceLocator.GetService<LocalizationService>().LocalizationChanged -= new Action(this.OnLocalizationChanged);
    }

    public void Build()
    {
      if ((UnityEngine.Object) this.textField == (UnityEngine.Object) null)
      {
        this.textField = this.gameObject.GetComponent<Text>();
        this.textField.supportRichText = true;
      }
      if ((UnityEngine.Object) this.textField == (UnityEngine.Object) null)
        throw new Exception("Localization: GameObject " + this.gameObject.name + " don't have UnityEngine.UI.Text!");
      if (this.signature != null)
        this.textField.text = TextHelper.FormatString(this.signature, this.textField.fontSize, this.smallCaps, this.allCaps);
      else
        this.textField.text = (string) null;
    }

    private void OnLocalizationChanged() => this.Build();
  }
}
