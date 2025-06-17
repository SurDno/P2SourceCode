using System.Collections;
using System.Collections.Generic;

namespace Scripts.Editor
{
  public struct AssetsEnumerator(string title, IEnumerable<string> assets) : IEnumerable<string>, IEnumerable {
    private string title = title;

    public IEnumerator<string> GetEnumerator() => assets.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator()
    {
      return GetEnumerator();
    }
  }
}
