using Engine.Common.Services;
using Engine.Common.Types;
using Engine.Impl.Services;
using Engine.Impl.UI.Controls;
using Engine.Source.Services;
using Engine.Source.Services.Inputs;
using InputServices;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Engine.Source.UI.Menu.Protagonist.MindMap
{
  public class MMSideBar : MonoBehaviour
  {
    [SerializeField]
    private MMWindow mindMap;
    [SerializeField]
    private GameObject globalButton;
    [SerializeField]
    private GameObject pageButtonPrototype;
    [SerializeField]
    private RectTransform pageButtonsAnchor;
    private List<GameObject> pageButtons = new List<GameObject>();
    private int currentIndex = 0;

    private void OnDisable()
    {
      ServiceLocator.GetService<MMService>().ChangeUndiscoveredEvent -= new Action(this.UpdateUndiscovered);
      InputService.Instance.onJoystickUsedChanged -= new Action<bool>(this.OnJoystick);
      GameActionService service = ServiceLocator.GetService<GameActionService>();
      service.RemoveListener(GameActionType.DPadDown, new GameActionHandle(this.ChangeChapter));
      service.RemoveListener(GameActionType.DPadUp, new GameActionHandle(this.ChangeChapter));
    }

    private void OnEnable()
    {
      ServiceLocator.GetService<MMService>().ChangeUndiscoveredEvent += new Action(this.UpdateUndiscovered);
      InputService.Instance.onJoystickUsedChanged += new Action<bool>(this.OnJoystick);
      GameActionService service = ServiceLocator.GetService<GameActionService>();
      service.AddListener(GameActionType.DPadDown, new GameActionHandle(this.ChangeChapter));
      service.AddListener(GameActionType.DPadUp, new GameActionHandle(this.ChangeChapter));
      this.OnJoystick(InputService.Instance.JoystickUsed);
    }

    private void OnJoystick(bool joystick)
    {
      EventSystem.current.SetSelectedGameObject((GameObject) null);
      if (!joystick)
        return;
      EventSystem.current.SetSelectedGameObject(this.pageButtons[this.currentIndex]);
    }

    private bool ChangeChapter(GameActionType type, bool down)
    {
      if (type == GameActionType.DPadUp & down)
      {
        --this.currentIndex;
        this.ChangeSelection();
        return true;
      }
      if (!(type == GameActionType.DPadDown & down))
        return false;
      ++this.currentIndex;
      this.ChangeSelection();
      return true;
    }

    private void ChangeSelection()
    {
      int pageCount = this.mindMap.PageCount;
      if (this.mindMap.GlobalPage != null)
        ++pageCount;
      if (this.currentIndex > pageCount - 1)
        this.currentIndex = 0;
      if (this.currentIndex < 0)
        this.currentIndex = pageCount - 1;
      EventSystem.current.SetSelectedGameObject(this.currentIndex < this.mindMap.PageCount ? this.pageButtons[this.currentIndex] : this.globalButton);
      ExecuteEvents.Execute<ISubmitHandler>(this.currentIndex < this.mindMap.PageCount ? this.pageButtons[this.currentIndex] : this.globalButton, (BaseEventData) new PointerEventData(EventSystem.current), ExecuteEvents.submitHandler);
    }

    public void UpdateButtons()
    {
      if (this.mindMap.GlobalPage != null)
      {
        this.globalButton.GetComponent<Button>().interactable = this.mindMap.OpenedPage != this.mindMap.GlobalPage;
        string str = (string) null;
        LocalizedText title = this.mindMap.GlobalPage.Title;
        if (title != LocalizedText.Empty)
          str = ServiceLocator.GetService<LocalizationService>().GetText(title);
        if (string.IsNullOrEmpty(str))
          str = "Global";
        this.globalButton.GetComponent<Text>().text = str;
        this.globalButton.SetActive(true);
        if (this.mindMap.OpenedPage == this.mindMap.GlobalPage)
          this.currentIndex = this.mindMap.PageCount;
      }
      else
        this.globalButton.SetActive(false);
      int pageCount = this.mindMap.PageCount;
      MMPage openedPage = this.mindMap.OpenedPage;
      while (this.pageButtons.Count < pageCount)
      {
        int index = this.pageButtons.Count;
        GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(this.pageButtonPrototype);
        gameObject.transform.SetParent((Transform) this.pageButtonsAnchor, false);
        this.pageButtons.Add(gameObject);
        gameObject.GetComponent<Button>().onClick.AddListener((UnityAction) (() => this.OpenPage(index)));
      }
      for (int index = 0; index < this.pageButtons.Count; ++index)
      {
        if (index < pageCount)
        {
          MMPage page = this.mindMap.GetPage(index);
          this.pageButtons[index].GetComponent<Button>().interactable = this.mindMap.OpenedPage != page;
          string str = (string) null;
          LocalizedText title = page.Title;
          if (title != LocalizedText.Empty)
            str = ServiceLocator.GetService<LocalizationService>().GetText(title);
          if (string.IsNullOrEmpty(str))
            str = index.ToString();
          this.pageButtons[index].GetComponent<Text>().text = str;
          this.pageButtons[index].SetActive(true);
          if (this.mindMap.OpenedPage == page)
            this.currentIndex = index;
        }
        else
          this.pageButtons[index].SetActive(false);
      }
      this.UpdateUndiscovered();
    }

    public void OpenPage(int index)
    {
      this.currentIndex = index;
      this.mindMap.OpenPage(this.mindMap.GetPage(index));
    }

    public void OpenGlobal() => this.mindMap.OpenPage(this.mindMap.GlobalPage);

    public void UpdateUndiscovered()
    {
      if (this.mindMap.GlobalPage != null)
        this.globalButton.GetComponent<HideableView>().Visible = this.mindMap.GlobalPage.HasUndiscovered();
      int pageCount = this.mindMap.PageCount;
      for (int index = 0; index < pageCount; ++index)
        this.pageButtons[index].GetComponent<HideableView>().Visible = this.mindMap.GetPage(index).HasUndiscovered();
    }
  }
}
