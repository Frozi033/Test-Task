public interface IDamageable
{ 
    float health { get; set; }
    void TakeDamage(float damage);
    float ConvertHP();
}
