// Decompiled with JetBrains decompiler
// Type: Engine.Source.Components.Interactable.InteractItemInfo
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using Engine.Source.Services.Inputs;
using Inspectors;

#nullable disable
namespace Engine.Source.Components.Interactable
{
  public class InteractItemInfo
  {
    [Inspected]
    public InteractItem Item;
    [Inspected]
    public GameActionType OverrideAction;
    [Inspected]
    public bool Invalid;
    [Inspected]
    public bool Dublicate;
    [Inspected]
    public string Reason;
    [Inspected]
    public bool Crime;
  }
}
