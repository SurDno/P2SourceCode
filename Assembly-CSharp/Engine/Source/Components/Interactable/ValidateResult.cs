namespace Engine.Source.Components.Interactable
{
  public struct ValidateResult(bool result, string reason = "") {
    public bool Result { get; private set; } = result;

    public string Reason { get; private set; } = reason;
  }
}
