using System.Collections.Generic;
using System.Text;
using SRDebugger.Services;
using SRDebugger.UI.Controls;
using SRF;
using SRF.Service;
using UnityEngine;

namespace SRDebugger.UI.Tabs
{
  public class InfoTabController : SRMonoBehaviour
  {
    public const string NameColor = "#BCBCBC";
    private Dictionary<string, InfoBlock> _infoBlocks = new Dictionary<string, InfoBlock>();
    public InfoBlock InfoBlockPrefab;
    public RectTransform LayoutContainer;

    protected void Start() => Construct();

    protected void OnEnable() => Refresh();

    public void Refresh()
    {
      ISystemInformationService service = SRServiceManager.GetService<ISystemInformationService>();
      foreach (KeyValuePair<string, InfoBlock> infoBlock in _infoBlocks)
        FillInfoBlock(infoBlock.Value, service.GetInfo(infoBlock.Key));
    }

    private void Construct()
    {
      ISystemInformationService service = SRServiceManager.GetService<ISystemInformationService>();
      foreach (string category in service.GetCategories())
      {
        IList<ISystemInfo> info = service.GetInfo(category);
        if (info.Count != 0)
        {
          InfoBlock block = CreateBlock(category);
          FillInfoBlock(block, info);
          _infoBlocks.Add(category, block);
        }
      }
    }

    private void FillInfoBlock(InfoBlock block, IList<ISystemInfo> info)
    {
      StringBuilder stringBuilder = new StringBuilder();
      int num1 = 0;
      foreach (ISystemInfo systemInfo in info)
      {
        if (systemInfo.Title.Length > num1)
          num1 = systemInfo.Title.Length;
      }
      int num2 = num1 + 2;
      bool flag = true;
      foreach (ISystemInfo systemInfo in info)
      {
        if (flag)
          flag = false;
        else
          stringBuilder.AppendLine();
        stringBuilder.Append("<color=");
        stringBuilder.Append("#BCBCBC");
        stringBuilder.Append(">");
        stringBuilder.Append(systemInfo.Title);
        stringBuilder.Append(": ");
        stringBuilder.Append("</color>");
        for (int length = systemInfo.Title.Length; length <= num2; ++length)
          stringBuilder.Append(' ');
        if (systemInfo.Value is bool)
          stringBuilder.Append(systemInfo.Value);
        else
          stringBuilder.Append(systemInfo.Value);
      }
      block.Content.text = stringBuilder.ToString();
    }

    private InfoBlock CreateBlock(string title)
    {
      InfoBlock block = SRInstantiate.Instantiate(InfoBlockPrefab);
      block.Title.text = title;
      block.CachedTransform.SetParent(LayoutContainer, false);
      return block;
    }
  }
}
