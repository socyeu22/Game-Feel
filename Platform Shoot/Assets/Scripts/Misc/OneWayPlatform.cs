using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OneWayPlatform : MonoBehaviour
{
    [SerializeField] private float _disableColliderTime = 1f;
    private bool _playOnPlatform = false;
    private Collider2D _collider;
    private void Awake() {
        _collider = GetComponent<Collider2D>();
    }
    private void Update() {
        DetectPlayerInput();
    }
    private void OnCollisionEnter2D(Collision2D other) {
        if(other.gameObject.GetComponent<PlayerController>()) {
            _playOnPlatform = true;
        }
    }

    private void OnCollisionExit2D(Collision2D other) {
        if(other.gameObject.GetComponent<PlayerController>()) {
            _playOnPlatform = false;
        }
    }

    private void DetectPlayerInput() {
        if(!_playOnPlatform) return;

        if(PlayerController.Instance.MoveInput.y < 0) {
            StartCoroutine(DisablePlatformColliderRoutine());
        }
    }

    private IEnumerator DisablePlatformColliderRoutine() {
        Collider2D[] PlayerColliders = PlayerController.Instance.GetComponents<Collider2D>();
        foreach(Collider2D playerCollider in PlayerColliders) {
            Physics2D.IgnoreCollision(playerCollider, _collider, true);
        }
        yield return new WaitForSeconds(_disableColliderTime);

        foreach(Collider2D playerCollider in PlayerColliders) {
            Physics2D.IgnoreCollision(playerCollider, _collider, false);
        }
    }
}
