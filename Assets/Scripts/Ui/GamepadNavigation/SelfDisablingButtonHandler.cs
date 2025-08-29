using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

[RequireComponent(typeof(Button))]
public class SelfDisablingButtonHandler : MonoBehaviour, ISubmitHandler
{
    [SerializeField] private Selectable pairedButton;
    [SerializeField] private Selectable self;
    [SerializeField] private bool selectPairedButton = false;

    public void OnSubmit(BaseEventData eventData)
    {
        if (selectPairedButton)
        {
            EventSystem.current.SetSelectedGameObject(pairedButton.gameObject);
        }
        else
        {
            FindNearbyButton();
        }
    }

    private void FindNearbyButton()
    {
        var eventSystem = EventSystem.current;

        Selectable target;
        // favour selection towards the center of the screen
        bool favourRight = Camera.main.WorldToScreenPoint(self.transform.position).x < Screen.width / 2;
        if (favourRight)
        {
            target = GetValidNeighbor(self.FindSelectableOnRight(), s => s.FindSelectableOnRight())
                      ?? GetValidNeighbor(self.FindSelectableOnLeft(), s => s.FindSelectableOnLeft());
        }
        else
        {
            target = GetValidNeighbor(self.FindSelectableOnLeft(), s => s.FindSelectableOnLeft())
                      ?? GetValidNeighbor(self.FindSelectableOnRight(), s => s.FindSelectableOnRight());
        }

        if (target != null)
            eventSystem.SetSelectedGameObject(target.gameObject);
    }

    private Selectable GetValidNeighbor(Selectable candidate, System.Func<Selectable, Selectable> nextStep)
    {
        if (candidate == null) return null;

        if (candidate == pairedButton)
            return GetValidNeighbor(nextStep(candidate), nextStep);

        if (!candidate.IsActive() || !candidate.IsInteractable())
            return null;

        return candidate;
    }

}
