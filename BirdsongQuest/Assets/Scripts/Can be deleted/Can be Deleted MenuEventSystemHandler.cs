using DG.Tweening;
using System.Collections.Generic;
using Unity.Multiplayer.Center.Common;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class NewMonoBehaviourScript : MonoBehaviour
{
    [Header("References")]
    public List<Selectable> Selectables = new List<Selectable>();

    [Header("Animations")]
    [SerializeField] protected float _selectedAnimationScale = 1.1f;
    [SerializeField] protected float _scaleDuration = 0.25f;

    protected Dictionary<Selectable, Vector3> _scales = new Dictionary<Selectable, Vector3>();

    protected Tween _scaleUpTween;
    protected Tween _scaleDownTween;

    public virtual void Awake()
    {
        foreach (Selectable selectable in Selectables)
        {
            AddSelectionListeners(selectable);
            _scales.Add(selectable, selectable.transform.localScale);  
        }
    }

    public virtual void OnEnable()
    {
        //ensure all Selectables are reset back to original size
        foreach(Selectable selectable in Selectables)
        {
            selectable.transform.localScale = _scales[selectable];
        }
    }

    public virtual void OnDisable()
    {
        _scaleUpTween?.Kill(true);
        _scaleDownTween?.Kill(true);
    }


    protected virtual void AddSelectionListeners(Selectable selectable)
    {
        //add Listener
        EventTrigger trigger = selectable.GetComponent<EventTrigger>();
        if (trigger == null)
        {
            trigger = selectable.gameObject.AddComponent<EventTrigger>();
        }

        //add Select Event 
        EventTrigger.Entry SelectEntry = new EventTrigger.Entry
        {
            eventID = EventTriggerType.Select
        };
        SelectEntry.callback.AddListener(OnSelect);
        trigger.triggers.Add(SelectEntry);

        //add Deselect event
        EventTrigger.Entry DeselectEntry = new EventTrigger.Entry
        {
            eventID = EventTriggerType.Deselect
        };
        DeselectEntry.callback.AddListener(OnDeselect);
        trigger.triggers.Add(DeselectEntry);

        //add ONPOINTERENTER event
        EventTrigger.Entry PointerEnter = new EventTrigger.Entry
        {
            eventID = EventTriggerType.PointerEnter
        };
        PointerEnter.callback.AddListener(OnPointerEnter);
        trigger.triggers.Add(PointerEnter);

        //add ONPOINTEREXIT event
        EventTrigger.Entry PointerExit = new EventTrigger.Entry
        {
            eventID = EventTriggerType.PointerExit
        };
        PointerExit.callback.AddListener(OnPointerExit);
        trigger.triggers.Add(PointerExit);
    }

    public void OnSelect(BaseEventData eventData)
    {
        Vector3 newScale = eventData.selectedObject.transform.localScale * _selectedAnimationScale;
        _scaleUpTween = eventData.selectedObject.transform.DOScale(newScale, _scaleDuration);
    }
    public void OnDeselect(BaseEventData eventData)
    {
        Selectable selectable = eventData.selectedObject.GetComponent<Selectable>();
        Vector3 oldScale = _scales[selectable];
        _scaleDownTween = eventData.selectedObject.transform.DOScale(oldScale, _scaleDuration);
    }

    public void OnPointerEnter(BaseEventData eventData)
    {
        PointerEventData pointerEventData = (PointerEventData)eventData;
        if (pointerEventData != null)
        {
            Selectable selectable = pointerEventData.pointerEnter.GetComponentInParent<Selectable>();
            if(selectable == null)
            {
                selectable = pointerEventData.pointerEnter.GetComponentInChildren<Selectable>();
            }

            pointerEventData.selectedObject = selectable.gameObject;
        }
    }


    public void OnPointerExit(BaseEventData eventData)
    {
        PointerEventData pointerEventData = (PointerEventData)eventData;
        if (pointerEventData != null)
        {
            pointerEventData.selectedObject = null;
        }
    }

}
