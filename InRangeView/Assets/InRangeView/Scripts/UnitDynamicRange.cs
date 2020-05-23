using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace GG.Utils.InRangeView
{
    public class UnitDynamicRange : Unit, IPointerDownHandler, IDragHandler
    {
        public override float Range { get => _currentRange; }

        private float _currentRange;

        public override void OnDrag(PointerEventData eventData)
        {
            Vector3 previousPosition = transform.position;
            base.OnDrag(eventData);
            float distance = Vector3.Distance(previousPosition, transform.position);
            _currentRange -= distance;

            if (_currentRange <= 0)
            {
                _currentRange = _range;
            }
        }
    }
}
