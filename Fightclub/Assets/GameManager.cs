using BehaviorTree;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.PlayerLoop;

//Responsible for keeping track of what frame it currently is in a game round and calling InputHandler.ReadInput() for each player
//StartRound() -> starts game round
//SetPlayerHandler -> connects character InputHandlers to be read in Update()
public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public int updateEveryFPS = 60;
    float updateInterval;

    public List<GameObject> characters = new List<GameObject>();
    BehaviorTree.Tree firstPlayer;
    BehaviorTree.Tree secondPlayer;

    [SerializeField]
    int currentFrame = 0;
    public int GetCurrentFrame { get { return currentFrame; } }

    [SerializeField]
    float roundDuration;
    bool gaming = false;

    InputHandler player1InputHandler;
    InputHandler player2InputHandler;

    private void Awake()
    {
        updateInterval = 1 / updateEveryFPS;
        if (Instance != null && Instance != this)
            DestroyImmediate(Instance);
        else if (Instance == null)
            Instance = this;
    }
    private void Update()
    {
        if(player1InputHandler != null)
            player1InputHandler.ReadInput();
        if (player2InputHandler != null)
            player2InputHandler.ReadInput();
    }
    private void Start()
    {
        StartRound(0); //temp for testing call this wherever players choose their characters
    }

    //Starts round with 2 player setting up each player's character tree...
    public void StartRound(int p1CharacterID, int p2CharacterID)
    {
        if(gaming)
        {
            Debug.Log("Already gaming");
            return;
        }
        gaming = true;
        //make trees here
        firstPlayer = constructCharacterTree(1, p1CharacterID);
        secondPlayer = constructCharacterTree(2, p2CharacterID);
        StartCoroutine(RoundTimer(roundDuration));
        StartCoroutine(FrameUpdate());
    }
    //...or start with 1 player
    public void StartRound(int p1CharacterID)
    {
        if (gaming)
        {
            Debug.Log("Already gaming");
            return;
        }
        gaming = true;
        //make trees here
        firstPlayer = constructCharacterTree(1, p1CharacterID);
        StartCoroutine(RoundTimer(roundDuration));
        StartCoroutine(FrameUpdate());
    }
    //returns different character trees base on characterID, 
    //same as GameObject stored in List<GameObject> characters
    BehaviorTree.Tree constructCharacterTree(int slot ,int characterID)
    {
        switch (characterID) 
        {
            case 0:
                return new Char_01_Tree(slot, characterID);
            default:
                return null;
        }
    }

    //Called during character tree construtor
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
            if (firstPlayer == null)
            {
                yield break;
            }
            firstPlayer.Evaluate();
            if(secondPlayer != null)
                secondPlayer.Evaluate();
            currentFrame++; 
            float frameTime = 1f / 60f;
            float startTime = Time.time;
            // This loop waits until it's time to proceed to the next frame.
            while (Time.time < startTime + frameTime)
            {
                yield return null; // This yields for one frame.
            }
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
