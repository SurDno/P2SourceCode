namespace Engine.Common.Types
{
  public struct Pair<T1, T2>(T1 item1, T2 item2) {
    public T1 Item1 { get; set; } = item1;

    public T2 Item2 { get; set; } = item2;
  }
}
