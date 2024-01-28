using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveListHolder : MonoBehaviour
{
    public List<MoveData> moveList;
    public List<Transform> projectileFirePoints;
    public List<AnimationClip> movementAnimations;
    public List<AnimationClip> damageAnimations;
    public BehaviorTree.Tree masterTree;
}
