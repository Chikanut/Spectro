using System;
using MenuNavigation;

public class ScreenElement : Showable
{
    public override void Show(Action onFinish)
    {
        TransitionState = ShowableTransitionState.Shown;
        onFinish?.Invoke();
    }

    public override void Hide(Action onFinish)
    {
        TransitionState = ShowableTransitionState.Hidden;
        MenuNavigationController.ReturnScreenElement(this);
        onFinish?.Invoke();
    }

    public override bool IsActive {
        get => gameObject.activeSelf;
        set => gameObject.SetActive(value);
    }
        
    public override ShowableTransitionState TransitionState { get; protected set; }
}
