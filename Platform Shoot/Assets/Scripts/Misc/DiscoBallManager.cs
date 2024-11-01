using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.GlobalIllumination;
using UnityEngine.Rendering.Universal;

public class DiscoBallManager : MonoBehaviour
{   public static Action OnDiscoBallHitEvent;
    [SerializeField] private float _discoPartyTime = 3f;
    [SerializeField] private float _discoGlobalLightIntensity = 0.2f;
    [SerializeField] private Light2D _globalLight;

    private float _defaultGlobalLightIntensity = 0.2f;

    private Coroutine _discoCoroutine;
    private ColorSpotlight[] _allSpotlights;

    private void Awake() {
        _defaultGlobalLightIntensity = _globalLight.intensity;
    }
    private void Start() {
        _allSpotlights = FindObjectsByType<ColorSpotlight>(FindObjectsSortMode.None);
    }
    private void OnEnable() {
        OnDiscoBallHitEvent += DimTheLights;
    }

    private void OnDisable() {
        OnDiscoBallHitEvent -= DimTheLights;
    }

    public void DiscoBallParty() {
        if(_discoCoroutine != null) {
            return;
        }
        OnDiscoBallHitEvent?.Invoke();
    }

    private void DimTheLights() {
        foreach(ColorSpotlight spotLight in _allSpotlights) {
            StartCoroutine(spotLight.SpotLightDiscoParty(_discoPartyTime));
        }

        _discoCoroutine = StartCoroutine(GlobalLightResetRoutine());
    }


    private IEnumerator GlobalLightResetRoutine() {
        _globalLight.intensity = _discoGlobalLightIntensity;
        yield return new WaitForSeconds(_discoPartyTime);
        _globalLight.intensity = _defaultGlobalLightIntensity;
        _discoCoroutine = null;
    }
}
