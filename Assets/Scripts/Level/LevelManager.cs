using UnityEngine;
using System.Collections.Generic;
using System;

/// <summary>
/// Class responsible for managing game levels.
/// </summary>
public class LevelManager
{
    /// <summary>
    /// Gets the current level of the game.
    /// </summary>
    public static int level { get { return getLevel(); } }

    
    private static int getLevel()
    {
        // calculate the level according to level factors. The relationshop of different level factors is defined in this method.
        int value = 0;
        value += EnergyProvision.Instance.get();
        value -= EnergyConsumption.Instance.get();
        return value;
    }

    /// <summary>
    /// Retrieves a level factor based on the provided key.
    /// </summary>
    /// <param name="key">The key for the level factor.</param>
    /// <returns>The level factor associated with the key.</returns>

    public static LevelFactor getLevelFactor(string key)
    {
        // load LevelFactor according to key
        LevelFactor levelFactor;
        switch (key)
        {
            case "Energy Provision":
                levelFactor = EnergyProvision.Instance;
                break;
            case "Energy Consumption":
                levelFactor = EnergyConsumption.Instance;
                break;
            default:
                levelFactor = EnergyProvision.Instance;
                break;
        }
        return levelFactor;
    }

    /// <summary>
    /// Retrieves all available level factors.
    /// </summary>
    /// <returns>A list of all available level factors.</returns>
    public static List<LevelFactor> getAllLevelFactors()
    {
        // load all LevelFactor
        List<LevelFactor> levelFactors = new List<LevelFactor>();
        levelFactors.Add(EnergyProvision.Instance);
        levelFactors.Add(EnergyConsumption.Instance);
        return levelFactors;
    }

}
