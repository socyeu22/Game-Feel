using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathSplatterHandler : MonoBehaviour
{
    private void OnEnable() {

        Health.OnDeath += SpawnDeathSplatterPrefab;
        Health.OnDeath += SpawnDeathVFX;
    }

    private void OnDisable() {
        Health.OnDeath -= SpawnDeathSplatterPrefab;       
        Health.OnDeath -= SpawnDeathVFX;       
    }

    private void SpawnDeathSplatterPrefab(Health sender) {
        GameObject newSplatterPrefab = Instantiate(sender.SplatterPrefab, sender.transform.position, sender.transform.rotation);
        SpriteRenderer deathSplaterSpriteRenderer = newSplatterPrefab.GetComponent<SpriteRenderer>();
        ColorChanger colorChanger = sender.GetComponent<ColorChanger>();
        Color curentColor = colorChanger.DefaultColor;
        deathSplaterSpriteRenderer.color = curentColor;
        newSplatterPrefab.transform.SetParent(this.transform);
    }
    private void SpawnDeathVFX(Health sender){
        GameObject deathVFX = Instantiate(sender.DeathVFX, sender.transform.position, sender.transform.rotation);
        ParticleSystem.MainModule particleSystem = deathVFX.GetComponent<ParticleSystem>().main;
        ColorChanger colorChanger = sender.GetComponent<ColorChanger>();
        Color curentColor = colorChanger.DefaultColor;
        particleSystem.startColor = curentColor;
        deathVFX.transform.SetParent(this.transform);
    }
}
