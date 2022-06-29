namespace BattleVariables
{
    public interface IAmmo
    { 
        int Ammo { get; set; }
    
        void AmmoMinus();
    }
}
