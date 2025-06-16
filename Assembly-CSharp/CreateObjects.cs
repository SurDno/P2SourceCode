using Cofe.Meta;
using Engine.Common;
using Engine.Common.Services;
using Engine.Source.Connections;
using Engine.Source.Services.Templates;
using Engine.Source.Test;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public class CreateObjects : MonoBehaviour
{
  private List<IEntity> instances;
  [SerializeField]
  private IEntitySerializable template;
  [SerializeField]
  private int count;

  private IEnumerator Start()
  {
    yield return (object) new WaitForSeconds(1f);
    this.Test();
    yield return (object) new WaitForSeconds(1f);
  }

  private void Test()
  {
    IFactory service = ServiceLocator.GetService<IFactory>();
    ServiceLocator.GetService<IEditorTemplateService>();
    IEntity template = this.template.Value;
    if (template == null)
    {
      UnityEngine.Debug.LogError((object) ("Template not found, id : " + (object) this.template.Id));
    }
    else
    {
      this.instances = new List<IEntity>(this.count);
      Stopwatch stopwatch = new Stopwatch();
      stopwatch.Restart();
      for (int index = 0; index < this.count; ++index)
      {
        IEntity target = service.Instantiate<IEntity>(template);
        MetaService.GetContainer(target.GetType()).GetHandler(TestAttribute.Id).Compute((object) target, (object) null);
        this.instances.Add(target);
      }
      stopwatch.Stop();
      UnityEngine.Debug.LogError((object) ("Complete, count : " + (object) this.count + " , elapsed : " + (object) stopwatch.Elapsed));
    }
  }
}
