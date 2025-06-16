using System.Collections;
using System.Collections.Generic;

namespace Scripts.Editor
{
  public struct AssetsEnumerator : IEnumerable<string>, IEnumerable
  {
    private IEnumerable<string> assets;
    private string title;

    public AssetsEnumerator(string title, IEnumerable<string> assets)
    {
      this.title = title;
      this.assets = assets;
    }

    public IEnumerator<string> GetEnumerator() => assets.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator()
    {
      return GetEnumerator();
    }
  }
}
