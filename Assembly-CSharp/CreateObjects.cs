using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using Cofe.Meta;
using Engine.Common;
using Engine.Common.Services;
using Engine.Source.Connections;
using Engine.Source.Services.Templates;
using Engine.Source.Test;

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
    Test();
    yield return (object) new WaitForSeconds(1f);
  }

  private void Test()
  {
    IFactory service = ServiceLocator.GetService<IFactory>();
    ServiceLocator.GetService<IEditorTemplateService>();
    IEntity template = this.template.Value;
    if (template == null)
    {
      UnityEngine.Debug.LogError((object) ("Template not found, id : " + this.template.Id));
    }
    else
    {
      instances = new List<IEntity>(count);
      Stopwatch stopwatch = new Stopwatch();
      stopwatch.Restart();
      for (int index = 0; index < count; ++index)
      {
        IEntity target = service.Instantiate(template);
        MetaService.GetContainer(target.GetType()).GetHandler(TestAttribute.Id).Compute(target, null);
        instances.Add(target);
      }
      stopwatch.Stop();
      UnityEngine.Debug.LogError((object) ("Complete, count : " + count + " , elapsed : " + stopwatch.Elapsed));
    }
  }
}
