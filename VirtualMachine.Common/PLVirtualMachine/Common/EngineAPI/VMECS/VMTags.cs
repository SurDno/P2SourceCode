// Decompiled with JetBrains decompiler
// Type: PLVirtualMachine.Common.EngineAPI.VMECS.VMTags
// Assembly: VirtualMachine.Common, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: FE3F54BA-1089-4F0E-B049-A4D27F3D2E73
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\VirtualMachine.Common.dll

using Engine.Common.Components;
using PLVirtualMachine.Common.EngineAPI.VMECS.VMAttributes;
using System.Collections.Generic;
using System.Linq;

#nullable disable
namespace PLVirtualMachine.Common.EngineAPI.VMECS
{
  [Info("TagsComponent", typeof (ITagsComponent))]
  public class VMTags : VMEngineComponent<ITagsComponent>
  {
    public const string ComponentName = "TagsComponent";
    private List<string> tagList = new List<string>();

    [Method("Get tag count", "", "")]
    public int GetTagCount() => this.Component.Tags.Count<EntityTagEnum>();

    [Method("Get tag", "index", "")]
    public EntityTagEnum GetTag(int index)
    {
      return this.Component.Tags.ElementAtOrDefault<EntityTagEnum>(index);
    }

    public List<string> TagsList => this.tagList;

    protected override void Init()
    {
      this.tagList.Clear();
      string engineData = "";
      foreach (EntityTagEnum tag in this.TemplateComponent.Tags)
      {
        string str = tag.ToString();
        this.tagList.Add(str);
        if (engineData != "")
          engineData += ",";
        engineData += str;
      }
      this.SetEngineData(engineData);
    }
  }
}
