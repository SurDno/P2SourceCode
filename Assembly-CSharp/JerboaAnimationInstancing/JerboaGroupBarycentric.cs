using Engine.Source.Audio;
using Engine.Source.Commons;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace JerboaAnimationInstancing
{
  public class JerboaGroupBarycentric : MonoBehaviour
  {
    private const float maxRespawnsPerSecond = 10f;
    private const float updateClipsTime = 1f;
    private const float partOfInstancesProcessedPerFrame = 0.1f;
    private const float maxAngularSpeed = 40f;
    private NavMeshAgent agent;
    private AudioSource audioSource;
    private float pathSampleRadius;
    private float groupSpreadAngle;
    private float groupRayDistance;
    private float groupRayDistanceSquared;
    private int groupWalkNavigationMask;
    private int groupRayCount;
    private int updateIndex;
    private float volume;
    private Vector3[] raysInWorldSpace;
    private float[] rayDistances;
    private int lastRayIndex = -1;
    private JerboaManager jerboaManager;
    private List<JerboaInstanceDescription> instances = new List<JerboaInstanceDescription>(100);
    private bool initialized;

    public bool Aloud { get; set; }

    private AudioClip AudioClip
    {
      set
      {
        if (!((UnityEngine.Object) this.audioSource != (UnityEngine.Object) null))
          return;
        this.audioSource.clip = value;
      }
    }

    public void Syncronize()
    {
      foreach (JerboaInstanceDescription instance in this.instances)
        instance.Respawn();
    }

    public void Initialize(JerboaManager jerboaManager)
    {
      this.jerboaManager = jerboaManager;
      this.initialized = true;
      this.OnEnable();
    }

    private void OnEnable()
    {
      if (!this.initialized)
        return;
      this.agent = this.gameObject.GetComponent<NavMeshAgent>();
      this.agent.obstacleAvoidanceType = ObstacleAvoidanceType.NoObstacleAvoidance;
      this.agent.autoBraking = true;
      this.agent.stoppingDistance = 0.2f;
      this.agent.autoRepath = false;
      this.agent.speed = 0.75f;
      this.agent.angularSpeed = 0.0f;
      this.pathSampleRadius = this.jerboaManager.PathSampleRadius;
      this.groupSpreadAngle = this.jerboaManager.GroupSpreadAngle;
      this.groupRayCount = this.jerboaManager.GroupRayCount;
      this.groupRayDistance = this.jerboaManager.GroupRayDistance;
      this.groupRayDistanceSquared = this.groupRayDistance * this.groupRayDistance;
      this.groupWalkNavigationMask = this.jerboaManager.GroupWalkNavigationMask;
      this.audioSource = this.GetComponent<AudioSource>();
      this.audioSource.outputAudioMixerGroup = this.jerboaManager.AudioMixerGroup;
      this.PrepareRays();
      this.agent.areaMask = this.groupWalkNavigationMask;
    }

    public void SetRandomPosition(JerboaInstanceDescription instance)
    {
      instance.GroupTriangleIndex = UnityEngine.Random.Range(0, this.groupRayCount);
      instance.GroupTriangleIndexNext = (instance.GroupTriangleIndex + 1) % this.groupRayCount;
      instance.GroupBarycentricA = UnityEngine.Random.value;
      instance.GroupBarycentricB = UnityEngine.Random.value;
      if ((double) instance.GroupBarycentricA + (double) instance.GroupBarycentricB <= 1.0)
        return;
      float groupBarycentricA = instance.GroupBarycentricA;
      instance.GroupBarycentricA = 1f - instance.GroupBarycentricB;
      instance.GroupBarycentricB = 1f - groupBarycentricA;
    }

    public Vector3 GetWorldPosition(JerboaInstanceDescription position)
    {
      int groupTriangleIndex = position.GroupTriangleIndex;
      Vector3 position1 = this.transform.position;
      Vector3 vector3_1 = this.transform.position + this.raysInWorldSpace[position.GroupTriangleIndex] * this.rayDistances[position.GroupTriangleIndex];
      Vector3 vector3_2 = this.transform.position + this.raysInWorldSpace[position.GroupTriangleIndexNext] * this.rayDistances[position.GroupTriangleIndexNext];
      float num1 = UnityEngine.Random.value;
      float num2 = UnityEngine.Random.value;
      return position1 + (vector3_1 - position1) * position.GroupBarycentricA + (vector3_2 - position1) * position.GroupBarycentricB;
    }

    private void PrepareRays()
    {
      this.rayDistances = new float[this.groupRayCount];
      this.raysInWorldSpace = new Vector3[this.groupRayCount];
      for (int index = 0; index < this.groupRayCount; ++index)
      {
        float num = (float) ((double) index * (double) this.groupSpreadAngle / (double) (this.groupRayCount - 1) - (double) this.groupSpreadAngle / 2.0);
        this.raysInWorldSpace[index] = new Vector3(Mathf.Sin(num * ((float) Math.PI / 180f)), 0.0f, Mathf.Cos(num * ((float) Math.PI / 180f)));
        this.rayDistances[index] = this.pathSampleRadius;
      }
    }

    public void AddInstance(JerboaInstanceDescription instance)
    {
      this.SetRandomPosition(instance);
      this.instances.Add(instance);
      instance.Position = this.GetWorldPosition(instance);
      instance.Rotation = Quaternion.Euler(new Vector3(0.0f, UnityEngine.Random.Range(0.0f, 360f), 0.0f));
      instance.Group = this;
    }

    private void UpdateSoundClips()
    {
      if (!this.Aloud)
      {
        this.AudioClip = (AudioClip) null;
      }
      else
      {
        float num1 = 0.0f;
        foreach (JerboaInstanceDescription instance in this.instances)
        {
          if ((double) (instance.Position - this.transform.position).sqrMagnitude < 4.0)
            ++num1;
        }
        float num2 = this.jerboaManager.Weight * this.jerboaManager.Quality;
        float num3 = num1 * num2;
        if ((double) num3 < 7.5)
          this.AudioClip = (AudioClip) null;
        else if ((double) num3 < 15.0)
          this.AudioClip = this.jerboaManager.GetJerboaAudioclip(0);
        else if ((double) num3 < 25.0)
          this.AudioClip = this.jerboaManager.GetJerboaAudioclip(1);
        else
          this.AudioClip = this.jerboaManager.GetJerboaAudioclip(2);
      }
    }

    private void UpdateSounds()
    {
      if ((UnityEngine.Object) this.audioSource == (UnityEngine.Object) null || (UnityEngine.Object) this.audioSource.clip == (UnityEngine.Object) null)
        return;
      this.volume = !this.Aloud ? Mathf.MoveTowards(this.volume, 0.0f, Time.deltaTime / 2f) : Mathf.MoveTowards(this.volume, 1f, Time.deltaTime / 2f);
      this.audioSource.volume = this.volume;
      if (this.audioSource.enabled)
      {
        if (this.audioSource.isPlaying)
        {
          if ((double) this.volume < 0.05000000074505806)
            this.audioSource.Stop();
        }
        else if ((double) this.volume > 0.05000000074505806)
          this.audioSource.PlayAndCheck();
      }
      this.UpdateAudioSourceEnable();
    }

    private void UpdateAudioSourceEnable()
    {
      bool flag = (double) (this.transform.position - EngineApplication.PlayerPosition).sqrMagnitude < (double) (this.audioSource.maxDistance * this.audioSource.maxDistance);
      if (this.audioSource.enabled == flag)
        return;
      this.audioSource.enabled = flag;
    }

    private void Update()
    {
      this.UpdateRaysLazy();
      this.UpdateSounds();
      if ((double) UnityEngine.Random.value < (double) Time.deltaTime / 1.0)
        this.UpdateSoundClips();
      if (this.agent.isOnNavMesh && !this.agent.hasPath && !this.agent.pathPending)
        this.TryCreateRandomPath(this.agent);
      int num1 = (int) ((double) this.instances.Count * 0.10000000149011612 + 1.0);
      float num2 = 10f * Time.deltaTime / (float) num1;
      for (int updateIndex = this.updateIndex; updateIndex < this.updateIndex + num1; ++updateIndex)
      {
        JerboaInstanceDescription instance = this.instances[updateIndex % this.instances.Count];
        Vector3 forward = this.GetWorldPosition(instance) - instance.Position;
        float sqrMagnitude = forward.sqrMagnitude;
        if ((double) sqrMagnitude > (double) this.groupRayDistanceSquared)
        {
          if ((double) UnityEngine.Random.value < (double) num2)
          {
            instance.Respawn();
          }
          else
          {
            Quaternion to = Quaternion.LookRotation(forward);
            instance.Rotation = Quaternion.RotateTowards(instance.Rotation, to, (float) (40.0 * (double) Time.deltaTime / 0.10000000149011612));
          }
        }
        else if ((double) sqrMagnitude > 4.0)
        {
          Quaternion to = Quaternion.LookRotation(forward);
          instance.Rotation = Quaternion.RotateTowards(instance.Rotation, to, 40f * Time.deltaTime);
        }
      }
      this.updateIndex = (this.updateIndex + num1) % this.instances.Count;
    }

    private void TryCreateRandomPath(NavMeshAgent agent)
    {
      Vector2 insideUnitCircle = UnityEngine.Random.insideUnitCircle;
      NavMeshHit hit;
      if (!NavMesh.SamplePosition(agent.gameObject.transform.position + new Vector3(insideUnitCircle.x, 0.0f, insideUnitCircle.y) * this.pathSampleRadius, out hit, 2f, this.groupWalkNavigationMask))
        return;
      agent.destination = hit.position;
    }

    public bool TryGroupTeleport(Vector3 center, float radius)
    {
      float f = UnityEngine.Random.Range(0.0f, 6.28318548f);
      NavMeshHit hit;
      if (!NavMesh.SamplePosition(center + new Vector3(Mathf.Sin(f), 0.0f, Mathf.Cos(f)) * radius, out hit, 3f, this.groupWalkNavigationMask))
        return false;
      this.agent.Warp(hit.position);
      this.UpdateRays();
      return true;
    }

    private void UpdateRays()
    {
      for (int index = 0; index < this.groupRayCount; ++index)
      {
        NavMeshHit hit;
        this.rayDistances[index] = !NavMesh.Raycast(this.transform.position, this.transform.position + this.raysInWorldSpace[index] * this.groupRayDistance, out hit, this.groupWalkNavigationMask) ? this.groupRayDistance : hit.distance;
      }
    }

    private void UpdateRaysLazy()
    {
      this.lastRayIndex = (this.lastRayIndex + 1) % this.groupRayCount;
      NavMeshHit hit;
      if (NavMesh.Raycast(this.transform.position, this.transform.position + this.raysInWorldSpace[this.lastRayIndex] * this.groupRayDistance, out hit, this.groupWalkNavigationMask))
        this.rayDistances[this.lastRayIndex] = hit.distance;
      else
        this.rayDistances[this.lastRayIndex] = this.groupRayDistance;
    }

    private void OnDrawGizmos()
    {
      Gizmos.color = Color.red;
      for (int index = 0; index < this.raysInWorldSpace.Length; ++index)
      {
        Vector3 from = this.transform.position + 0.5f * Vector3.up;
        Gizmos.DrawLine(from, from + this.raysInWorldSpace[index] * this.rayDistances[index]);
      }
    }
  }
}
