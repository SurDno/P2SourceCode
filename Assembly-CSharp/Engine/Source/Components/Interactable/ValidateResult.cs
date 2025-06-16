namespace Engine.Source.Components.Interactable;

public struct ValidateResult {
	public ValidateResult(bool result, string reason = "") {
		Result = result;
		Reason = reason;
	}

	public bool Result { get; private set; }

	public string Reason { get; private set; }
}