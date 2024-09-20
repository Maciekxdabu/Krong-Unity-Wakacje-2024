using Assets.Scripts.Extensions;
using Assets.Scripts.Runtime.Character;
using Assets.Scripts.Runtime.Waves;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Runtime
{
    /// <summary>
    /// singleton for managing the game state
    /// </summary>
    public class GameManager : MonoBehaviour
    {
        private WaveController _waveController;

        public Hero Hero;
        public List<Enemy> Enemies = new List<Enemy>();

        private static GameManager _instance;
        public static GameManager Instance {
            get
            {
                if (_instance == null) {
                    var go = new GameObject("GameManager");
                    _instance = go.AddComponent<GameManager>();
                }
                return _instance;
            }
        }

        public void RegisterEnemy(Enemy enemy)
        {
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

        public void Start()
        {
            Hero = FindObjectOfType<Hero>();
        }

        public void FixedUpdate()
        {
            var minions = Hero.GetMinions();
            foreach (var e in Enemies) {
                if ( Hero.IsInRangeSquared(e, Enemy.AGGRO_RANGE_SQUARED) ) {
                    e.TrySettingAggroOn(Hero.gameObject);
                }
                foreach (var m in minions)
                {
                    if (m.IsInRangeSquared(e, Enemy.AGGRO_RANGE_SQUARED))
                    {
                        e.TrySettingAggroOn(m.gameObject);
                        m.EnemyInRange(e);
                    }
                }
            }
        }
    }
}
