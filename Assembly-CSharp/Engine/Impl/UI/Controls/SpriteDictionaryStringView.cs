// Decompiled with JetBrains decompiler
// Type: Engine.Impl.UI.Controls.SpriteDictionaryStringView
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
namespace Engine.Impl.UI.Controls
{
  public class SpriteDictionaryStringView : StringView
  {
    [SerializeField]
    private SpriteView view;
    [SerializeField]
    private StringSpritePair[] dictionary;

    public override void SkipAnimation()
    {
    }

    protected override void ApplyStringValue()
    {
      if (this.dictionary == null || (Object) this.view == (Object) null)
        return;
      Sprite sprite = (Sprite) null;
      foreach (StringSpritePair stringSpritePair in this.dictionary)
      {
        if (stringSpritePair.Key == this.StringValue)
        {
          sprite = stringSpritePair.Value;
          break;
        }
      }
      this.view.SetValue(sprite, false);
    }
  }
}
