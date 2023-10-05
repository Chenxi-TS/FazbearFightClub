using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Move", menuName = "New Move")]


public class MoveData : ScriptableObject
{
    public enum MoveType
    {
        LIGHT,
        MEDIUM,
        HEAVY,
        COMMAND,
        SPECIAL,
        SUPER
    }
    public enum HitType
    {
        OVERHEAD,
        MID,
        LOW
    }
    public enum PowerType
    {
        NORMAL,
        KNOCK_DOWN,
        HARD_KNOCK_DOWN
    }
    /*public MoveData(string moveName, MoveType type, string inputNotations, int startUpFrames, int activeFrames, int recoveryFrames
        , bool antiAir, bool overhead, bool low, bool knocksDown, bool hardKnocksDown, AnimationClip moveAnimation, bool specialCancelable
        , string nameOfSpecialThatCancelsThisMove)
    {
        moveName = this.moveName;
        type = this.type;
        inputNotations = this.inputNotations;
    }*/
    public MoveData(string name, MoveType type, int start, int active, int recovery)
    {
        moveName = name;
        this.type = type;
        startUpFrames = start;
        activeFrames = active;
        recoveryFrames = recovery;
    }
    public string moveName;
    public MoveType type;
    public string inputNotations = "Numpad notations, assuming facing right";

    public int damage;
    public HitType hitType;
    public float blockDamage;
    public int startUpFrames;
    public int activeFrames;
    public int recoveryFrames;
    public int hitAdvantage;
    public int blockAdvantage;

    public PowerType powerType;

    public bool specialCancelable;
    public string nameOfSpecialThatCancelsThisMove = "Leave blank if this is a special and not cancelable";

    public AnimationClip moveAnimation;


    public GameObject hitbox;
}
