// Decompiled with JetBrains decompiler
// Type: EngineInspector
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using AssetDatabases;
using Engine.Common;
using Engine.Common.Services;
using Engine.Source.Commons;
using InputServices;
using Inspectors;
using Scripts.Inspectors;
using System.Collections.Generic;

#nullable disable
public class EngineInspector : MonoBehaviourInstance<EngineInspector>
{
  [Inspected]
  private ICursorController CursorController => CursorService.Instance;

  [Inspected]
  private InputService InputService => InputService.Instance;

  [Inspected]
  private UnityEngineInfo UnityEngineInfo => UnityEngineInfo.Instance;

  [Inspected(Mode = ExecuteMode.EditAndRuntime)]
  private EngineApplication EngineApplication => InstanceByRequest<EngineApplication>.Instance;

  [Inspected]
  private UpdateService UpdateService => InstanceByRequest<UpdateService>.Instance;

  [Inspected(Mode = ExecuteMode.EditAndRuntime)]
  private IEnumerable<object> Services => ServiceLocator.GetServices();

  [Inspected]
  private UnityHierarchy UnityHierarchy => UnityHierarchy.Instance;

  [Inspected]
  private IAssetDatabase AssetDatabase => AssetDatabaseService.Instance;

  [Inspected]
  public IEntity Player => ServiceLocator.GetService<ISimulation>().Player;
}
