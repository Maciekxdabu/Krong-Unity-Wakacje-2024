using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Assets.Scripts.Runtime.Character;

namespace Assets.Scripts.Runtime.UI
{
    public class HUD : MonoBehaviour
    {
        [SerializeField] private TMP_Text controlledMinionText;
        [SerializeField] private TMP_Text maxMinionText;
        [SerializeField] private TMP_Text currentMinionText;
        [SerializeField] private TMP_Text heroHpText;
        [SerializeField] private TMP_Text heroHpMaxText;

        private Hero ownerHero;

        //singleton
        private static HUD _instance;
        public static HUD Instance { get { return _instance; } }

        // ---------- Unity messages

        private void Awake()
        {
            _instance = this;
        }

        // ---------- public methods

        //Refreshes the HUD with the values from the given Hero
        public void RefreshHUD(Hero hero)
        {
            if (hero == null)
            {
                hero = ownerHero;
                if (hero == null)
                {
                    Debug.LogWarning("WAR: Updating HUD without a Hero reference", gameObject);
                    return;
                }
            }
            else
                ownerHero = hero;

            currentMinionText.text = hero.MinionCount.ToString();
            maxMinionText.text = "10";
            controlledMinionText.text = hero.ControlledType.ToString();

            Health ownerHealth = hero.gameObject.GetComponent<Health>();
            heroHpText.text = ownerHealth.HealthPoints.ToString();
            heroHpMaxText.text = ownerHealth.MaxHealthPoints.ToString();
        }
    }
}