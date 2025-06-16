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
      X = x;
      Y = y;
    }

    public override string ToString()
    {
      return DefaultConverter.ToString(X) + " " + DefaultConverter.ToString(Y);
    }

    public override int GetHashCode() => X.GetHashCode() ^ Y.GetHashCode();

    public override bool Equals(object other) => other is Position position && this == position;

    public static bool operator ==(Position a, Position b)
    {
      return a.X == (double) b.X && a.Y == (double) b.Y;
    }

    public static bool operator !=(Position a, Position b) => !(a == b);
  }
}
