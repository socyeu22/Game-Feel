public interface IDamageable : IHitable
{
    void TakeDamage(int _damageAmount, float _knockbackThrust);
}