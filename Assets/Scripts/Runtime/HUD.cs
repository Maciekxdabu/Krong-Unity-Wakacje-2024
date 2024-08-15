using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace Assets.Scripts.Runtime.UI
{
    public class HUD : MonoBehaviour
    {
        [SerializeField] private TMP_Text controlledMinionText;

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
    }
}