using System;

public class LevelFactor
{
    private string key;

    private int value;

    protected LevelFactor(string key)
    {
        // Load the value of this LevelFactor from player data using the provided key.
        value = ES3.Load<int>(key, "Player/Level", 0);
        this.key = key;
    }

    // Set the value of this LevelFactor.
    public virtual void set(int value)
    {
        this.value = value;
    }

    // Get the current value of this LevelFactor.
    public virtual int get()
    {
        return value;
    }

    // Get the name of this LevelFactor (the key).
    public string name() { return key; }

    // Update the LevelFactor value based on the difference between oldValue and newValue.
    public virtual void update(int oldValue, int newValue)
    {
        set(value + newValue - oldValue);
    }

    // Update the LevelFactor value by adding the provided diff value.
    public virtual void update(int diff)
    {
        set(value + diff);
    }

    // Store the current value of this LevelFactor in player data.
    public void store()
    {
        ES3.Save<int>(key, value, "Player/Level");
    }
}
