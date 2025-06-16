// Decompiled with JetBrains decompiler
// Type: HerbBrideFall
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using Engine.Behaviours.Components;
using Engine.Common.Services;
using Engine.Source.Audio;
using Engine.Source.Commons;
using Engine.Source.Services;
using Engine.Source.Services.CameraServices;
using Inspectors;
using System;
using UnityEngine;
using UnityEngine.SceneManagement;

#nullable disable
public class HerbBrideFall : MonoBehaviour
{
  [SerializeField]
  private AudioClip impactAudioClip;
  [SerializeField]
  private AudioSource termitnikAudiosource;
  [SerializeField]
  private AudioSource womanCryAudiosource;
  private float jumpTime = 2f;
  private float maxPlayerDistance = 40f;
  [Inspected]
  private bool IsGame;
  private Animator animator;
  private HerbBrideFall.StateEnum state;
  private Quaternion initialRotation;
  private Vector3 initialPosition;
  private Pivot pivot;
  private AudioSource audioSource;
  private float lookTime;
  private float termitnikVolume;
  private Collider collider;
  private Collider[] colliders = new Collider[64];

  public void Wait() => this.SetState(HerbBrideFall.StateEnum.Waiting);

  public void Jump() => this.SetState(HerbBrideFall.StateEnum.Falling);

  private void Start()
  {
    this.IsGame = SceneManager.GetActiveScene().name != "TermitnikFall";
    this.initialRotation = this.transform.rotation;
    this.initialPosition = this.transform.position;
    this.pivot = this.GetComponent<Pivot>();
    this.animator = this.pivot.GetAnimator();
    this.audioSource = this.GetComponent<AudioSource>();
    this.pivot.GetAnimatorEventProxy().AnimatorMoveEvent += new Action(this.HerbBrideFall_AnimatorMoveEvent);
    this.collider = this.GetComponent<Collider>();
    this.SetState(HerbBrideFall.StateEnum.Waiting);
  }

  private void HerbBrideFall_AnimatorMoveEvent()
  {
    this.transform.position += this.animator.deltaPosition;
    this.transform.rotation *= this.animator.deltaRotation;
  }

  private void SetState(HerbBrideFall.StateEnum state)
  {
    switch (state)
    {
      case HerbBrideFall.StateEnum.Waiting:
        this.animator.SetTrigger("Triggers/Reset");
        this.animator.ResetTrigger("Triggers/Fall");
        this.transform.SetPositionAndRotation(this.initialPosition, this.initialRotation);
        break;
      case HerbBrideFall.StateEnum.Falling:
        if (this.IsGame)
          ServiceLocator.GetService<LogicEventService>().FireCommonEvent("Termitnik_Jump");
        this.animator.ResetTrigger("Triggers/Reset");
        this.animator.SetTrigger("Triggers/Fall");
        this.audioSource.PlayAndCheck();
        break;
      case HerbBrideFall.StateEnum.Dead:
        this.audioSource.Stop();
        this.audioSource.PlayOneShot(this.impactAudioClip);
        break;
    }
    this.state = state;
  }

  private void Update()
  {
    switch (this.state)
    {
      case HerbBrideFall.StateEnum.Waiting:
        this.UpdatePlayerSeesHerbBride();
        this.collider.enabled = true;
        break;
      case HerbBrideFall.StateEnum.Falling:
        this.collider.enabled = false;
        this.lookTime = 0.0f;
        if (this.IsAboutToCollideWithTerrain())
        {
          this.SetState(HerbBrideFall.StateEnum.Dead);
          break;
        }
        break;
      case HerbBrideFall.StateEnum.Dead:
        this.collider.enabled = false;
        this.lookTime = 0.0f;
        if (!this.IsGame && Input.GetKeyDown(KeyCode.E))
        {
          this.SetState(HerbBrideFall.StateEnum.Waiting);
          break;
        }
        break;
    }
    this.UpdateTermitnikAudio();
    this.UpdateCryingAudio();
  }

  private void UpdateTermitnikAudio()
  {
    if ((UnityEngine.Object) this.termitnikAudiosource == (UnityEngine.Object) null)
      return;
    GameObject playerGameObject = this.GetPlayerGameObject();
    if ((UnityEngine.Object) playerGameObject == (UnityEngine.Object) null)
      return;
    float magnitude = (playerGameObject.transform.position - this.transform.position).magnitude;
    if (this.state == HerbBrideFall.StateEnum.Waiting)
    {
      this.termitnikAudiosource.spatialBlend = Mathf.Clamp01(magnitude / 50f);
      this.termitnikVolume = Mathf.MoveTowards(this.termitnikVolume, 1f, Time.deltaTime / 4f);
      this.termitnikAudiosource.volume = this.termitnikVolume;
      if (this.termitnikAudiosource.isPlaying)
        return;
      this.termitnikAudiosource.PlayAndCheck();
    }
    else
    {
      this.termitnikVolume = Mathf.MoveTowards(this.termitnikVolume, 0.0f, Time.deltaTime / 4f);
      this.termitnikAudiosource.volume = this.termitnikVolume;
    }
  }

  private void UpdateCryingAudio()
  {
    if ((UnityEngine.Object) this.womanCryAudiosource == (UnityEngine.Object) null)
      return;
    GameObject playerGameObject = this.GetPlayerGameObject();
    if ((UnityEngine.Object) playerGameObject == (UnityEngine.Object) null)
      return;
    float magnitude = (playerGameObject.transform.position - this.transform.position).magnitude;
    if (this.state == HerbBrideFall.StateEnum.Waiting)
    {
      this.womanCryAudiosource.volume = 1f;
      if (this.womanCryAudiosource.isPlaying)
        return;
      this.womanCryAudiosource.PlayAndCheck();
    }
    else
    {
      if (!this.womanCryAudiosource.isPlaying)
        return;
      this.womanCryAudiosource.Stop();
    }
  }

  private bool IsAboutToCollideWithTerrain()
  {
    int num = Physics.OverlapSphereNonAlloc(this.animator.gameObject.transform.position, 3f, this.colliders);
    for (int index = 0; index < num; ++index)
    {
      if ((UnityEngine.Object) this.colliders[index].gameObject.GetComponent<TerrainCollider>() != (UnityEngine.Object) null)
        return true;
    }
    return false;
  }

  private Camera GetCamera()
  {
    if (!this.IsGame)
      return Camera.main;
    return ServiceLocator.GetService<CameraService>().Kind != CameraKindEnum.FirstPerson_Controlling ? (Camera) null : GameCamera.Instance.Camera;
  }

  private GameObject GetPlayerGameObject()
  {
    if (!this.IsGame)
      return GameObject.Find("FPSController");
    return ((IEntityView) ServiceLocator.GetService<ISimulation>().Player)?.GameObject;
  }

  private void OnDrawGizmos()
  {
    Camera camera = this.GetCamera();
    GameObject playerGameObject = this.GetPlayerGameObject();
    if ((UnityEngine.Object) camera == (UnityEngine.Object) null || (UnityEngine.Object) playerGameObject == (UnityEngine.Object) null)
      return;
    Vector3 forward = camera.transform.forward;
    Vector3 vector3_1 = this.transform.position + 1f * this.transform.forward;
    Vector3 vector3_2 = playerGameObject.transform.position + Vector3.up + forward * 1f;
    Vector3 vector3_3 = vector3_2 - vector3_1;
    float magnitude = vector3_3.magnitude;
    Vector3 vector3_4 = vector3_3 / magnitude;
    Gizmos.color = Color.red;
    Gizmos.DrawSphere(vector3_1, 0.2f);
    Gizmos.DrawSphere(vector3_2 + forward * 2f, 0.2f);
    Gizmos.color = Color.green;
    Gizmos.DrawLine(vector3_1, vector3_1 + this.maxPlayerDistance * vector3_4);
  }

  private void UpdatePlayerSeesHerbBride()
  {
    Camera camera = this.GetCamera();
    GameObject playerGameObject = this.GetPlayerGameObject();
    if ((UnityEngine.Object) camera == (UnityEngine.Object) null || (UnityEngine.Object) playerGameObject == (UnityEngine.Object) null)
      return;
    Vector3 forward = camera.transform.forward;
    Vector3 origin = this.transform.position + 1f * this.transform.forward;
    Vector3 vector3 = playerGameObject.transform.position + Vector3.up + forward * 1f - origin;
    float magnitude = vector3.magnitude;
    Vector3 direction = vector3 / magnitude;
    if ((double) magnitude > (double) this.maxPlayerDistance)
      this.lookTime = 0.0f;
    else if ((double) Vector3.Dot(-direction, forward) < (double) Mathf.Cos(0.5235988f))
    {
      this.lookTime = 0.0f;
    }
    else
    {
      RaycastHit hitInfo;
      if (Physics.Raycast(origin, direction, out hitInfo, magnitude))
      {
        Debug.Log((object) hitInfo.collider.gameObject);
        this.lookTime = 0.0f;
      }
      else
      {
        this.lookTime += Time.deltaTime;
        if ((double) this.lookTime <= (double) this.jumpTime)
          return;
        this.Jump();
      }
    }
  }

  private enum StateEnum
  {
    Waiting,
    Falling,
    Dead,
  }
}
