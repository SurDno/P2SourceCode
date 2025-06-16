using System.Collections.Generic;
using Engine.Source.Components.BoundCharacters;
using Inspectors;

namespace Engine.Source.Services
{
  [GameService(typeof (BoundCharactersService))]
  public class BoundCharactersService
  {
    [Inspected]
    private List<BoundCharacterComponent> items = new List<BoundCharacterComponent>();

    public IEnumerable<BoundCharacterComponent> Items
    {
      get => items;
    }

    public void AddKeyCharacter(BoundCharacterComponent item)
    {
      if (items.Contains(item))
        return;
      items.Add(item);
    }

    public void RemoveKeyCharacter(BoundCharacterComponent item)
    {
      int index = items.IndexOf(item);
      if (index == -1)
        return;
      items.RemoveAt(index);
    }
  }
}
