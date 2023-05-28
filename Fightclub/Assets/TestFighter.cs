using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestFighter : Fighter
{
    // Start is called before the first frame update
    void Start()
    {
        
    }


    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            frameCounter(listOfMoves[0]);
        }
    }
}
