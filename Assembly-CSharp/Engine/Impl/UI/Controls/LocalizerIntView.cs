using Engine.Behaviours.Localization;
using UnityEngine;

namespace Engine.Impl.UI.Controls
{
  public class LocalizerIntView : IntView
  {
    [SerializeField]
    private Localizer localizer;
    [SerializeField]
    private string signaturePrefix;
    [SerializeField]
    private string signatureSuffix;

    public override void SkipAnimation()
    {
    }

    protected override void ApplyIntValue()
    {
      if ((Object) this.localizer == (Object) null)
        return;
      this.localizer.Signature = this.signaturePrefix + this.IntValue.ToString() + this.signatureSuffix;
    }
  }
}
