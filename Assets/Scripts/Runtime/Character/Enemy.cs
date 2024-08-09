using Assets.Scripts.Runtime.Character;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    private Hero _hero;
    private NavMeshAgent _agent;

    // Start is called before the first frame update
    void Start()
    {
        _hero = FindObjectOfType<Hero>();
        _agent = gameObject.GetComponent<NavMeshAgent>();
    }

    // Update is called once per frame
    void Update()
    {
        _agent.destination = _hero.transform.position;
    }

    public void dsadasd()
    {
        throw new System.NotImplementedException();
    }
}
