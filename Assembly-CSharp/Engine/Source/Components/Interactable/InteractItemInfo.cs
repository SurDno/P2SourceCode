using Engine.Source.Services.Inputs;
using Inspectors;

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
