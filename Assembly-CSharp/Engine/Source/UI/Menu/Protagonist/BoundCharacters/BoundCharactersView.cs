using System.Collections.Generic;
using Engine.Common.BoundCharacters;
using Engine.Common.Commons;
using Engine.Common.Services;
using Engine.Source.Components.BoundCharacters;
using Engine.Source.Services;
using Engine.Source.Settings.External;
using InputServices;
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
    private float xPrev;

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
      for (int index1 = 0; index1 < groups.Length; ++index1)
      {
        List<BoundCharacterComponent> characterComponentList;
        if (dictionary.TryGetValue(groups[index1], out characterComponentList))
        {
          characterComponentList.Sort(SortingComparison);
          BoundCharactersGroupView charactersGroupView = Instantiate(groupViewPrefab, layout, false);
          groupViews.Add(charactersGroupView);
          charactersGroupView.SetGroup(groups[index1]);
          for (int index2 = 0; index2 < characterComponentList.Count; ++index2)
          {
            BoundCharacterView boundCharacterView = charactersGroupView.AddCharacter(characterComponentList[index2]);
            if (!boundCharacterView.IsGroupSeen())
              unseenGroupViews.Add(boundCharacterView);
            if (!boundCharacterView.IsStateSeen())
              unseenStateViews.Add(boundCharacterView);
          }
        }
      }
      if (unseenGroupViews.Count > 0 || unseenStateViews.Count > 0)
      {
        animationTime = Mathf.Min(allAnimationsTime / (unseenGroupViews.Count + unseenStateViews.Count), singleAnimationTime);
        animationPhase = singleAnimationTime * 0.5f;
      }
      ServiceLocator.GetService<NotificationService>().RemoveNotify(NotificationEnum.BoundCharacters);
      _scrollbar = GetComponent<ScrollRect>().horizontalScrollbar;
    }

    private void Clear()
    {
      unseenGroupViews.Clear();
      unseenStateViews.Clear();
      foreach (Component groupView in groupViews)
        Destroy(groupView.gameObject);
      groupViews.Clear();
      animationPhase = -1f;
    }

    private void OnDisable()
    {
      Clear();
      InputService.Instance.onJoystickUsedChanged -= OnJoystick;
    }

    private void OnEnable()
    {
      Build();
      InputService.Instance.onJoystickUsedChanged += OnJoystick;
      OnJoystick(InputService.Instance.JoystickUsed);
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
      float num = (xPrev + axis) * Time.deltaTime;
      xPrev = axis;
      if (num == 0.0)
        return;
      _scrollbar.value += num * Time.deltaTime * ExternalSettingsInstance<ExternalInputSettings>.Instance.JoystickSensitivity;
    }

    private void OnJoystick(bool joystick) => controlPanel?.SetActive(joystick);

    private void Update()
    {
      UpdateScrollNavigation();
      if (animationPhase == -1.0)
        return;
      animationPhase += Time.deltaTime;
      if (animationPhase < (double) animationTime)
        return;
      animationPhase -= animationTime;
      if (unseenGroupViews.Count > 0)
      {
        int index = Random.Range(0, unseenGroupViews.Count);
        unseenGroupViews[index].MakeGroupSeen();
        unseenGroupViews.RemoveAt(index);
      }
      else if (unseenStateViews.Count > 0)
      {
        int index = Random.Range(0, unseenStateViews.Count);
        unseenStateViews[index].MakeStateSeen();
        unseenStateViews.RemoveAt(index);
      }
      else
        animationPhase = -1f;
    }
  }
}
