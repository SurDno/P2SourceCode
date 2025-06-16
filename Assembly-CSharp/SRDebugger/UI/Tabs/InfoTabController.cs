using SRDebugger.Services;
using SRDebugger.UI.Controls;
using SRF;
using SRF.Service;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace SRDebugger.UI.Tabs
{
  public class InfoTabController : SRMonoBehaviour
  {
    public const string NameColor = "#BCBCBC";
    private Dictionary<string, InfoBlock> _infoBlocks = new Dictionary<string, InfoBlock>();
    public InfoBlock InfoBlockPrefab;
    public RectTransform LayoutContainer;

    protected void Start() => this.Construct();

    protected void OnEnable() => this.Refresh();

    public void Refresh()
    {
      ISystemInformationService service = SRServiceManager.GetService<ISystemInformationService>();
      foreach (KeyValuePair<string, InfoBlock> infoBlock in this._infoBlocks)
        this.FillInfoBlock(infoBlock.Value, service.GetInfo(infoBlock.Key));
    }

    private void Construct()
    {
      ISystemInformationService service = SRServiceManager.GetService<ISystemInformationService>();
      foreach (string category in service.GetCategories())
      {
        IList<ISystemInfo> info = service.GetInfo(category);
        if (info.Count != 0)
        {
          InfoBlock block = this.CreateBlock(category);
          this.FillInfoBlock(block, info);
          this._infoBlocks.Add(category, block);
        }
      }
    }

    private void FillInfoBlock(InfoBlock block, IList<ISystemInfo> info)
    {
      StringBuilder stringBuilder = new StringBuilder();
      int num1 = 0;
      foreach (ISystemInfo systemInfo in (IEnumerable<ISystemInfo>) info)
      {
        if (systemInfo.Title.Length > num1)
          num1 = systemInfo.Title.Length;
      }
      int num2 = num1 + 2;
      bool flag = true;
      foreach (ISystemInfo systemInfo in (IEnumerable<ISystemInfo>) info)
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
          stringBuilder.Append(systemInfo.Value.ToString());
        else
          stringBuilder.Append(systemInfo.Value);
      }
      block.Content.text = stringBuilder.ToString();
    }

    private InfoBlock CreateBlock(string title)
    {
      InfoBlock block = SRInstantiate.Instantiate<InfoBlock>(this.InfoBlockPrefab);
      block.Title.text = title;
      block.CachedTransform.SetParent((Transform) this.LayoutContainer, false);
      return block;
    }
  }
}
