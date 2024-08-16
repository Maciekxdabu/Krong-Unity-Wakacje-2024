using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace Assets.Scripts.Runtime.UI
{
    public class HUD : MonoBehaviour
    {
        [SerializeField] private TMP_Text controlledMinionText;
        [SerializeField] private TMP_Text heroHpText;

        //singleton
        private static HUD _instance;
        public static HUD Instance { get { return _instance; } }

        // ---------- Unity messages

        private void Awake()
        {
            _instance = this;
        }

        // ---------- public methods

        public void UpdateControlledMinion(string newTexr)
        {
            controlledMinionText.text = newTexr;
        }

        public void Update()
        {
            var hero = GameManager.Instance.Hero;
            var heroHealth = hero.gameObject.GetComponent<Health>();
            heroHpText.text = $"HP {heroHealth.HealthPoints} / {heroHealth.MaxHealthPoints}";
        }
    }
}