// Decompiled with JetBrains decompiler
// Type: SRF.Service.SRServiceBase`1
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

#nullable disable
namespace SRF.Service
{
  public abstract class SRServiceBase<T> : SRMonoBehaviour where T : class
  {
    protected virtual void Awake() => SRServiceManager.RegisterService<T>((object) this);

    protected virtual void OnDestroy() => SRServiceManager.UnRegisterService<T>();
  }
}
