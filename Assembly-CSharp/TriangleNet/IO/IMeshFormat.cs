namespace TriangleNet.IO
{
  public interface IMeshFormat
  {
    Mesh Import(string filename);

    void Write(Mesh mesh, string filename);
  }
}
