namespace ClipperLib
{
  public struct IntRect(long l, long t, long r, long b) {
    public long left = l;
    public long top = t;
    public long right = r;
    public long bottom = b;

    public IntRect(IntRect ir) : this(ir.left, ir.top, ir.right, ir.bottom) { }
  }
}
