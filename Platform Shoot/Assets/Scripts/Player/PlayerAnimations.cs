using System.Collections;
using System.Collections.Generic;
using System.Net.Http.Headers;
using Cinemachine;
using UnityEngine;

public class PlayerAnimations : MonoBehaviour
{
    [SerializeField] private ParticleSystem _moveDustVFX;
    [SerializeField] private ParticleSystem _jumpDustVFX;
    [SerializeField] private Transform _characterSpriteTransform;
    [SerializeField] private Transform _cowboyHatSpriteTransform;
    [SerializeField] private float _tiltAngle = 10f;
    [SerializeField] private float _cowboyHatTiltModifer = 2f;
    [SerializeField] private float _tiltSpeed = 5f;
    [SerializeField] private float _yLandVelocityCheck = -20f; //Tốc độ sử dụng để so sánh khi người chơi rơi xuống, nếu tốc độ rơi nhỏ hơn thì sẽ tạo hiệu ứng rung màn hình và bụi
    
    private Vector2 _velocityBeforePhysicsUpdate; //Vận tốc của player trước khi cập nhật vật lý
    private Rigidbody2D _rigidbody;
    private CinemachineImpulseSource[] _impulseSources;

    private void Awake() {
        _rigidbody = GetComponent<Rigidbody2D>();
        _impulseSources = GetComponents<CinemachineImpulseSource>();
    }

    private void OnEnable() {
        PlayerController.OnJump += PlayProofDustVFX;
    }

    private void OnDisable() {
        PlayerController.OnJump -= PlayProofDustVFX;
    }

    private void FixedUpdate() {
        _velocityBeforePhysicsUpdate = _rigidbody.velocity;
    }

    private void OnCollisionEnter2D(Collision2D other) {
        if(_velocityBeforePhysicsUpdate.y < _yLandVelocityCheck) {
            PlayProofDustVFX();
            _impulseSources[_impulseSources.Length - 1].GenerateImpulse();
        }
    }
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

    private void PlayProofDustVFX() {
        _jumpDustVFX.Play();
    }
}
