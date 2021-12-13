using System;
using ShootCommon.Views.Mediation;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Player.InputSystem
{
    public class PlayerInput : View, IBeginDragHandler, IEndDragHandler, IPointerDownHandler, IPointerUpHandler, IDragHandler
    {

        [SerializeField] private Vector2 _sensitivity;
        [SerializeField] private float _clickTime = 0.2f;
        [SerializeField] private RectTransform _rect;
        
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
            _prevPointPos = eventData.position;
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            if (!_dragging && Time.time - _prevPointTime < _clickTime)
                _onInput?.Invoke(Vector2.up);
            
            // Debug.Log();
        }

        public void OnDrag(PointerEventData eventData)
        {
            if (Mathf.Abs((eventData.position.y - _prevPointPos.y) / _rect.rect.height) >= _sensitivity.y ||
                Mathf.Abs((eventData.position.x - _prevPointPos.x) / _rect.rect.width) >= _sensitivity.x)
            {
                _onInput?.Invoke(new Vector2(
                    Mathf.Clamp(((eventData.position.x - _prevPointPos.x)/ _rect.rect.width) / _sensitivity.x, -1, 1),
                    Mathf.Clamp(((eventData.position.y - _prevPointPos.y)/ _rect.rect.height) / _sensitivity.y, -1, 1)));
            }
        }
    }
}
