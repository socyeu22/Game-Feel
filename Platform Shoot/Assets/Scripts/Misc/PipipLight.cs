using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class PipipLight : MonoBehaviour
{
    [SerializeField] private Light2D _light;
    [SerializeField] private float _pipipTime = 1.5f;

    private void Start() {
        StartCoroutine(PipipRoutine());
    }

    private IEnumerator PipipRoutine()
    {
        while (true) {
            yield return new WaitForSeconds(_pipipTime);
            _light.enabled = !_light.enabled;
        }
    }
}
