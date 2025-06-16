// Decompiled with JetBrains decompiler
// Type: Engine.Services.SelectionService
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using Engine.Source.Services;
using Inspectors;
using UnityEngine;

#nullable disable
namespace Engine.Services
{
  [GameService(new System.Type[] {typeof (SelectionService)})]
  public class SelectionService
  {
    [Inspected]
    private object[] selections = new object[10];

    public int SelectionCount => this.selections.Length;

    public void SetSelection(int index, object selection)
    {
      if (index < 0 || index >= this.selections.Length)
        Debug.LogWarning((object) "Selection index out of range");
      else
        this.selections[index] = selection;
    }

    public object GetSelection(int index)
    {
      if (index >= 0 && index < this.selections.Length)
        return this.selections[index];
      Debug.LogWarning((object) "Selection index out of range");
      return (object) null;
    }
  }
}
