// Decompiled with JetBrains decompiler
// Type: Engine.Source.Services.POIGroupActivity
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using System.Collections.Generic;
using UnityEngine;

#nullable disable
namespace Engine.Source.Services
{
  public class POIGroupActivity
  {
    private bool isDialog;
    private GroupActivityObject activityObject;
    private Dictionary<GameObject, bool> CharactersReady;
    private List<GameObject> characters = new List<GameObject>();

    public bool IsDialog => this.isDialog;

    public GroupActivityObject ActivityObject
    {
      get => this.activityObject;
      set
      {
        this.activityObject = value;
        if (!((Object) this.activityObject != (Object) null))
          return;
        this.isDialog = this.activityObject.IsDialogActivity;
      }
    }

    public List<GameObject> Characters
    {
      get => this.characters;
      set
      {
        this.characters = value;
        this.CharactersReady = new Dictionary<GameObject, bool>();
        foreach (GameObject character in this.characters)
          this.CharactersReady[character] = false;
      }
    }

    public void SetCharacterReady(GameObject character)
    {
      if (!this.CharactersReady.ContainsKey(character))
        return;
      this.CharactersReady[character] = true;
    }

    public bool NoCharactersReady() => !this.CharactersReady.ContainsValue(true);

    public bool AllCharactersReady() => !this.CharactersReady.ContainsValue(false);
  }
}
