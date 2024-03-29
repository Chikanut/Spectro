using UnityEngine;

public class GameManager : MonoBehaviour
{
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

    void Start()
    {
        Init();
    }

    public void Init()
    {
     
    }
}
