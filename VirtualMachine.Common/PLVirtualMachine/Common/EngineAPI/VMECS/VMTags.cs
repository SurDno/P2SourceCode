using System.Collections.Generic;
using System.Linq;
using Engine.Common.Components;
using PLVirtualMachine.Common.EngineAPI.VMECS.VMAttributes;

namespace PLVirtualMachine.Common.EngineAPI.VMECS
{
  [Info("TagsComponent", typeof (ITagsComponent))]
  public class VMTags : VMEngineComponent<ITagsComponent>
  {
    public const string ComponentName = "TagsComponent";
    private List<string> tagList = [];

    [Method("Get tag count", "", "")]
    public int GetTagCount() => Component.Tags.Count();

    [Method("Get tag", "index", "")]
    public EntityTagEnum GetTag(int index)
    {
      return Component.Tags.ElementAtOrDefault(index);
    }

    public List<string> TagsList => tagList;

    protected override void Init()
    {
      tagList.Clear();
      string engineData = "";
      foreach (EntityTagEnum tag in TemplateComponent.Tags)
      {
        string str = tag.ToString();
        tagList.Add(str);
        if (engineData != "")
          engineData += ",";
        engineData += str;
      }
      SetEngineData(engineData);
    }
  }
}
