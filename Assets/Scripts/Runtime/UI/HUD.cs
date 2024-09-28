using Assets.Scripts.Runtime.Character;
using Assets.Scripts.Runtime.ScriptableObjects;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UIElements;

namespace Assets.Scripts.Runtime.UI
{

    public class HUD : MonoBehaviour
    {
        [SerializeField] private GameObject _deadSplash;
        [SerializeField] private GameObject _parentStartingTimeToWave;
        [SerializeField] private GameObject _finishedWave;

        [SerializeField] private TMP_Text controlledMinionText;
        [SerializeField] private TMP_Text maxMinionText;
        [SerializeField] private TMP_Text currentMinionText;
        [SerializeField] private TMP_Text heroHpText;
        [SerializeField] private TMP_Text heroHpMaxText;
        [SerializeField] private RectTransform heroHpFill;
        private float heroHpFillMaxWidth;


        [Serializable]
        private class CustomText
        {
            public string ID;
            public TMP_Text textField;
        }
        [Space]
        [SerializeField] private List<CustomText> customTexts = new List<CustomText>();
        
        [Serializable]
        public class MinionHud
        {
            public MinionType MinionType;
            public TMP_Text count_TMP;
            public GameObject highlight;
        }
        [Space]
        [SerializeField] private List<MinionHud> minions = new List<MinionHud>();


        const string CUSTOM_TEXT_GOLD = "BonusGold";
        const string CUSTOM_TEXT_SOUL_ENERGY = "BonusSoulEnergy";
        const string CUSTOM_TEXT_GENERIC_MESSAGE = "GenericTextMessage";
        const string CUSTOM_TEXT_ENEMY_STARTING_WAVE = "EnemyStartingWave";

        private Hero ownerHero;

        //singleton
        private static HUD _instance;
        public static HUD Instance { get { return _instance; } }

        // ---------- Unity messages

        private void Awake()
        {
            _instance = this;
            refreshCustomHUD(CUSTOM_TEXT_GENERIC_MESSAGE, $"");
            heroHpFillMaxWidth = heroHpFill.rect.width;
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
            maxMinionText.text = Hero.MAX_MINIONS.ToString();

            foreach (var minion in minions)
            {
                minion.highlight.SetActive(hero.IsMinionTypeActive(minion.MinionType));
                minion.count_TMP.text = hero.GetMinionsCount(minion.MinionType).ToString();
            }

            heroHpText.text = hero.HealthPoints.ToString("F0");
            heroHpMaxText.text = hero.MaxHealthPoints.ToString();
            heroHpFill.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, heroHpFillMaxWidth * hero.HealthPoints / hero.MaxHealthPoints);
            _deadSplash.SetActive(hero.HealthPoints == 0);
        }


        internal void RefreshCustomHUD(ItemPickCounter itemPickCounter)
        {
            string ID = itemPickCounter.GetStringID;
            string newText = itemPickCounter.GetCurrentAmountAsString;

            Debug.Log($"HUD UPDATE: {itemPickCounter.GetStringID} -> {newText}");

            refreshCustomHUD(ID, newText);
        }

        internal void UpdateStartingTimeToWave(float time)
        {
            refreshCustomHUD(CUSTOM_TEXT_ENEMY_STARTING_WAVE, time.ToString("f2"));
        }

        //usage:
        //HUD.Instance.RefreshCustomHUD("ID", "new test value")'
        //"ID" must be an existing ID in the list in the Inspector
        private void refreshCustomHUD(string ID, string newText)
        {
            CustomText foundText = customTexts.Find(x => x.ID == ID);

            if (foundText != null)
                foundText.textField.text = newText;
        }

        internal void ShowNotEnoughCash(int missingCost)
        {
            refreshCustomHUD(CUSTOM_TEXT_GENERIC_MESSAGE, $"Not Enough Gold, Needs: {missingCost}");
            StartCoroutine(nameof(clearAfterTime));
        }

        private IEnumerator clearAfterTime()
        {
            yield return new WaitForSeconds(3.0f);
            refreshCustomHUD(CUSTOM_TEXT_GENERIC_MESSAGE, "");
        }

        private IEnumerator showFinishedWaveOfEnemies()
        {
            _finishedWave.SetActive(true);
            yield return new WaitForSeconds(2f);
            TurnOffFinishedWaveOfEnemies();
        }

        internal void ShowStartingTimeToWave()
        {
            _parentStartingTimeToWave.SetActive(true);
        }

        internal void TurnOffStartingTimeToWave()
        {
            _parentStartingTimeToWave.SetActive(false);
        }

        internal void ShowFinishedWaveOfEnemies()
        {
            StartCoroutine(showFinishedWaveOfEnemies());
        }

        private void TurnOffFinishedWaveOfEnemies()
        {
            _finishedWave.SetActive(false);
        }
    }
}