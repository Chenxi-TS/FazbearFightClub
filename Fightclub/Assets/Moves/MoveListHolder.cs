using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveListHolder : MonoBehaviour
{
    public List<MoveData> moveList;
    public List<Transform> projectileFirePoints;
    public BehaviorTree.Tree masterTree;
}
