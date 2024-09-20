using UnityEngine;

public class EnemyForwardAnimEvents : MonoBehaviour
{
    private Enemy _enemyParent;

    public void Awake()
    {
        _enemyParent = transform.parent.GetComponent<Enemy>();
    }

    public void AttackFrame()
    {
        _enemyParent.AttackFrame();
    }

    public void OnFootstep()
    {
        
    }
}
