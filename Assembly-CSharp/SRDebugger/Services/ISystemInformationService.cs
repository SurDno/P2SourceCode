using System.Collections.Generic;

namespace SRDebugger.Services
{
  public interface ISystemInformationService
  {
    IEnumerable<string> GetCategories();

    IList<ISystemInfo> GetInfo(string category);

    Dictionary<string, Dictionary<string, object>> CreateReport(bool includePrivate = false);

    void AddInfo(string category, ISystemInfo info);
  }
}
