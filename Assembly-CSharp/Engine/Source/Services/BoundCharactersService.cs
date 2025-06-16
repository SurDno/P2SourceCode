// Decompiled with JetBrains decompiler
// Type: Engine.Source.Services.BoundCharactersService
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using Engine.Source.Components.BoundCharacters;
using Inspectors;
using System;
using System.Collections.Generic;

#nullable disable
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
