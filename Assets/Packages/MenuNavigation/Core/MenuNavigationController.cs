using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

namespace MenuNavigation {
    public class MenuNavigationController : IMenuNavigationController
    {

        private readonly Transform _menuScreensParent;
        private readonly Transform _popupsParent;
        private readonly Transform _screenElementsPoolParent;
        private readonly CanvasGroup _canvasGroup;

        private readonly Dictionary<Type, Popup> _popups = new Dictionary<Type, Popup>();
        private readonly Dictionary<Type, MenuScreen> _menuScreens = new Dictionary<Type, MenuScreen>();

        private readonly Dictionary<Type, List<ScreenElement>> _screenElementsPool =
            new Dictionary<Type, List<ScreenElement>>();

        public MenuNavigationController(Transform menuScreensRoot,
            Transform popupsRoot, Transform screenElementsPoolParent, CanvasGroup canvasGroup)
        {
            _menuScreensParent = menuScreensRoot;
            _popupsParent = popupsRoot;
            _screenElementsPoolParent = screenElementsPoolParent;
            _canvasGroup = canvasGroup;
        }

        public async Task<T> ShowMenuScreen<T>(Action onFinish = null) where T : MenuScreen
        {
            _canvasGroup.interactable = false;
            var menuScreen = await Showable.Create<T>(this, _menuScreensParent);
            _menuScreens[menuScreen.GetType()] = menuScreen;
            menuScreen.transform.SetAsLastSibling();
            menuScreen.IsActive = true;
            menuScreen.Show(() =>
            {
                _canvasGroup.interactable = true;
                onFinish?.Invoke();
            });
            return menuScreen;
        }

        public void HideMenuScreen<T>(Action onFinish = null) where T : MenuScreen
        {
            if (_menuScreens.Count < 2 || !_menuScreens.Values.Any(e => e.IsActive))
            {
                onFinish?.Invoke();
                return;
            }

            if (!TryGetMenuScreen<T>(out var screen))
            {
                Debug.LogWarning($"Menu Screen of type '{typeof(T)}' does not exist yet!");
                return;
            }

            HideMenuScreen(screen, onFinish);
        }

        public void HideMenuScreen(MenuScreen menuScreen, Action onFinish = null)
        {
            _canvasGroup.interactable = false;
            HideAllPopups();
            HideAllScreensElements();
            menuScreen.Hide(() =>
            {
                _canvasGroup.interactable = true;
                menuScreen.IsActive = false;
                menuScreen.transform.SetAsFirstSibling();
                onFinish?.Invoke();
            });
        }

        public async Task<T> ShowPopup<T>(Action onFinish = null) where T : Popup
        {
            _canvasGroup.interactable = false;
            var popup = await Showable.Create<T>(this, _popupsParent);
            _popups[popup.GetType()] = popup;
            popup.transform.SetAsLastSibling();
            popup.IsActive = true;
            popup.Show(() =>
            {
                _canvasGroup.interactable = true;
                onFinish?.Invoke();
            });
            return popup;
        }

        public void HidePopup<T>(Action onFinish = null) where T : Popup
        {
            if (!_popups.Values.Any(pop => pop.IsActive)) return;
            if (!TryGetPopup<T>(out var popup))
            {
                Debug.LogWarning($"Popup of type '{typeof(T)}' does not exist yet!");
                return;
            }

            HidePopup(popup, onFinish);
        }

        public void HidePopup(Popup popup, Action onFinish = null)
        {
            _canvasGroup.interactable = false;
            popup.Hide(() =>
            {
                _canvasGroup.interactable = true;
                popup.IsActive = false;
                onFinish?.Invoke();
            });
        }

        public void HideAllPopups()
        {
            foreach (var popup in _popups.Values)
            {
                popup.IsActive = false;
            }
        }

        public async Task<T> ShowScreenElement<T>(Action onFinish = null)
            where T : ScreenElement
        {
            _canvasGroup.interactable = false;

            T element = null;
            var type = typeof(T);

            foreach (var elementsPool in _screenElementsPool.Where(elementsPool => elementsPool.Key == type))
            {
                foreach (var elements in elementsPool.Value.Where(elements => !elements.IsActive))
                {
                    element = (T) elements;
                    break;
                }

                break;
            }
            
            if (element == null)
            {
                element = await Showable.Create<T>(this, _screenElementsPoolParent);

                if (_screenElementsPool.ContainsKey(typeof(T)))
                    _screenElementsPool[type].Add(element);
                else
                    _screenElementsPool.Add(type, new List<ScreenElement> {element});
            }
            
            element.IsActive = true;
            
            element.Show(() =>
            {
                _canvasGroup.interactable = true;
                onFinish?.Invoke();
            });

            return element;
        }

        public void HideScreenElement(ScreenElement element, Action onFinish = null)
        {
            _canvasGroup.interactable = false;
            element.Hide(() =>
            {
                _canvasGroup.interactable = true;
                element.IsActive = false;
                onFinish?.Invoke();
            });
        }

        public void HideAllScreensElements()
        {
            foreach (var elements in _screenElementsPool.SelectMany(elementsPool => elementsPool.Value))
            {
                elements.IsActive = false;
                elements.transform.SetParent(_screenElementsPoolParent);
            }
        }

        public void ReturnScreenElement(ScreenElement element)
        {
            element.transform.SetParent(_screenElementsPoolParent);
        }

        public bool TryGetPopup<T>(out T popup) where T : Popup
        {
            var type = typeof(T);
            var hasPopup = _popups.TryGetValue(type, out var p);
            popup = (T) p;
            return hasPopup;
        }

        public bool TryGetMenuScreen<T>(out T menuScreen) where T : MenuScreen
        {
            var type = typeof(T);
            var hasMenuScreen = _menuScreens.TryGetValue(type, out var screen);
            menuScreen = (T) screen;
            return hasMenuScreen;
        }

        public bool Interactable
        {
            get => _canvasGroup.interactable;
            set => _canvasGroup.interactable = value;
        }
    }
}