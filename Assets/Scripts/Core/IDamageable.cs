namespace SmolTheftAuto.Core
{
    // Interface for entities that can receive damage
    public interface IDamageable
    {
        void TakeDamage(float damage);
    }
}
