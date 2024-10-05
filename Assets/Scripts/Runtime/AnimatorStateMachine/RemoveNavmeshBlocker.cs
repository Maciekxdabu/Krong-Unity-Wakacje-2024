using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class RemoveNavmeshBlocker : MonoBehaviour
{
    public void RemoveNavbeshBlocker()
    {
        GetComponent<NavMeshObstacle>().enabled = false;
    }
}
