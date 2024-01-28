using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
    HARD_KNOCK_DOWN,
    GRAB
}
[CreateAssetMenu(fileName = "New Move", menuName = "New Move")]
public class MoveData : ScriptableObject
{
    public string moveName;
    public MoveType type;
    public HitType hitType;
    public PowerType powerType;
    [Space(10)]
    public string inputNotations = "Numpad notations, assuming facing right";
    [Space(10)]
    public int damage;
    public float blockDamage;
    [Space(10)]
    public int startUpFrames;
    public int activeFrames;
    public int recoveryFrames;
    [Space(10)]
    public int grabRecovery;
    [Space(10)]
    public int hitAdvantage;
    public int blockAdvantage;
    [Space(10)]
    public List<MoveData> specialCancelables;
    [Space(10)]
    public AnimationClip moveAnimation;
    [Space(10)]
    public GameObject hitbox;
    public GameObject projectile;
}
