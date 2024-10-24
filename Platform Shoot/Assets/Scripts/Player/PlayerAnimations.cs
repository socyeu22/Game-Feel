using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimations : MonoBehaviour
{
    [SerializeField] private ParticleSystem _moveDustVFX;
    [SerializeField] private Transform _characterSpriteTransform;
    [SerializeField] private Transform _cowboyHatSpriteTransform;
    [SerializeField] private float _tiltAngle = 10f;
    [SerializeField] private float _cowboyHatTiltModifer = 2f;
    [SerializeField] private float _tiltSpeed = 5f;


    private void Update() {
        DetectMoveDust();
        ApplyTilt();
    }

    private void DetectMoveDust() {
        if(PlayerController.Instance.CheckGrounded()) {
            if(!_moveDustVFX.isPlaying) {
                _moveDustVFX.Play();
            }
        } else {
            if(_moveDustVFX.isPlaying) {
                _moveDustVFX.Stop();
            }
        }
    }

    private void ApplyTilt() {
        float targetAngle;
        if(PlayerController.Instance.MoveInput.x < 0) {
            targetAngle = _tiltAngle;
        } else if(PlayerController.Instance.MoveInput.x > 0) {
            targetAngle = -_tiltAngle;
        } else {
            targetAngle = 0;
        }
        Quaternion currentCharacterRotation = _characterSpriteTransform.rotation;
        Quaternion targetCharactorRotation = Quaternion.Euler(currentCharacterRotation.eulerAngles.x, currentCharacterRotation.eulerAngles.y, targetAngle);
        _characterSpriteTransform.rotation = Quaternion.Lerp(currentCharacterRotation, targetCharactorRotation, _tiltSpeed * Time.deltaTime);

        Quaternion currentHatRotation = _cowboyHatSpriteTransform.rotation;
        Quaternion targetHatRotation = Quaternion.Euler(currentHatRotation.eulerAngles.x, currentHatRotation.eulerAngles.y, -targetAngle /_cowboyHatTiltModifer);
        _cowboyHatSpriteTransform.rotation = Quaternion.Lerp(currentHatRotation, targetHatRotation, _tiltSpeed * _cowboyHatTiltModifer * Time.deltaTime);
    }
}
