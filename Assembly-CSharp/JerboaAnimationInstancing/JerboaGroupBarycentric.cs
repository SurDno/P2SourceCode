﻿using System;
using System.Collections.Generic;
using Engine.Source.Audio;
using Engine.Source.Commons;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

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
    private List<JerboaInstanceDescription> instances = new(100);
    private bool initialized;

    public bool Aloud { get; set; }

    private AudioClip AudioClip
    {
      set
      {
        if (!(audioSource != null))
          return;
        audioSource.clip = value;
      }
    }

    public void Syncronize()
    {
      foreach (JerboaInstanceDescription instance in instances)
        instance.Respawn();
    }

    public void Initialize(JerboaManager jerboaManager)
    {
      this.jerboaManager = jerboaManager;
      initialized = true;
      OnEnable();
    }

    private void OnEnable()
    {
      if (!initialized)
        return;
      agent = gameObject.GetComponent<NavMeshAgent>();
      agent.obstacleAvoidanceType = ObstacleAvoidanceType.NoObstacleAvoidance;
      agent.autoBraking = true;
      agent.stoppingDistance = 0.2f;
      agent.autoRepath = false;
      agent.speed = 0.75f;
      agent.angularSpeed = 0.0f;
      pathSampleRadius = jerboaManager.PathSampleRadius;
      groupSpreadAngle = jerboaManager.GroupSpreadAngle;
      groupRayCount = jerboaManager.GroupRayCount;
      groupRayDistance = jerboaManager.GroupRayDistance;
      groupRayDistanceSquared = groupRayDistance * groupRayDistance;
      groupWalkNavigationMask = jerboaManager.GroupWalkNavigationMask;
      audioSource = GetComponent<AudioSource>();
      audioSource.outputAudioMixerGroup = jerboaManager.AudioMixerGroup;
      PrepareRays();
      agent.areaMask = groupWalkNavigationMask;
    }

    public void SetRandomPosition(JerboaInstanceDescription instance)
    {
      instance.GroupTriangleIndex = Random.Range(0, groupRayCount);
      instance.GroupTriangleIndexNext = (instance.GroupTriangleIndex + 1) % groupRayCount;
      instance.GroupBarycentricA = Random.value;
      instance.GroupBarycentricB = Random.value;
      if (instance.GroupBarycentricA + (double) instance.GroupBarycentricB <= 1.0)
        return;
      float groupBarycentricA = instance.GroupBarycentricA;
      instance.GroupBarycentricA = 1f - instance.GroupBarycentricB;
      instance.GroupBarycentricB = 1f - groupBarycentricA;
    }

    public Vector3 GetWorldPosition(JerboaInstanceDescription position)
    {
      int groupTriangleIndex = position.GroupTriangleIndex;
      Vector3 position1 = transform.position;
      Vector3 vector3_1 = transform.position + raysInWorldSpace[position.GroupTriangleIndex] * rayDistances[position.GroupTriangleIndex];
      Vector3 vector3_2 = transform.position + raysInWorldSpace[position.GroupTriangleIndexNext] * rayDistances[position.GroupTriangleIndexNext];
      float num1 = Random.value;
      float num2 = Random.value;
      return position1 + (vector3_1 - position1) * position.GroupBarycentricA + (vector3_2 - position1) * position.GroupBarycentricB;
    }

    private void PrepareRays()
    {
      rayDistances = new float[groupRayCount];
      raysInWorldSpace = new Vector3[groupRayCount];
      for (int index = 0; index < groupRayCount; ++index)
      {
        float num = (float) (index * (double) groupSpreadAngle / (groupRayCount - 1) - groupSpreadAngle / 2.0);
        raysInWorldSpace[index] = new Vector3(Mathf.Sin(num * ((float) Math.PI / 180f)), 0.0f, Mathf.Cos(num * ((float) Math.PI / 180f)));
        rayDistances[index] = pathSampleRadius;
      }
    }

    public void AddInstance(JerboaInstanceDescription instance)
    {
      SetRandomPosition(instance);
      instances.Add(instance);
      instance.Position = GetWorldPosition(instance);
      instance.Rotation = Quaternion.Euler(new Vector3(0.0f, Random.Range(0.0f, 360f), 0.0f));
      instance.Group = this;
    }

    private void UpdateSoundClips()
    {
      if (!Aloud)
      {
        AudioClip = null;
      }
      else
      {
        float num1 = 0.0f;
        foreach (JerboaInstanceDescription instance in instances)
        {
          if ((instance.Position - transform.position).sqrMagnitude < 4.0)
            ++num1;
        }
        float num2 = jerboaManager.Weight * jerboaManager.Quality;
        float num3 = num1 * num2;
        if (num3 < 7.5)
          AudioClip = null;
        else if (num3 < 15.0)
          AudioClip = jerboaManager.GetJerboaAudioclip(0);
        else if (num3 < 25.0)
          AudioClip = jerboaManager.GetJerboaAudioclip(1);
        else
          AudioClip = jerboaManager.GetJerboaAudioclip(2);
      }
    }

    private void UpdateSounds()
    {
      if (audioSource == null || audioSource.clip == null)
        return;
      volume = !Aloud ? Mathf.MoveTowards(volume, 0.0f, Time.deltaTime / 2f) : Mathf.MoveTowards(volume, 1f, Time.deltaTime / 2f);
      audioSource.volume = volume;
      if (audioSource.enabled)
      {
        if (audioSource.isPlaying)
        {
          if (volume < 0.05000000074505806)
            audioSource.Stop();
        }
        else if (volume > 0.05000000074505806)
          audioSource.PlayAndCheck();
      }
      UpdateAudioSourceEnable();
    }

    private void UpdateAudioSourceEnable()
    {
      bool flag = (transform.position - EngineApplication.PlayerPosition).sqrMagnitude < (double) (audioSource.maxDistance * audioSource.maxDistance);
      if (audioSource.enabled == flag)
        return;
      audioSource.enabled = flag;
    }

    private void Update()
    {
      UpdateRaysLazy();
      UpdateSounds();
      if (Random.value < Time.deltaTime / 1.0)
        UpdateSoundClips();
      if (agent.isOnNavMesh && !agent.hasPath && !agent.pathPending)
        TryCreateRandomPath(agent);
      int num1 = (int) (instances.Count * 0.10000000149011612 + 1.0);
      float num2 = 10f * Time.deltaTime / num1;
      for (int updateIndex = this.updateIndex; updateIndex < this.updateIndex + num1; ++updateIndex)
      {
        JerboaInstanceDescription instance = instances[updateIndex % instances.Count];
        Vector3 forward = GetWorldPosition(instance) - instance.Position;
        float sqrMagnitude = forward.sqrMagnitude;
        if (sqrMagnitude > (double) groupRayDistanceSquared)
        {
          if (Random.value < (double) num2)
          {
            instance.Respawn();
          }
          else
          {
            Quaternion to = Quaternion.LookRotation(forward);
            instance.Rotation = Quaternion.RotateTowards(instance.Rotation, to, (float) (40.0 * Time.deltaTime / 0.10000000149011612));
          }
        }
        else if (sqrMagnitude > 4.0)
        {
          Quaternion to = Quaternion.LookRotation(forward);
          instance.Rotation = Quaternion.RotateTowards(instance.Rotation, to, 40f * Time.deltaTime);
        }
      }
      this.updateIndex = (this.updateIndex + num1) % instances.Count;
    }

    private void TryCreateRandomPath(NavMeshAgent agent)
    {
      Vector2 insideUnitCircle = Random.insideUnitCircle;
      if (!NavMesh.SamplePosition(agent.gameObject.transform.position + new Vector3(insideUnitCircle.x, 0.0f, insideUnitCircle.y) * pathSampleRadius, out NavMeshHit hit, 2f, groupWalkNavigationMask))
        return;
      agent.destination = hit.position;
    }

    public bool TryGroupTeleport(Vector3 center, float radius)
    {
      float f = Random.Range(0.0f, 6.28318548f);
      if (!NavMesh.SamplePosition(center + new Vector3(Mathf.Sin(f), 0.0f, Mathf.Cos(f)) * radius, out NavMeshHit hit, 3f, groupWalkNavigationMask))
        return false;
      agent.Warp(hit.position);
      UpdateRays();
      return true;
    }

    private void UpdateRays()
    {
      for (int index = 0; index < groupRayCount; ++index)
      {
        rayDistances[index] = !NavMesh.Raycast(transform.position, transform.position + raysInWorldSpace[index] * groupRayDistance, out NavMeshHit hit, groupWalkNavigationMask) ? groupRayDistance : hit.distance;
      }
    }

    private void UpdateRaysLazy()
    {
      lastRayIndex = (lastRayIndex + 1) % groupRayCount;
      if (NavMesh.Raycast(transform.position, transform.position + raysInWorldSpace[lastRayIndex] * groupRayDistance, out NavMeshHit hit, groupWalkNavigationMask))
        rayDistances[lastRayIndex] = hit.distance;
      else
        rayDistances[lastRayIndex] = groupRayDistance;
    }

    private void OnDrawGizmos()
    {
      Gizmos.color = Color.red;
      for (int index = 0; index < raysInWorldSpace.Length; ++index)
      {
        Vector3 from = transform.position + 0.5f * Vector3.up;
        Gizmos.DrawLine(from, from + raysInWorldSpace[index] * rayDistances[index]);
      }
    }
  }
}
