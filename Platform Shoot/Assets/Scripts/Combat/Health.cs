using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Health : MonoBehaviour
{
    [SerializeField] private GameObject _splaterPrefab;
    [SerializeField] private int _startingHealth = 3;

    private int _currentHealth;

    private void Start() {
        ResetHealth();
    }

    public void ResetHealth() {
        _currentHealth = _startingHealth;
    }

    public void TakeDamage(int amount) {
        _currentHealth -= amount;

        if (_currentHealth <= 0) {
            Destroy(gameObject);
            SpawDeathSplaterPrefab();
        }
    }
    private void SpawDeathSplaterPrefab() {
        GameObject newSplaterPrefab = Instantiate(_splaterPrefab, transform.position, transform.rotation);
        SpriteRenderer deathSplaterSpriteRenderer = newSplaterPrefab.GetComponent<SpriteRenderer>();
        ColorChanger colorChanger = GetComponent<ColorChanger>();
        Color curentColor = colorChanger.DefaultColor;
        deathSplaterSpriteRenderer.color = curentColor;
    }
}
