using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Move", menuName = "New Move")]
public class MoveData : ScriptableObject
{
    public enum MoveType
    {
        LIGHT = 1,
        MEDIUM = 2,
        HEAVY = 3,
        COMMAND = 4,
        SPECIAL = 5,
        SUPER = 6
    }
    public enum HitType
    {
        OVERHEAD,
        MID,
        LOW,
        GRAB
    }
    public enum PowerType
    {
        NORMAL,
        KNOCK_DOWN,
        HARD_KNOCK_DOWN
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
    public List<MoveData> specialCancelables;

    public AnimationClip moveAnimation;
    public GameObject hitbox;

    public GameObject projectile;
    public Transform projectileFirePoint;
}
