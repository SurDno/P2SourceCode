using Engine.Common.Components;
using PLVirtualMachine.Common.EngineAPI.VMECS.VMAttributes;
using System.Collections.Generic;
using System.Linq;

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
