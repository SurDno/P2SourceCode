using Engine.Common.BoundCharacters;
using Engine.Common.Commons;
using Engine.Common.Services;
using Engine.Source.Components.BoundCharacters;
using Engine.Source.Services;
using Engine.Source.Settings.External;
using InputServices;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace Engine.Source.UI.Menu.Protagonist.BoundCharacters
{
  public class BoundCharactersView : MonoBehaviour
  {
    [SerializeField]
    private BoundCharactersGroupView groupViewPrefab;
    [SerializeField]
    private RectTransform layout;
    [SerializeField]
    private BoundCharacterGroup[] groups;
    [SerializeField]
    [FormerlySerializedAs("animationTime")]
    private float singleAnimationTime = 1f;
    [SerializeField]
    private float allAnimationsTime = 3f;
    private List<BoundCharactersGroupView> groupViews = new List<BoundCharactersGroupView>();
    private List<BoundCharacterView> unseenGroupViews = new List<BoundCharacterView>();
    private List<BoundCharacterView> unseenStateViews = new List<BoundCharacterView>();
    private float animationTime = -1f;
    private float animationPhase = -1f;
    private Scrollbar _scrollbar;
    [SerializeField]
    private GameObject controlPanel;
    private float xPrev = 0.0f;

    private void Build()
    {
      BoundCharactersService service = ServiceLocator.GetService<BoundCharactersService>();
      if (service == null)
        return;
      Dictionary<BoundCharacterGroup, List<BoundCharacterComponent>> dictionary = new Dictionary<BoundCharacterGroup, List<BoundCharacterComponent>>();
      foreach (BoundCharacterComponent characterComponent in service.Items)
      {
        BoundCharacterGroup group = characterComponent.Group;
        List<BoundCharacterComponent> characterComponentList;
        if (!dictionary.TryGetValue(group, out characterComponentList))
        {
          characterComponentList = new List<BoundCharacterComponent>();
          dictionary.Add(group, characterComponentList);
        }
        characterComponentList.Add(characterComponent);
      }
      for (int index1 = 0; index1 < this.groups.Length; ++index1)
      {
        List<BoundCharacterComponent> characterComponentList;
        if (dictionary.TryGetValue(this.groups[index1], out characterComponentList))
        {
          characterComponentList.Sort(new Comparison<BoundCharacterComponent>(this.SortingComparison));
          BoundCharactersGroupView charactersGroupView = UnityEngine.Object.Instantiate<BoundCharactersGroupView>(this.groupViewPrefab, (Transform) this.layout, false);
          this.groupViews.Add(charactersGroupView);
          charactersGroupView.SetGroup(this.groups[index1]);
          for (int index2 = 0; index2 < characterComponentList.Count; ++index2)
          {
            BoundCharacterView boundCharacterView = charactersGroupView.AddCharacter(characterComponentList[index2]);
            if (!boundCharacterView.IsGroupSeen())
              this.unseenGroupViews.Add(boundCharacterView);
            if (!boundCharacterView.IsStateSeen())
              this.unseenStateViews.Add(boundCharacterView);
          }
        }
      }
      if (this.unseenGroupViews.Count > 0 || this.unseenStateViews.Count > 0)
      {
        this.animationTime = Mathf.Min(this.allAnimationsTime / (float) (this.unseenGroupViews.Count + this.unseenStateViews.Count), this.singleAnimationTime);
        this.animationPhase = this.singleAnimationTime * 0.5f;
      }
      ServiceLocator.GetService<NotificationService>().RemoveNotify(NotificationEnum.BoundCharacters);
      this._scrollbar = this.GetComponent<ScrollRect>().horizontalScrollbar;
    }

    private void Clear()
    {
      this.unseenGroupViews.Clear();
      this.unseenStateViews.Clear();
      foreach (Component groupView in this.groupViews)
        UnityEngine.Object.Destroy((UnityEngine.Object) groupView.gameObject);
      this.groupViews.Clear();
      this.animationPhase = -1f;
    }

    private void OnDisable()
    {
      this.Clear();
      InputService.Instance.onJoystickUsedChanged -= new Action<bool>(this.OnJoystick);
    }

    private void OnEnable()
    {
      this.Build();
      InputService.Instance.onJoystickUsedChanged += new Action<bool>(this.OnJoystick);
      this.OnJoystick(InputService.Instance.JoystickUsed);
    }

    private int SortingComparison(BoundCharacterComponent x, BoundCharacterComponent y)
    {
      return x.SortOrder - y.SortOrder;
    }

    private void UpdateScrollNavigation()
    {
      if (!InputService.Instance.JoystickUsed)
        return;
      Vector2 zero = Vector2.zero;
      float axis = InputService.Instance.GetAxis("LeftStickX");
      float num = (this.xPrev + axis) * Time.deltaTime;
      this.xPrev = axis;
      if ((double) num == 0.0)
        return;
      this._scrollbar.value += num * Time.deltaTime * ExternalSettingsInstance<ExternalInputSettings>.Instance.JoystickSensitivity;
    }

    private void OnJoystick(bool joystick) => this.controlPanel?.SetActive(joystick);

    private void Update()
    {
      this.UpdateScrollNavigation();
      if ((double) this.animationPhase == -1.0)
        return;
      this.animationPhase += Time.deltaTime;
      if ((double) this.animationPhase < (double) this.animationTime)
        return;
      this.animationPhase -= this.animationTime;
      if (this.unseenGroupViews.Count > 0)
      {
        int index = UnityEngine.Random.Range(0, this.unseenGroupViews.Count);
        this.unseenGroupViews[index].MakeGroupSeen();
        this.unseenGroupViews.RemoveAt(index);
      }
      else if (this.unseenStateViews.Count > 0)
      {
        int index = UnityEngine.Random.Range(0, this.unseenStateViews.Count);
        this.unseenStateViews[index].MakeStateSeen();
        this.unseenStateViews.RemoveAt(index);
      }
      else
        this.animationPhase = -1f;
    }
  }
}
