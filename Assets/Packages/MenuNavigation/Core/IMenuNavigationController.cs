using System;
using System.Threading.Tasks;

namespace MenuNavigation {
    public interface IMenuNavigationController {
        Task<T> ShowMenuScreen<T>(Action onFinish = null) where T : MenuScreen;
        void HideMenuScreen<T>(Action onFinish = null) where T : MenuScreen;
        void HideMenuScreen(MenuScreen menuScreen, Action onFinish = null);
        Task<T> ShowPopup<T>(Action onFinish = null) where T : Popup;
        void HidePopup<T>(Action onFinish = null) where T : Popup;
        void HidePopup(Popup popup, Action onFinish = null);
        void HideAllPopups();
        bool TryGetMenuScreen<T>(out T menuScreen) where T : MenuScreen;
        bool TryGetPopup<T>(out T popup) where T : Popup;
        bool Interactable { get; set; }
    }
}
