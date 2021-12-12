using System;
using ShootCommon.Views.Mediation;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Player.InputSystem
{
    public class PlayerInput : View, IBeginDragHandler, IEndDragHandler, IPointerDownHandler, IPointerUpHandler, IDragHandler
    {

        private Action<Vector2> _onInput;

        private Vector2 _directionalInput;
        

        private Vector2 _prevPointPos;
        private float _prevPointTime;


        public void Init(Action<Vector2> onInput)
        {
            _onInput = onInput;
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
           
        }

        public void OnEndDrag(PointerEventData eventData)
        {
          
        }

        public void OnPointerDown(PointerEventData eventData)
        {
           
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            
        }

        public void OnDrag(PointerEventData eventData)
        {
            if(eventData.delta.magnitude > 1)
                _onInput?.Invoke(eventData.delta);
        }
    }
}
