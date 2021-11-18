using System;
using UnityEngine;

namespace MenuNavigation {
    public abstract class Popup : Showable {

        protected RectTransform RectTransform;
        protected RectTransform MainPanel;
        protected bool IsTransitionsAvailable;

        private Vector3 _shownPosition;

        protected new virtual void Awake() {
            base.Awake();
            RectTransform = GetComponent<RectTransform>();
            if (transform.childCount < 1) {
                IsTransitionsAvailable = false;
                return;
            }

            MainPanel = transform.GetChild(0).GetComponent<RectTransform>();
            if (MainPanel == null) {
                IsTransitionsAvailable = false;
                return;
            }

            _shownPosition = MainPanel.anchoredPosition;
        }

        public override void Show(Action onFinish) {
            TransitionState = ShowableTransitionState.Shown;
            onFinish?.Invoke();
        }

        public override void Hide(Action onFinish) {
            TransitionState = ShowableTransitionState.Hidden;
            onFinish?.Invoke();
        }

        public override bool IsActive {
            get => gameObject.activeSelf;
            set => gameObject.SetActive(value);
        }

        public override ShowableTransitionState TransitionState { get; protected set; }
    }
}