using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using UnityEngine.UI;

namespace UIWidgets {
    /// <summary>
    /// Sidebar handle.
    /// </summary>
    /// [System.Serializable]
	public class PointerUnityEvent : UnityEvent<PointerEventData> { }

    public class SidebarHandle : MonoBehaviour, IBeginDragHandler, IEndDragHandler, IDragHandler {
        public ScrollRect scrollrect;
        /// <summary>
        /// BeginDrag event.
        /// </summary>
        public PointerUnityEvent BeginDragEvent = new PointerUnityEvent();

        /// <summary>
        /// EndDrag event.
        /// </summary>
        public PointerUnityEvent EndDragEvent = new PointerUnityEvent();

        /// <summary>
        /// Drag event.
        /// </summary>
        public PointerUnityEvent DragEvent = new PointerUnityEvent();

        /// <summary>
        /// Called by a BaseInputModule before a drag is started.
        /// </summary>
        /// <param name="eventData">Event data.</param>
        public void OnBeginDrag(PointerEventData eventData) {
            transform.parent.parent.parent.parent.parent.GetComponent<ScrollRect>().OnBeginDrag(eventData);
        }

        /// <summary>
        /// Called by a BaseInputModule when a drag is ended.
        /// </summary>
        /// <param name="eventData">Event data.</param>
        public void OnEndDrag(PointerEventData eventData) {
            transform.parent.parent.parent.parent.parent.GetComponent<ScrollRect>().OnEndDrag(eventData);
            EndDragEvent.Invoke(eventData);
        }

        /// <summary>
        /// When draging is occuring this will be called every time the cursor is moved.
        /// </summary>
        /// <param name="eventData">Event data.</param>
        public void OnDrag(PointerEventData eventData) {
            transform.parent.parent.parent.parent.parent.GetComponent<ScrollRect>().OnDrag(eventData);
            if (eventData.pointerEnter == gameObject) {
                if (eventData.delta.y == 0) {
                    DragEvent.Invoke(eventData);
                }
            }
        }
    }
}