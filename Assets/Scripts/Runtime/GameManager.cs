using Assets.Scripts.Runtime.Character;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Runtime
{
    /// <summary>
    /// singleton for managing the game state
    /// </summary>
    public class GameManager : MonoBehaviour
    {
        public Hero Hero;
        public List<Enemy> Enemies = new List<Enemy>();

        private static GameManager _instance;
        public static GameManager Instance { get {
                if (_instance == null) {
                    var go = new GameObject("GameManager");
                    _instance = go.AddComponent<GameManager>();
                }
                return _instance;
        } }


        public void RegisterEnemy(Enemy enemy) {
            Enemies.Add(enemy);
        }

        public void UnregisterEnemy(Enemy enemy)
        {
            Enemies.Remove(enemy);
        }

        public void Awake()
        {
            if (_instance != null && _instance != this)
            {
                Destroy(gameObject);
            }
            else
            {
                _instance = this;
            }
        }

        public void Start() {
            Hero = FindObjectOfType<Hero>();
        }

        public void FixedUpdate()
        {
            foreach (var e in Enemies) {
                if ((Hero.transform.position - e.transform.position).sqrMagnitude < Enemy.AGGRO_RANGE_SQUARED) {
                    e.TryAggroOn(Hero.gameObject);
                }
            }
        }

    }
}
