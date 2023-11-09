using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UltimateClean
{
    /// <summary>
    /// Create the level factor UI
    /// </summary>
    public class LevelFactorCreator : MonoBehaviour
    {
        public GameObject levelFactorPrefab;
        public GameObject levelFactorParent;
        List<LevelFactor> levelFactors;
        GameObject levelFactor;

        // Start is called before the first frame update
        void Start()
        {
            levelFactors = LevelManager.getAllLevelFactors();
            foreach (LevelFactor factor in levelFactors)
            {
                levelFactor = Instantiate(levelFactorPrefab, levelFactorParent.transform);
                levelFactor.GetComponent<LevelFactorController>().SetLevelFactor(factor);
            }
        }

    }
}

