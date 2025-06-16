using System.Collections.Generic;
using UnityEngine;

namespace Engine.Source.Services
{
  public class POIGroupActivity
  {
    private bool isDialog;
    private GroupActivityObject activityObject;
    private Dictionary<GameObject, bool> CharactersReady;
    private List<GameObject> characters = new List<GameObject>();

    public bool IsDialog => isDialog;

    public GroupActivityObject ActivityObject
    {
      get => activityObject;
      set
      {
        activityObject = value;
        if (!(activityObject != null))
          return;
        isDialog = activityObject.IsDialogActivity;
      }
    }

    public List<GameObject> Characters
    {
      get => characters;
      set
      {
        characters = value;
        CharactersReady = new Dictionary<GameObject, bool>();
        foreach (GameObject character in characters)
          CharactersReady[character] = false;
      }
    }

    public void SetCharacterReady(GameObject character)
    {
      if (!CharactersReady.ContainsKey(character))
        return;
      CharactersReady[character] = true;
    }

    public bool NoCharactersReady() => !CharactersReady.ContainsValue(true);

    public bool AllCharactersReady() => !CharactersReady.ContainsValue(false);
  }
}
