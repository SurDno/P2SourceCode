using System;

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
    private float transitionDuration;
    private bool isInTransition;
    private float transitionTimer;
    [NonSerialized]
    public float transitionProgress;
    private int eventIndex = -1;
    private WrapMode wrapMode;
    private float speedParameter = 1f;
    private float cacheParameter = 1f;
    private AnimationEvent aniEvent;

    public WrapMode Mode
    {
      get => wrapMode;
      set => wrapMode = value;
    }

    public void Respawn()
    {
      Group.SetRandomPosition(this);
      Position = Group.GetWorldPosition(this);
    }

    public bool IsLoop() => Mode == WrapMode.Loop;

    public bool IsPause() => speedParameter == 0.0;

    public void PlayAnimation(string name)
    {
      PlayAnimation(Source.FindAnimationInfo(name.GetHashCode()));
    }

    public void PlayAnimation(int animationIndex)
    {
      if (Source.aniInfo == null || animationIndex == aniIndex && !IsPause())
        return;
      transitionDuration = 0.0f;
      transitionProgress = 1f;
      isInTransition = false;
      Debug.Assert(animationIndex < Source.aniInfo.Length);
      if (0 <= animationIndex && animationIndex < Source.aniInfo.Length)
      {
        preAniIndex = aniIndex;
        aniIndex = animationIndex;
        preAniFrame = (int) (curFrame + 0.5);
        curFrame = (float) UnityEngine.Random.Range(0, Source.aniInfo[aniIndex].totalFrame);
        eventIndex = -1;
        preAniTextureIndex = aniTextureIndex;
        aniTextureIndex = Source.aniInfo[aniIndex].textureIndex;
        wrapMode = Source.aniInfo[aniIndex].wrapMode;
        speedParameter = 1f;
      }
      else
        Debug.LogWarning((object) "The requested animation index is out of the count.");
    }

    public void CrossFade(string animationName, float duration)
    {
      CrossFade(Source.FindAnimationInfo(animationName.GetHashCode()), duration);
    }

    public void CrossFade(int animationIndex, float duration)
    {
      PlayAnimation(animationIndex);
      if (duration > 0.0)
      {
        isInTransition = true;
        transitionTimer = 0.0f;
        transitionProgress = 0.0f;
      }
      else
        transitionProgress = 1f;
      transitionDuration = duration;
    }

    public void UpdateAnimation()
    {
      if (Source.aniInfo == null || IsPause())
        return;
      if (isInTransition)
      {
        transitionTimer += Time.deltaTime;
        transitionProgress = Mathf.Min(transitionTimer / transitionDuration, 1f);
        if (transitionProgress >= 1.0)
        {
          isInTransition = false;
          preAniIndex = -1;
          preAniFrame = -1f;
        }
      }
      curFrame += Source.playSpeed * speedParameter * Time.deltaTime * Source.aniInfo[aniIndex].fps;
      int totalFrame = Source.aniInfo[aniIndex].totalFrame;
      switch (wrapMode)
      {
        case WrapMode.Default:
        case WrapMode.Once:
          if (curFrame < 0.0 || curFrame > totalFrame - 1.0)
          {
            Pause();
          }
          break;
        case WrapMode.Loop:
          if (curFrame < 0.0)
          {
            curFrame += totalFrame - 1;
            break;
          }
          if (curFrame > (double) (totalFrame - 1))
          {
            curFrame -= totalFrame - 1;
          }
          break;
        case WrapMode.PingPong:
          if (curFrame < 0.0)
          {
            speedParameter = Mathf.Abs(speedParameter);
            curFrame = Mathf.Abs(curFrame);
            break;
          }
          if (curFrame > (double) (totalFrame - 1))
          {
            speedParameter = -Mathf.Abs(speedParameter);
            curFrame = 2 * (totalFrame - 1) - curFrame;
          }
          break;
      }
      curFrame = Mathf.Clamp(curFrame, 0.0f, (float) (totalFrame - 1));
      UpdateAnimationEvent();
    }

    public void Pause()
    {
      cacheParameter = speedParameter;
      speedParameter = 0.0f;
    }

    public void Resume() => speedParameter = cacheParameter;

    public void Stop()
    {
      aniIndex = -1;
      preAniIndex = -1;
      eventIndex = -1;
      curFrame = 0.0f;
    }

    public bool IsPlaying() => aniIndex >= 0;

    public bool IsReady() => Source.aniInfo != null;

    public AnimationInfo GetCurrentAnimationInfoUnsafe() => Source.aniInfo[aniIndex];

    public AnimationInfo GetCurrentAnimationInfo()
    {
      return Source.aniInfo != null && 0 <= aniIndex && aniIndex < Source.aniInfo.Length ? Source.aniInfo[aniIndex] : null;
    }

    public AnimationInfo GetPreAnimationInfo()
    {
      return Source.aniInfo != null && 0 <= preAniIndex && preAniIndex < Source.aniInfo.Length ? Source.aniInfo[preAniIndex] : null;
    }

    public bool UpdateAnimationUnsafeNoEventsNoTransitions(float deltaTime)
    {
      curFrame += Source.playSpeed * speedParameter * deltaTime * Source.aniInfo[aniIndex].fps;
      int totalFrame = Source.aniInfo[aniIndex].totalFrame;
      if (curFrame <= (double) (totalFrame - 1))
        return false;
      curFrame -= totalFrame - 1;
      return true;
    }

    public void UpdateAnimationUnsafeNoEvents(float deltaTime)
    {
      if (isInTransition)
      {
        transitionTimer += deltaTime;
        transitionProgress = Mathf.Min(transitionTimer / transitionDuration, 1f);
        if (transitionProgress >= 1.0)
        {
          isInTransition = false;
          preAniIndex = -1;
          preAniFrame = -1f;
        }
      }
      curFrame += Source.playSpeed * speedParameter * deltaTime * Source.aniInfo[aniIndex].fps;
      int totalFrame = Source.aniInfo[aniIndex].totalFrame;
      switch (wrapMode)
      {
        case WrapMode.Default:
        case WrapMode.Once:
          if (curFrame >= 0.0 && curFrame <= totalFrame - 1.0)
            break;
          Pause();
          break;
        case WrapMode.Loop:
          if (curFrame < 0.0)
          {
            curFrame += totalFrame - 1;
            break;
          }
          if (curFrame <= (double) (totalFrame - 1))
            break;
          curFrame -= totalFrame - 1;
          break;
        case WrapMode.PingPong:
          if (curFrame < 0.0)
          {
            speedParameter = Mathf.Abs(speedParameter);
            curFrame = Mathf.Abs(curFrame);
            break;
          }
          if (curFrame <= (double) (totalFrame - 1))
            break;
          speedParameter = -Mathf.Abs(speedParameter);
          curFrame = 2 * (totalFrame - 1) - curFrame;
          break;
      }
    }

    private void UpdateAnimationEvent()
    {
      AnimationInfo currentAnimationInfo = GetCurrentAnimationInfo();
      if (currentAnimationInfo == null || currentAnimationInfo.eventList.Count == 0)
        return;
      if (aniEvent == null)
      {
        float num = curFrame / currentAnimationInfo.fps;
        for (int eventIndex = this.eventIndex >= 0 ? this.eventIndex : 0; eventIndex < currentAnimationInfo.eventList.Count; ++eventIndex)
        {
          if (currentAnimationInfo.eventList[eventIndex].time > (double) num)
          {
            aniEvent = currentAnimationInfo.eventList[eventIndex];
            this.eventIndex = eventIndex;
            break;
          }
        }
      }
      if (aniEvent == null || aniEvent.time > (double) (curFrame / currentAnimationInfo.fps))
        return;
      Source.gameObject.SendMessage(aniEvent.function, (object) aniEvent);
      aniEvent = null;
    }
  }
}
