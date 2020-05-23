using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace GG.Utils.InRangeView
{
    public class Unit : MonoBehaviour, IPointerDownHandler, IDragHandler
    {
        public static event Action<Unit> OnSelectUnit;

        public event Action OnPositionUpdate;

        [SerializeField, Range(2, 20)]
        protected float _range;

        public virtual float Range { get => _range; }

        [SerializeField]
        private Camera _camera;

        Vector3 _offsetFromDragStart;

        private Plane _dragPlane;

        private Vector3 _offsetFromPlane;

        private void Start()
        {
            _offsetFromPlane = Vector3.up * 0.5f;
            _dragPlane = new Plane(Vector3.up, transform.position - _offsetFromPlane);
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            OnSelectUnit?.Invoke(this);
            Vector3 worldPosition = GetWorldPosition(eventData.position);
            _offsetFromDragStart = transform.position - worldPosition;
        }

        public virtual void OnDrag(PointerEventData eventData)
        {
            Vector3 worldPosition = GetWorldPosition(eventData.position);
            transform.position = worldPosition + _offsetFromDragStart;
            OnPositionUpdate?.Invoke();
        }

        private Vector3 GetWorldPosition(Vector3 screenPosition)
        {
            Ray ray = _camera.ScreenPointToRay(screenPosition);
            float rayDistance;
            Vector3 result = transform.position;
            if (_dragPlane.Raycast(ray, out rayDistance))
            {
                result = ray.GetPoint(rayDistance) + _offsetFromPlane;
            }
            return result;
        }
    }
}
