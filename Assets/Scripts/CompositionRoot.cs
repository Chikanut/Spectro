using MenuNavigation;
using UnityEngine;


public class CompositionRoot : MonoBehaviour
{
    [SerializeField] private Camera _camera;
    [SerializeField] private CanvasGroup _canvas;
    [SerializeField] private RectTransform _screenParent;
    [SerializeField] private RectTransform _popUpParent;
    [SerializeField] private RectTransform _elementsParent;
    private void Start() {
        DontDestroyOnLoad(gameObject);
  
        var menuNavigation = new MenuNavigationController(_screenParent, _popUpParent, _elementsParent, _canvas);
        
        Controllers.NavigationController = menuNavigation;
        Controllers.Camera = _camera;
    }
}