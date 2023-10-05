using BehaviorTree;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.PlayerLoop;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public List<GameObject> characters = new List<GameObject>();
    BehaviorTree.Tree firstPlayer;
    BehaviorTree.Tree secondPlayer;

    int currentFrame = 0;
    public int GetCurrentFrame { get { return currentFrame; } }

    [SerializeField]
    float roundDuration;
    bool gaming = false;

    InputHandler player1InputHandler;
    InputHandler player2InputHandler;
    private void Awake()
    {
        Application.targetFrameRate = 60;
        if (Instance != null && Instance != this)
            DestroyImmediate(Instance);
        else if (Instance == null)
            Instance = this;
    }
    private void Update()
    {
        firstPlayer.Evaluate();
    }
    private void Start()
    {
        StartRound();
    }
    void StartRound()
    {
        if(gaming)
        {
            Debug.Log("Already gaming");
            return;
        }
        gaming = true;
        //make trees here
        firstPlayer = new Char_01_Tree(1);
        StartCoroutine(RoundTimer(roundDuration));
        StartCoroutine(FrameUpdate());
    }
    public void SetPlayerHandler(int playerSlotNumber, InputHandler inputHandler)
    {
        if(playerSlotNumber == 1)
        {
            if(player1InputHandler != null)
            {
                Debug.LogError("Player 1 already has an InputHandler");
                return;
            }    
            player1InputHandler = inputHandler;
        }
        else if(playerSlotNumber == 2)
        {
            if(player2InputHandler != null)
            {
                Debug.LogError("Player 2 already has an InputHandler");
                return;
            }
            player2InputHandler = inputHandler;
        }
    }
    IEnumerator FrameUpdate()
    {
        while(gaming)
        {
            if (player1InputHandler == null)
            {
                yield break;
            }
            player1InputHandler.ReadInput();
            if(player2InputHandler != null)
                player2InputHandler.ReadInput();
            yield return new WaitForEndOfFrame();
            currentFrame++;
        }
        Debug.Log("Round Ended");
    }
    IEnumerator RoundTimer(float roundDuration)
    {
        yield return new WaitForSeconds(roundDuration);
        gaming = false;
        Debug.Log("Time Ran Out, Number of Frames: " + currentFrame);
        currentFrame = 0;
    }

}
