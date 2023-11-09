using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace UltimateClean
{
    public class AchievementButtonController : MonoBehaviour
    {
        public TextMeshProUGUI PendingTaskAmount;

        // Start is called before the first frame update
        void Start()
        {
            getPendingTaskAmount();
        }

        // Update is called once per frame
        void Update()
        {

        }
        // TODO: get active pending task account
        void getPendingTaskAmount()
        {
        }
    }
}