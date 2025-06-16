using System;
using Engine.Common.Services;
using Engine.Impl.Services;

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
      get => signature;
      set
      {
        signature = value;
        if (!this.isActiveAndEnabled)
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
      if ((UnityEngine.Object) textField == (UnityEngine.Object) null)
      {
        textField = this.gameObject.GetComponent<Text>();
        textField.supportRichText = true;
      }
      if ((UnityEngine.Object) textField == (UnityEngine.Object) null)
        throw new Exception("Localization: GameObject " + this.gameObject.name + " don't have UnityEngine.UI.Text!");
      if (signature != null)
        textField.text = TextHelper.FormatString(signature, textField.fontSize, smallCaps, allCaps);
      else
        textField.text = (string) null;
    }

    private void OnLocalizationChanged() => Build();
  }
}
