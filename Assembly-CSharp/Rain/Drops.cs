// Decompiled with JetBrains decompiler
// Type: Rain.Drops
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using System;
using UnityEngine;

#nullable disable
namespace Rain
{
  public class Drops : MonoBehaviour
  {
    public float animationLength = 1f;
    public float nearRadius = 5f;
    public float farRadius = 25f;
    public int maxParticles = 8192;
    public DropsMesh[] dropsMeshes;
    private float _animationPhase = 0.0f;
    private int _dropMeshToUpdate = 0;
    private Material _material;
    private VertexBuffer _buffer;
    private int _activeMeshes = 0;

    private void DecreaseActiveMeshCount()
    {
      if (this._activeMeshes == 0)
      {
        Debug.LogError((object) "Drops : Trying to decrease active mesh count when it is aready zero.");
      }
      else
      {
        --this._activeMeshes;
        if (this._activeMeshes != 0)
          return;
        GameCamera.Instance.Camera.GetComponent<BlurBehind>().enabled = false;
      }
    }

    private void IncreaseActiveMeshCount()
    {
      if (this._activeMeshes == 0)
        GameCamera.Instance.Camera.GetComponent<BlurBehind>().enabled = true;
      ++this._activeMeshes;
    }

    private void Start()
    {
      MeshRenderer component = this.dropsMeshes[0].GetComponent<MeshRenderer>();
      this._material = new Material(component.sharedMaterial);
      this._material.name += " (Instance)";
      component.sharedMaterial = this._material;
      this._buffer = new VertexBuffer();
    }

    private void Update()
    {
      DropsMesh dropsMesh = this.dropsMeshes[this._dropMeshToUpdate];
      RainManager instance = RainManager.Instance;
      int num1 = !((UnityEngine.Object) instance != (UnityEngine.Object) null) ? 0 : Mathf.RoundToInt(instance.actualRainIntensity * (float) this.maxParticles / (float) this.dropsMeshes.Length);
      if (num1 > 0)
      {
        float num2 = Mathf.Lerp(this.nearRadius, this.farRadius, (float) this._dropMeshToUpdate / (float) (this.dropsMeshes.Length - 1));
        if ((UnityEngine.Object) dropsMesh == (UnityEngine.Object) null)
        {
          GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(this.dropsMeshes[0].gameObject);
          gameObject.transform.SetParent(this.transform, false);
          gameObject.name = "Drops Mesh " + this._dropMeshToUpdate.ToString();
          DropsMesh component = gameObject.GetComponent<DropsMesh>();
          component.count = num1;
          component.radius = num2;
          component.gameObject.SetActive(true);
          this.dropsMeshes[this._dropMeshToUpdate] = component;
          this.IncreaseActiveMeshCount();
        }
        else
        {
          dropsMesh.count = num1;
          dropsMesh.radius = num2;
          if (dropsMesh.gameObject.activeSelf)
          {
            dropsMesh.UpdateMesh(this._buffer);
          }
          else
          {
            dropsMesh.CreateMesh(this._buffer);
            dropsMesh.gameObject.SetActive(true);
            this.IncreaseActiveMeshCount();
          }
        }
      }
      else if (this._dropMeshToUpdate == 0)
      {
        if ((UnityEngine.Object) dropsMesh == (UnityEngine.Object) null)
          throw new Exception("updatingMesh == null");
        if (dropsMesh.gameObject.activeSelf)
        {
          dropsMesh.gameObject.SetActive(false);
          this.DecreaseActiveMeshCount();
        }
      }
      else if ((UnityEngine.Object) dropsMesh != (UnityEngine.Object) null)
      {
        UnityEngine.Object.Destroy((UnityEngine.Object) dropsMesh.gameObject);
        this.dropsMeshes[this._dropMeshToUpdate] = (DropsMesh) null;
        this.DecreaseActiveMeshCount();
      }
      ++this._dropMeshToUpdate;
      if (this._dropMeshToUpdate >= this.dropsMeshes.Length)
        this._dropMeshToUpdate = 0;
      this._animationPhase += Time.deltaTime / this.animationLength;
      double num3 = (double) Mathf.Repeat(this._animationPhase, 1f);
      this._material.SetFloat("_Phase", this._animationPhase);
    }
  }
}
