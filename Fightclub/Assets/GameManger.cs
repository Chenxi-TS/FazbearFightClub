using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManger : MonoBehaviour
{
    static GameManger _instance;
    private void Start()
    {
        _instance = this;
        Application.targetFrameRate = 60;
    }
}
