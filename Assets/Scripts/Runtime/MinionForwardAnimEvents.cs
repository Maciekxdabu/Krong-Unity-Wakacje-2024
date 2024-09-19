using Assets.Scripts.Runtime.Character;
using UnityEngine;

public class MinionForwardAnimEvents : MonoBehaviour
{
    private Minion _minionParent;

    public void Awake()
    {
        _minionParent = transform.parent.GetComponent<Minion>();
    }

    public void AttackFrame()
    {
        _minionParent.AttackFrame();
    }
}
