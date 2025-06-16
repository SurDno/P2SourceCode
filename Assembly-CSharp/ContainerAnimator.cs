// Decompiled with JetBrains decompiler
// Type: ContainerAnimator
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using Inspectors;
using Scripts.Utility;
using System;
using UnityEngine;

#nullable disable
public class ContainerAnimator : MonoBehaviour
{
  [SerializeField]
  private ContainerAnimator.TransformInfo Opened;
  [SerializeField]
  private ContainerAnimator.TransformInfo Closed;
  private bool isOpened;
  private float progress;
  private float speed = 2f;

  public bool IsOpened
  {
    get => this.isOpened;
    set
    {
      if (this.isOpened == value)
        return;
      this.isOpened = value;
    }
  }

  private void Update()
  {
    if (this.isOpened)
    {
      if ((double) this.progress == 1.0)
        return;
      this.progress += Time.deltaTime * this.speed;
      if ((double) this.progress > 1.0)
        this.progress = 1f;
    }
    else
    {
      if ((double) this.progress == 0.0)
        return;
      this.progress -= Time.deltaTime * this.speed;
      if ((double) this.progress < 0.0)
        this.progress = 0.0f;
    }
    float t = EasyUtility.QuarticEaseOut(this.progress);
    this.transform.localPosition = Vector3.LerpUnclamped(this.Closed.Position, this.Opened.Position, t);
    this.transform.localRotation = Quaternion.LerpUnclamped(this.Closed.Rotation, this.Opened.Rotation, t);
  }

  [Inspected(Mode = ExecuteMode.Edit)]
  private void SaveAsClosed()
  {
    this.Closed.Position = this.transform.localPosition;
    this.Closed.Rotation = this.transform.localRotation;
  }

  [Inspected(Mode = ExecuteMode.Edit)]
  private void SaveAsOpened()
  {
    this.Opened.Position = this.transform.localPosition;
    this.Opened.Rotation = this.transform.localRotation;
  }

  [Inspected(Mode = ExecuteMode.EditAndRuntime)]
  private void Close()
  {
    this.progress = 0.0f;
    this.transform.localPosition = this.Closed.Position;
    this.transform.localRotation = this.Closed.Rotation;
  }

  [Inspected(Mode = ExecuteMode.EditAndRuntime)]
  private void Open()
  {
    this.progress = 1f;
    this.transform.localPosition = this.Opened.Position;
    this.transform.localRotation = this.Opened.Rotation;
  }

  [Serializable]
  public struct TransformInfo
  {
    public Vector3 Position;
    public Quaternion Rotation;
  }
}
