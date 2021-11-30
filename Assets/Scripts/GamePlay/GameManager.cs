using UnityEngine;

public class GameManager : MonoBehaviour
{
    public class GameInfo : Observable
    {
        public enum GameState
        {
            Construction,
            Puzzle,
            Platformer
        }

        private GameState _state;
        public GameState State
        {
            get => _state;
            set
            {
                _state = value;
                NotifyObservers();
            }
        }

    }

    private static GameManager _instance;

    public static GameManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<GameManager>();
                
                if(_instance == null)
                    _instance = new GameObject("_GameManager").AddComponent<GameManager>();
            }

            return _instance;
        }
        set => _instance = value;
    }

    public GameInfo Info = new GameInfo();

    public void Init()
    {
        
    }

    public void FinishTriggerActivated()
    {
        Info.State = GameInfo.GameState.Platformer;
        
    }
}
