using Cofe.Serializations.Converters;

namespace Engine.Common.Types
{
  public struct Position
  {
    public static readonly Position Zero;
    public float X;
    public float Y;

    public Position(float x, float y)
    {
      this.X = x;
      this.Y = y;
    }

    public override string ToString()
    {
      return DefaultConverter.ToString(this.X) + " " + DefaultConverter.ToString(this.Y);
    }

    public override int GetHashCode() => this.X.GetHashCode() ^ this.Y.GetHashCode();

    public override bool Equals(object other) => other is Position position && this == position;

    public static bool operator ==(Position a, Position b)
    {
      return (double) a.X == (double) b.X && (double) a.Y == (double) b.Y;
    }

    public static bool operator !=(Position a, Position b) => !(a == b);
  }
}
