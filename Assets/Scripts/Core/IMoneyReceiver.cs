namespace SmolTheftAuto.Core
{
    // Interface for systems that can receive money
    // Decouples MoneyPickup from PlayerMoney
    public interface IMoneyReceiver
    {
        void AddMoney(int amount);
    }
}

