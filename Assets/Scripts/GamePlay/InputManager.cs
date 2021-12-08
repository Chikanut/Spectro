using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour, Observer
{
    private static InputManager _instance;

    public static InputManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<InputManager>();
                
                if(_instance == null)
                    _instance = new GameObject("_GameManager").AddComponent<InputManager>();
            }

            return _instance;
        }
        set => _instance = value;
    }
    
    public void OnNotify(Observable observable)
    {
        
    }
}
