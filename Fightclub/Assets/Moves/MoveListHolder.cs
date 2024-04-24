using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveListHolder : MonoBehaviour
{
    public List<MoveData> moveList;
    public List<Transform> projectileFirePoints;
    public List<AnimationClip> movementAnimations;
    public List<AnimationClip> damageAnimations;
    public List<AnimationClip> recoveryAnimations;
    public PhysicMaterial[] physMatList; //0 ground, 1 air

    public BehaviorTree.Tree masterTree;
}
