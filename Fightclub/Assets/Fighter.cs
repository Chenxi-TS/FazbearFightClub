using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum moveStatus
{
    STARTUP,
    ACTIVE,
    RECOVERY
}
public abstract class Fighter : MonoBehaviour
{
    protected int health = 100;
    [SerializeField]
    protected List<MoveData> listOfMoves;
    
    protected void frameCounter(MoveData move)
    {
        Debug.Log("started counting");
        StartCoroutine(countingFrames(move.startUpFrames));
    }

    IEnumerator countingFrames(int frames)
    {
        while (frames > 0)
        {
            yield return null;
            frames--;
        }
        Debug.Log("Frames Finished");
    }
}
