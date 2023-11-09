
using System.Runtime.ConstrainedExecution;

public class EnergyProvision: LevelFactor
{
    public readonly static EnergyProvision Instance = new EnergyProvision();
    
    protected EnergyProvision(): base("Energy Provision"){
       
    }
}
