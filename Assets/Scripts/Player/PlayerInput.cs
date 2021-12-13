using System;
using ShootCommon.Views.Mediation;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Player.InputSystem
{
    public class PlayerInput : View, IBeginDragHandler, IEndDragHandler, IPointerDownHandler, IPointerUpHandler, IDragHandler
    {

        [SerializeField] private Vector2 _sensitivity;
        
        private Action<Vector2> _onInput;

        private Vector2 _directionalInput;
        

        private Vector2 _prevPointPos;
        private float _prevPointTime;
        private bool _dragging;


        public void Init(Action<Vector2> onInput)
        {
            _onInput = onInput;
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            _dragging = true;
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            _dragging = false;
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            _prevPointTime = Time.time;
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            if (!_dragging && Time.time - _prevPointTime < 0.2f)
                _onInput?.Invoke(Vector2.up);
        }

        public void OnDrag(PointerEventData eventData)
        {
            if (Mathf.Abs(eventData.delta.y) >= _sensitivity.y || Mathf.Abs(eventData.delta.x) >= _sensitivity.x)
            {
                _onInput?.Invoke(new Vector2(Mathf.Clamp(eventData.delta.x / _sensitivity.x, -1, 1),
                    Mathf.Clamp(eventData.delta.y / _sensitivity.y, -1, 1)));
            }
        }
    }
}
