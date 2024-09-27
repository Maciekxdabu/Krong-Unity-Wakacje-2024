using Assets.Scripts.Runtime;
using UnityEngine;

public class EndLevelPortal : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        GameManager.Instance.FinshLevel();
    }
}
