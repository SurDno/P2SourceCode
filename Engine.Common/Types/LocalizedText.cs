namespace Engine.Common.Types
{
  public struct LocalizedText(ulong id) {
    public static readonly LocalizedText Empty;

    public ulong Id => id;

    public override int GetHashCode() => Id.GetHashCode();

    public override bool Equals(object a)
    {
      return a is LocalizedText localizedText && this == localizedText;
    }

    public static bool operator ==(LocalizedText a, LocalizedText b) => (long) a.Id == (long) b.Id;

    public static bool operator !=(LocalizedText a, LocalizedText b) => !(a == b);
  }
}
