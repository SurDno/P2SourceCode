// Decompiled with JetBrains decompiler
// Type: JerboaAnimationInstancing.JerboaInstanceDescription
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using System;
using UnityEngine;

#nullable disable
namespace JerboaAnimationInstancing
{
  public class JerboaInstanceDescription
  {
    public Vector3 Position;
    public Quaternion Rotation = Quaternion.identity;
    public BoundingSphere BoundingSphere = new BoundingSphere(new Vector3(0.0f, 0.0f, 0.0f), 0.5f);
    public bool visible = true;
    public JerboaInstance Source;
    public JerboaGroupBarycentric Group;
    public float GroupBarycentricA;
    public float GroupBarycentricB;
    public int GroupTriangleIndex;
    public int GroupTriangleIndexNext;
    [NonSerialized]
    public float curFrame;
    [NonSerialized]
    public float preAniFrame;
    [NonSerialized]
    public int aniIndex = -1;
    [NonSerialized]
    public int preAniIndex = -1;
    [NonSerialized]
    public int aniTextureIndex = -1;
    private int preAniTextureIndex = -1;
    private float transitionDuration = 0.0f;
    private bool isInTransition = false;
    private float transitionTimer = 0.0f;
    [NonSerialized]
    public float transitionProgress = 0.0f;
    private int eventIndex = -1;
    private WrapMode wrapMode;
    private float speedParameter = 1f;
    private float cacheParameter = 1f;
    private AnimationEvent aniEvent = (AnimationEvent) null;

    public WrapMode Mode
    {
      get => this.wrapMode;
      set => this.wrapMode = value;
    }

    public void Respawn()
    {
      this.Group.SetRandomPosition(this);
      this.Position = this.Group.GetWorldPosition(this);
    }

    public bool IsLoop() => this.Mode == WrapMode.Loop;

    public bool IsPause() => (double) this.speedParameter == 0.0;

    public void PlayAnimation(string name)
    {
      this.PlayAnimation(this.Source.FindAnimationInfo(name.GetHashCode()));
    }

    public void PlayAnimation(int animationIndex)
    {
      if (this.Source.aniInfo == null || animationIndex == this.aniIndex && !this.IsPause())
        return;
      this.transitionDuration = 0.0f;
      this.transitionProgress = 1f;
      this.isInTransition = false;
      Debug.Assert(animationIndex < this.Source.aniInfo.Length);
      if (0 <= animationIndex && animationIndex < this.Source.aniInfo.Length)
      {
        this.preAniIndex = this.aniIndex;
        this.aniIndex = animationIndex;
        this.preAniFrame = (float) (int) ((double) this.curFrame + 0.5);
        this.curFrame = (float) UnityEngine.Random.Range(0, this.Source.aniInfo[this.aniIndex].totalFrame);
        this.eventIndex = -1;
        this.preAniTextureIndex = this.aniTextureIndex;
        this.aniTextureIndex = this.Source.aniInfo[this.aniIndex].textureIndex;
        this.wrapMode = this.Source.aniInfo[this.aniIndex].wrapMode;
        this.speedParameter = 1f;
      }
      else
        Debug.LogWarning((object) "The requested animation index is out of the count.");
    }

    public void CrossFade(string animationName, float duration)
    {
      this.CrossFade(this.Source.FindAnimationInfo(animationName.GetHashCode()), duration);
    }

    public void CrossFade(int animationIndex, float duration)
    {
      this.PlayAnimation(animationIndex);
      if ((double) duration > 0.0)
      {
        this.isInTransition = true;
        this.transitionTimer = 0.0f;
        this.transitionProgress = 0.0f;
      }
      else
        this.transitionProgress = 1f;
      this.transitionDuration = duration;
    }

    public void UpdateAnimation()
    {
      if (this.Source.aniInfo == null || this.IsPause())
        return;
      if (this.isInTransition)
      {
        this.transitionTimer += Time.deltaTime;
        this.transitionProgress = Mathf.Min(this.transitionTimer / this.transitionDuration, 1f);
        if ((double) this.transitionProgress >= 1.0)
        {
          this.isInTransition = false;
          this.preAniIndex = -1;
          this.preAniFrame = -1f;
        }
      }
      this.curFrame += this.Source.playSpeed * this.speedParameter * Time.deltaTime * (float) this.Source.aniInfo[this.aniIndex].fps;
      int totalFrame = this.Source.aniInfo[this.aniIndex].totalFrame;
      switch (this.wrapMode)
      {
        case WrapMode.Default:
        case WrapMode.Once:
          if ((double) this.curFrame < 0.0 || (double) this.curFrame > (double) totalFrame - 1.0)
          {
            this.Pause();
            break;
          }
          break;
        case WrapMode.Loop:
          if ((double) this.curFrame < 0.0)
          {
            this.curFrame += (float) (totalFrame - 1);
            break;
          }
          if ((double) this.curFrame > (double) (totalFrame - 1))
          {
            this.curFrame -= (float) (totalFrame - 1);
            break;
          }
          break;
        case WrapMode.PingPong:
          if ((double) this.curFrame < 0.0)
          {
            this.speedParameter = Mathf.Abs(this.speedParameter);
            this.curFrame = Mathf.Abs(this.curFrame);
            break;
          }
          if ((double) this.curFrame > (double) (totalFrame - 1))
          {
            this.speedParameter = -Mathf.Abs(this.speedParameter);
            this.curFrame = (float) (2 * (totalFrame - 1)) - this.curFrame;
            break;
          }
          break;
      }
      this.curFrame = Mathf.Clamp(this.curFrame, 0.0f, (float) (totalFrame - 1));
      this.UpdateAnimationEvent();
    }

    public void Pause()
    {
      this.cacheParameter = this.speedParameter;
      this.speedParameter = 0.0f;
    }

    public void Resume() => this.speedParameter = this.cacheParameter;

    public void Stop()
    {
      this.aniIndex = -1;
      this.preAniIndex = -1;
      this.eventIndex = -1;
      this.curFrame = 0.0f;
    }

    public bool IsPlaying() => this.aniIndex >= 0;

    public bool IsReady() => this.Source.aniInfo != null;

    public AnimationInfo GetCurrentAnimationInfoUnsafe() => this.Source.aniInfo[this.aniIndex];

    public AnimationInfo GetCurrentAnimationInfo()
    {
      return this.Source.aniInfo != null && 0 <= this.aniIndex && this.aniIndex < this.Source.aniInfo.Length ? this.Source.aniInfo[this.aniIndex] : (AnimationInfo) null;
    }

    public AnimationInfo GetPreAnimationInfo()
    {
      return this.Source.aniInfo != null && 0 <= this.preAniIndex && this.preAniIndex < this.Source.aniInfo.Length ? this.Source.aniInfo[this.preAniIndex] : (AnimationInfo) null;
    }

    public bool UpdateAnimationUnsafeNoEventsNoTransitions(float deltaTime)
    {
      this.curFrame += this.Source.playSpeed * this.speedParameter * deltaTime * (float) this.Source.aniInfo[this.aniIndex].fps;
      int totalFrame = this.Source.aniInfo[this.aniIndex].totalFrame;
      if ((double) this.curFrame <= (double) (totalFrame - 1))
        return false;
      this.curFrame -= (float) (totalFrame - 1);
      return true;
    }

    public void UpdateAnimationUnsafeNoEvents(float deltaTime)
    {
      if (this.isInTransition)
      {
        this.transitionTimer += deltaTime;
        this.transitionProgress = Mathf.Min(this.transitionTimer / this.transitionDuration, 1f);
        if ((double) this.transitionProgress >= 1.0)
        {
          this.isInTransition = false;
          this.preAniIndex = -1;
          this.preAniFrame = -1f;
        }
      }
      this.curFrame += this.Source.playSpeed * this.speedParameter * deltaTime * (float) this.Source.aniInfo[this.aniIndex].fps;
      int totalFrame = this.Source.aniInfo[this.aniIndex].totalFrame;
      switch (this.wrapMode)
      {
        case WrapMode.Default:
        case WrapMode.Once:
          if ((double) this.curFrame >= 0.0 && (double) this.curFrame <= (double) totalFrame - 1.0)
            break;
          this.Pause();
          break;
        case WrapMode.Loop:
          if ((double) this.curFrame < 0.0)
          {
            this.curFrame += (float) (totalFrame - 1);
            break;
          }
          if ((double) this.curFrame <= (double) (totalFrame - 1))
            break;
          this.curFrame -= (float) (totalFrame - 1);
          break;
        case WrapMode.PingPong:
          if ((double) this.curFrame < 0.0)
          {
            this.speedParameter = Mathf.Abs(this.speedParameter);
            this.curFrame = Mathf.Abs(this.curFrame);
            break;
          }
          if ((double) this.curFrame <= (double) (totalFrame - 1))
            break;
          this.speedParameter = -Mathf.Abs(this.speedParameter);
          this.curFrame = (float) (2 * (totalFrame - 1)) - this.curFrame;
          break;
      }
    }

    private void UpdateAnimationEvent()
    {
      AnimationInfo currentAnimationInfo = this.GetCurrentAnimationInfo();
      if (currentAnimationInfo == null || currentAnimationInfo.eventList.Count == 0)
        return;
      if (this.aniEvent == null)
      {
        float num = this.curFrame / (float) currentAnimationInfo.fps;
        for (int eventIndex = this.eventIndex >= 0 ? this.eventIndex : 0; eventIndex < currentAnimationInfo.eventList.Count; ++eventIndex)
        {
          if ((double) currentAnimationInfo.eventList[eventIndex].time > (double) num)
          {
            this.aniEvent = currentAnimationInfo.eventList[eventIndex];
            this.eventIndex = eventIndex;
            break;
          }
        }
      }
      if (this.aniEvent == null || (double) this.aniEvent.time > (double) (this.curFrame / (float) currentAnimationInfo.fps))
        return;
      this.Source.gameObject.SendMessage(this.aniEvent.function, (object) this.aniEvent);
      this.aniEvent = (AnimationEvent) null;
    }
  }
}
