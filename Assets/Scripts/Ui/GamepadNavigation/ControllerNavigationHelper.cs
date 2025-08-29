using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.UI;
using UnityEngine.UI;

[RequireComponent(typeof(InputSystemUIInputModule))]
public class ControllerNavigationHelper : MonoBehaviour
{
    #region singleton

    public static ControllerNavigationHelper Instance;

    private void Awake()
    {
        Instance = this;
        FirstSelectedUIElement.Select();

        uiInput = GetComponent<InputSystemUIInputModule>();

        uiInput.move.action.performed += TryRecoverSelection;
        uiInput.submit.action.performed += TryRecoverSelection;
    }

    #endregion

    public Selectable FirstSelectedUIElement;
    private InputSystemUIInputModule uiInput;

    private Selectable[] currentlySelectedScope = null;
    [HideInInspector] public Transform currentSelectionRoot = null;

    void OnDestroy()
    {
        uiInput.move.action.performed -= TryRecoverSelection;
        uiInput.submit.action.performed -= TryRecoverSelection;
    }

    public void SetScope(Transform newScopeRoot, bool selectFirstObject = true) 
    {
        Debug.Log("setting new scope: " + (newScopeRoot != null ? newScopeRoot.name : "null"));

        if (currentlySelectedScope == null)
        {
            SetNavigation(Selectable.allSelectablesArray, false);
        }
        else
        {
            SetNavigation(currentlySelectedScope, false);
        }

        if (newScopeRoot != null)
        {
            var newScope = newScopeRoot.GetComponentsInChildren<Selectable>(true);
            SetNavigation(newScope, true);
            currentlySelectedScope = newScope;
            currentSelectionRoot = newScopeRoot;

            if (selectFirstObject && newScope.Length > 0)
            {
                var first = newScope[0];
                if (first != null)
                    EventSystem.current.SetSelectedGameObject(first.gameObject);
            }
        }
        else
        {
            SetNavigation(Selectable.allSelectablesArray, true);
            currentlySelectedScope = null;
            currentSelectionRoot = null;
        }
    }

    private void SetNavigation(Selectable[] scope, bool enable)
    {
        foreach (var selectable in scope)
        {
            if(selectable == null || selectable is Scrollbar) continue;

            if (enable)
                selectable.navigation = Navigation.defaultNavigation;
            else
                selectable.navigation = new Navigation { mode = Navigation.Mode.None };
        }
    }

    private void TryRecoverSelection(InputAction.CallbackContext ctx)
    {
        var eventSystem = EventSystem.current;
        if (eventSystem == null) return;

        if (eventSystem.currentSelectedGameObject == null || !eventSystem.currentSelectedGameObject.activeInHierarchy)
        {
            Selectable fallback = FindFirstSelectable();
            if (fallback != null)
                eventSystem.SetSelectedGameObject(fallback.gameObject);

            // Optional: here you could *replay* the input if you want
            // e.g. simulate a Submit if this was a submit input
        }
    }

    private Selectable FindFirstSelectable()
    {
        foreach (Selectable selectable in Selectable.allSelectablesArray)
        {
            if (selectable == null) continue;
            if (!selectable.IsActive() || !selectable.IsInteractable()) continue;

            var nav = selectable.navigation;
            if (nav.mode == Navigation.Mode.None) continue;

            return selectable;
        }
        return null;
    }
}
