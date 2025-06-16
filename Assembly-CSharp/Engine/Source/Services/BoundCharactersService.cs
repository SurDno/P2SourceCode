using Engine.Source.Components.BoundCharacters;
using Inspectors;
using System;
using System.Collections.Generic;

namespace Engine.Source.Services
{
  [GameService(new Type[] {typeof (BoundCharactersService)})]
  public class BoundCharactersService
  {
    [Inspected]
    private List<BoundCharacterComponent> items = new List<BoundCharacterComponent>();

    public IEnumerable<BoundCharacterComponent> Items
    {
      get => (IEnumerable<BoundCharacterComponent>) this.items;
    }

    public void AddKeyCharacter(BoundCharacterComponent item)
    {
      if (this.items.Contains(item))
        return;
      this.items.Add(item);
    }

    public void RemoveKeyCharacter(BoundCharacterComponent item)
    {
      int index = this.items.IndexOf(item);
      if (index == -1)
        return;
      this.items.RemoveAt(index);
    }
  }
}
