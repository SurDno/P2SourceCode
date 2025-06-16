// Decompiled with JetBrains decompiler
// Type: EngineDependent
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using Cofe.Meta;
using Engine.Common.Services;
using Engine.Source.Commons;
using System;
using UnityEngine;

#nullable disable
public abstract class EngineDependent : MonoBehaviour
{
  private bool waiting;

  protected bool Connected { get; private set; }

  protected virtual void OnEnable()
  {
    if (InstanceByRequest<EngineApplication>.Instance.IsInitialized)
      this.Connect();
    else
      this.BeginWaiting();
  }

  private void Disconnect()
  {
    if (!this.Connected)
      return;
    this.Connected = false;
    this.OnDisconnectFromEngine();
  }

  private void Connect()
  {
    if (this.Connected)
      return;
    this.Connected = true;
    MetaService.GetContainer(((object) this).GetType()).GetHandler(FromLocatorAttribute.Id).Compute((object) this, (object) null);
    this.EndWaiting();
    this.OnConnectToEngine();
  }

  private void EndWaiting()
  {
    if (!this.waiting)
      return;
    this.waiting = false;
    InstanceByRequest<EngineApplication>.Instance.OnInitialized -= new Action(this.Connect);
  }

  private void BeginWaiting()
  {
    if (this.waiting)
      return;
    this.waiting = true;
    InstanceByRequest<EngineApplication>.Instance.OnInitialized += new Action(this.Connect);
  }

  protected virtual void OnDisable()
  {
    this.EndWaiting();
    this.Disconnect();
  }

  protected abstract void OnConnectToEngine();

  protected abstract void OnDisconnectFromEngine();
}
