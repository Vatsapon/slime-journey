using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class EventManager : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler, IPointerUpHandler, IBeginDragHandler, IEndDragHandler, IInitializePotentialDragHandler {
    public UnityEvent onMouseEnter;
    public UnityEvent onMouseExit;
    public UnityEvent onMouseClick;
    public UnityEvent onPointerEnter;
    public UnityEvent onPointerExit;
    public UnityEvent onPointerClick;
    public UnityEvent onPointerUp;
    public UnityEvent onSliderDrag;
    public UnityEvent onSliderClick;
    public UnityEvent onSliderRelease;
    public UnityEvent onTriggerEnter;
    public List<GameObject> objectEnterList;
    public UnityEvent onTriggerExit;
    public List<GameObject> objectExitList;

    void OnMouseEnter() {
        onMouseEnter.Invoke();
    }

    void OnMouseExit() {
        onMouseExit.Invoke();
    }

    void OnMouseDown() {
        onMouseClick.Invoke();
    }

    public void OnPointerEnter(PointerEventData eventData) {
        onPointerEnter.Invoke();
    }

    public void OnPointerExit(PointerEventData eventData) {
        onPointerExit.Invoke();
    }

    public void OnPointerClick(PointerEventData eventData) {
        onPointerClick.Invoke();
    }

    public void OnPointerUp(PointerEventData eventData) {
        onPointerUp.Invoke();
    }

    public void OnTriggerEnter2D(Collider2D collision) {
        if (objectEnterList.Count > 0) {
            foreach (GameObject obj in objectEnterList) {
                if (collision.gameObject.Equals(obj)) {
                    onTriggerEnter.Invoke();
                }
            }
        }
        
    }

    public void OnTriggerExit2D(Collider2D collision) {
        if (objectExitList.Count > 0) {
            foreach (GameObject obj in objectExitList) {
                if (collision.gameObject.Equals(obj)) {
                    onTriggerExit.Invoke();
                }
            }
        }
    }

    public void OnBeginDrag(PointerEventData eventData) {
        onSliderDrag.Invoke();
    }

    public void OnEndDrag(PointerEventData eventData) {
        onSliderRelease.Invoke();
    }

    public void OnInitializePotentialDrag(PointerEventData eventData) {
        onSliderClick.Invoke();
    }
}
