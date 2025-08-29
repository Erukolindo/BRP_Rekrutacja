using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.UI;
using UnityEngine.UI;

[RequireComponent(typeof(ScrollRect))]
public class ScrollRectNavigationHelper : MonoBehaviour
{
    private ScrollRect scrollRect;
    private GameObject lastSelectedObject;

    void Awake()
    {
        scrollRect = GetComponent<ScrollRect>();

        foreach (var scrollbar in GetComponentsInChildren<Scrollbar>(true))
        {
            var nav = scrollbar.navigation;
            nav.mode = Navigation.Mode.None;
            scrollbar.navigation = nav;
        }
    }

    void OnEnable()
    {
        StartCoroutine(CheckCurrentSelectionDelayed());
    }

    void Update()
    {
        // Check if selection changed every frame
        var eventSystem = EventSystem.current;
        if (eventSystem != null)
        {
            var currentSelected = eventSystem.currentSelectedGameObject;
            if (currentSelected != lastSelectedObject)
            {
                lastSelectedObject = currentSelected;
                CheckCurrentSelection();
            }
        }
    }

    private IEnumerator CheckCurrentSelectionDelayed()
    {
        yield return null; // Wait one frame
        CheckCurrentSelection();
    }

    private void CheckCurrentSelection()
    {
        var eventSystem = EventSystem.current;
        if (eventSystem == null) return;

        var selected = eventSystem.currentSelectedGameObject;
        if (selected == null) return;
        if (!selected.transform.IsChildOf(scrollRect.content)) return;

        ScrollTo(selected.GetComponent<RectTransform>());
    }

    private void ScrollTo(RectTransform child)
    {
        Canvas.ForceUpdateCanvases();

        Vector2 viewportLocalPosition = scrollRect.viewport.localPosition;
        Vector2 childLocalPosition = child.localPosition;
        Vector2 result = new Vector2(0, 0 - (viewportLocalPosition.y + childLocalPosition.y));

        Vector2 originalPosition = scrollRect.content.localPosition;
        scrollRect.content.localPosition = result;

        float normalizedPosition = scrollRect.verticalNormalizedPosition;

        normalizedPosition = Mathf.Clamp01(normalizedPosition);

        scrollRect.content.localPosition = originalPosition;
        scrollRect.verticalNormalizedPosition = normalizedPosition;
    }
}
