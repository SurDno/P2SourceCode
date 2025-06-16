namespace Engine.Common.Types;

public struct LocalizedText {
	public static readonly LocalizedText Empty;
	private readonly ulong id;

	public LocalizedText(ulong id) {
		this.id = id;
	}

	public ulong Id => id;

	public override int GetHashCode() {
		return Id.GetHashCode();
	}

	public override bool Equals(object a) {
		return a is LocalizedText localizedText && this == localizedText;
	}

	public static bool operator ==(LocalizedText a, LocalizedText b) {
		return (long)a.Id == (long)b.Id;
	}

	public static bool operator !=(LocalizedText a, LocalizedText b) {
		return !(a == b);
	}
}