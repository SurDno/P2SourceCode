// Decompiled with JetBrains decompiler
// Type: Engine.Source.UI.Menu.Protagonist.BoundCharacters.BoundCharactersGroupView
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using Engine.Common.BoundCharacters;
using Engine.Impl.UI.Controls;
using Engine.Source.Components.BoundCharacters;
using UnityEngine;

#nullable disable
namespace Engine.Source.UI.Menu.Protagonist.BoundCharacters
{
  public class BoundCharactersGroupView : MonoBehaviour
  {
    [SerializeField]
    private BoundCharacterView characterViewPrefab;
    [SerializeField]
    private StringView nameView;
    [SerializeField]
    private RectTransform layout;

    public void SetGroup(BoundCharacterGroup group)
    {
      if (group != 0)
      {
        this.nameView.StringValue = "{UI.Menu.Protagonist.BoundCharacterGroup." + group.ToString() + "}";
        this.nameView.gameObject.SetActive(true);
      }
      else
        this.nameView.gameObject.SetActive(false);
    }

    public BoundCharacterView AddCharacter(BoundCharacterComponent character)
    {
      BoundCharacterView boundCharacterView = Object.Instantiate<BoundCharacterView>(this.characterViewPrefab, (Transform) this.layout, false);
      boundCharacterView.SetCharacter(character);
      return boundCharacterView;
    }
  }
}
