using System;
using Engine.Common.Services;
using Engine.Impl.Services;
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
    private bool allCaps;
    private Text textField;

    public string Signature
    {
      get => signature;
      set
      {
        signature = value;
        if (!isActiveAndEnabled)
          return;
        Build();
      }
    }

    protected override void OnEnable()
    {
      base.OnEnable();
      Build();
      ServiceLocator.GetService<LocalizationService>().LocalizationChanged -= OnLocalizationChanged;
      ServiceLocator.GetService<LocalizationService>().LocalizationChanged += OnLocalizationChanged;
    }

    protected override void OnDisable()
    {
      base.OnDisable();
      ServiceLocator.GetService<LocalizationService>().LocalizationChanged -= OnLocalizationChanged;
    }

    public void Build()
    {
      if (textField == null)
      {
        textField = gameObject.GetComponent<Text>();
        textField.supportRichText = true;
      }
      if (textField == null)
        throw new Exception("Localization: GameObject " + gameObject.name + " don't have UnityEngine.UI.Text!");
      if (signature != null)
        textField.text = TextHelper.FormatString(signature, textField.fontSize, smallCaps, allCaps);
      else
        textField.text = null;
    }

    private void OnLocalizationChanged() => Build();
  }
}
