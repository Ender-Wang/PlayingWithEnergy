
using System.Runtime.ConstrainedExecution;

public class EnergyConsumption: LevelFactor
{
    public readonly static EnergyConsumption Instance = new EnergyConsumption();
    
    protected EnergyConsumption(): base("Energy Consumption"){
       
    }
}
