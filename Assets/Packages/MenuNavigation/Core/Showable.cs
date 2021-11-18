using System;
using System.Threading.Tasks;
using UnityEngine;

namespace MenuNavigation {
    public abstract class Showable : MonoBehaviour {
        
        public static string ShowableName = "ScreenName";

        protected MenuNavigationController MenuNavigationController;

        public static async Task<T> Create<T>(MenuNavigationController menuNavigationController, Transform parent) where T : Showable
        {
            var showableName = typeof(T).GetField("ShowableName").GetValue(null).ToString();

            var asyncOperation = Resources.LoadAsync(showableName);

            while (!asyncOperation.isDone)
                await Task.Yield();

            var screen = (Instantiate(asyncOperation.asset, parent) as GameObject)?.GetComponent<T>();

            if (screen != null)
            {
                screen.MenuNavigationController = menuNavigationController;

                return screen;
            }

            Debug.LogError("Screen that you want to create - " + typeof(T) + " cant be created, check ShowableName parameter");

            return null;
        }
        
        protected virtual void Awake() {
        }
        
        public abstract void Show(Action onFinish);

        public abstract void Hide(Action onFinish);

        public abstract bool IsActive { get; set; }
        public abstract ShowableTransitionState TransitionState { get; protected set; }
    }

    public enum ShowableTransitionState {
        None,
        Showing,
        Shown,
        Hiding,
        Hidden,
    }
}