using Assets.Scripts.Runtime.Character;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Assets.Scripts.Runtime.UI
{
    public class HUD : MonoBehaviour
    {
        [System.Serializable]
        private class CustomText
        {
            public string ID;
            public TMP_Text textField;
        }

        [SerializeField] private GameObject _parentStartingTimeToWave;
        [SerializeField] private TMP_Text controlledMinionText;
        [SerializeField] private TMP_Text maxMinionText;
        [SerializeField] private TMP_Text currentMinionText;
        [SerializeField] private TMP_Text heroHpText;
        [SerializeField] private TMP_Text heroHpMaxText;
        [Space]
        [SerializeField] private List<CustomText> customTexts = new List<CustomText>();

        const string CUSTOM_TEXT_GOLD = "BonusGold";
        const string CUSTOM_TEXT_SOUL_ENERGY = "BonusSoulEnergy";
        const string CUSTOM_TEXT_GENERIC_MESSAGE = "GenericTextMessage";
        const string CUSTOM_TEXT_ENEMY_WAVE = "EnemyWave";

        private Hero ownerHero;

        //singleton
        private static HUD _instance;
        public static HUD Instance { get { return _instance; } }

        // ---------- Unity messages

        private void Awake()
        {
            _instance = this;
            refreshCustomHUD(CUSTOM_TEXT_GENERIC_MESSAGE, $"");
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

            heroHpText.text = hero.HealthPoints.ToString("F0");
            heroHpMaxText.text = hero.MaxHealthPoints.ToString();
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
            refreshCustomHUD(CUSTOM_TEXT_ENEMY_WAVE, time.ToString("f2"));
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

        internal void ShowStartingTimeToWave()
        {
            _parentStartingTimeToWave.SetActive(true);
        }

        internal void TurnOffStartingTimeToWave()
        {
            _parentStartingTimeToWave.SetActive(false);
        }
    }
}