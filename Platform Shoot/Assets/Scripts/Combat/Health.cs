using UnityEngine;
using System;
public class Health : MonoBehaviour, IDamageable
{
    public GameObject SplatterPrefab => _splatterPrefab;
    public GameObject DeathVFX => _deathVFX;
    public static Action<Health> OnDeath;
    [SerializeField] private GameObject _splatterPrefab;
    [SerializeField] private GameObject _deathVFX;
    [SerializeField] private int _startingHealth = 3;

    private int _currentHealth;
    private Knockback _knockback;
    private Flash _flash;
    private Health _health;
    
    private void Awake() {
        _knockback = GetComponent<Knockback>();
        _flash = GetComponent<Flash>();
        _health = GetComponent<Health>();
    }
    private void Start() {
        ResetHealth();
    }

    // private void OnEnable() {
    //     OnDeath += SpawnDeathSplaterPrefab;
    //     OnDeath += SpawnDeathVFX;
    // }

    // private void OnDisable() {
    //     OnDeath -= SpawnDeathSplaterPrefab;
    //     OnDeath -= SpawnDeathVFX;       
    // }
    public void ResetHealth() {
        _currentHealth = _startingHealth;
    }

    public void TakeDamage(int amount) {
        _currentHealth -= amount;

        if (_currentHealth <= 0) {
            OnDeath?.Invoke(this);
            Destroy(gameObject);
        }
    }
    // private void SpawnDeathSplaterPrefab() {
    //     GameObject newSplaterPrefab = Instantiate(_splaterPrefab, transform.position, transform.rotation);
    //     SpriteRenderer deathSplaterSpriteRenderer = newSplaterPrefab.GetComponent<SpriteRenderer>();
    //     ColorChanger colorChanger = GetComponent<ColorChanger>();
    //     Color curentColor = colorChanger.DefaultColor;
    //     deathSplaterSpriteRenderer.color = curentColor;
    // }
    // private void SpawnDeathVFX(){
    //     GameObject deathVFX = Instantiate(_deathVFX, transform.position, transform.rotation);
    //     ParticleSystem.MainModule particleSystem = deathVFX.GetComponent<ParticleSystem>().main;
    //     ColorChanger colorChanger = GetComponent<ColorChanger>();
    //     Color curentColor = colorChanger.DefaultColor;
    //     particleSystem.startColor = curentColor;
    // }

    public void TakeDamage(Vector2 damageSourceDir, int damageAmount, float knockbackThrust)
    {
        _health.TakeDamage(damageAmount);
        _knockback.GetKnockBack(damageSourceDir, knockbackThrust);

    }
    public void TakeHit()
    {
        _flash.StartFalsh();
    }
}
