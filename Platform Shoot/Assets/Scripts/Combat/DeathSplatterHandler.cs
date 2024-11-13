using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathSplaterHandler : MonoBehaviour
{
    private void OnEnable() {

        Health.OnDeath += SpawnDeathSplaterPrefab;
        Health.OnDeath += SpawnDeathVFX;
    }

    private void OnDisable() {
        Health.OnDeath -= SpawnDeathSplaterPrefab;       
        Health.OnDeath -= SpawnDeathVFX;       
    }

    private void SpawnDeathSplaterPrefab(Health sender) {
        GameObject newSplaterPrefab = Instantiate(sender.SplatterPrefab, sender.transform.position, sender.transform.rotation);
        SpriteRenderer deathSplaterSpriteRenderer = newSplaterPrefab.GetComponent<SpriteRenderer>();
        ColorChanger colorChanger = sender.GetComponent<ColorChanger>();

        if(colorChanger) {
            Color curentColor = colorChanger.DefaultColor;
            deathSplaterSpriteRenderer.color = curentColor;
        }
        newSplaterPrefab.transform.SetParent(this.transform);
    }
    private void SpawnDeathVFX(Health sender){
        GameObject deathVFX = Instantiate(sender.DeathVFX, sender.transform.position, sender.transform.rotation);
        ParticleSystem.MainModule particleSystem = deathVFX.GetComponent<ParticleSystem>().main;
        ColorChanger colorChanger = sender.GetComponent<ColorChanger>();
        if(colorChanger) {
            Color curentColor = colorChanger.DefaultColor;
            particleSystem.startColor = curentColor;
        }
        deathVFX.transform.SetParent(this.transform);
    }
}
