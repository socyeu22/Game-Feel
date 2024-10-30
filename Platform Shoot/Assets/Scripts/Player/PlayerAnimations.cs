using System.Collections;
using System.Collections.Generic;
using System.Net.Http.Headers;
using Cinemachine;
using UnityEngine;

public class PlayerAnimations : MonoBehaviour
{
    [SerializeField] private ParticleSystem _moveDustVFX;
    [SerializeField] private ParticleSystem _poofDustVFX;
    [SerializeField] private Transform _characterSpriteTransform;
    [SerializeField] private Transform _cowboyHatSpriteTransform;
    [SerializeField] private float _tiltAngle = 10f;
    [SerializeField] private float _cowboyHatTiltModifer = 2f;
    [SerializeField] private float _tiltSpeed = 5f;
    [SerializeField] private float _yLandVelocityCheck = -20f; //Tốc độ sử dụng để so sánh khi người chơi rơi xuống, nếu tốc độ rơi nhỏ hơn thì sẽ tạo hiệu ứng rung màn hình và bụi
    
    private Vector2 _velocityBeforePhysicsUpdate; //Vận tốc của player trước khi cập nhật vật lý
    private Rigidbody2D _rigidbody;
    private CinemachineImpulseSource _impulseSource;

    private void Awake() {
        _rigidbody = GetComponent<Rigidbody2D>();
        _impulseSource = GetComponent<CinemachineImpulseSource>();
    }

    private void OnEnable() {
        PlayerController.OnJump += PlayPoofDustVFX; // Đăng ký sư kiện OnJump của PlayerController
    }

    private void OnDisable() {
        PlayerController.OnJump -= PlayPoofDustVFX; // Huỷ đăng ký sự kiện OnJump của PlayerController
    }

    private void FixedUpdate() {
        _velocityBeforePhysicsUpdate = _rigidbody.velocity;
    }

    private void OnCollisionEnter2D(Collision2D other) {
        if(_velocityBeforePhysicsUpdate.y < _yLandVelocityCheck) {
            PlayPoofDustVFX();
            _impulseSource.GenerateImpulse();
        }
    }
    private void Update() {
        DetectMoveDust();
        ApplyTilt();
    }

    // Hàm phát hiện di chuyển và kích hoạt hiệu ứng bụi
    private void DetectMoveDust() {
        if(PlayerController.Instance.CheckGrounded()) { //Kiểm tra nhân vật có đang đứng trên mặt đất không
            if(!_moveDustVFX.isPlaying) { // Nếu đứng trên mặt đất và đang không phát hiệu ứng bụi thì hiện hiệu ứng bụi(vì đã set up trong particle system là phát bụi khi đối tượng đi được một khoảng cách nhất định nên không cần kiểm tra đối tượng có đang di chuyển không)
                _moveDustVFX.Play();
            }
        } else {
            if(_moveDustVFX.isPlaying) { // nếu đối tượng không trên mặt đất và đối VFX đang phát thì tắt VFX
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
        //Player sprite tilt
        Quaternion currentCharacterRotation = _characterSpriteTransform.rotation; // Lấy góc quay hiệu tại của nhân vật
        Quaternion targetCharacterRotation = Quaternion.Euler(currentCharacterRotation.eulerAngles.x, currentCharacterRotation.eulerAngles.y, targetAngle); //Giữ nguyên góc quay theo trục x và y, chỉ thay đối góc quay theo trục z = targetAngle
        _characterSpriteTransform.rotation = Quaternion.Lerp(currentCharacterRotation, targetCharacterRotation, _tiltSpeed * Time.deltaTime); //Set góc quay mới cho nhân vật

        //Cowboy hat sprite tilt, tương tự như character sprite tilt nhưng góc nghiêng ngược lại và có biến _cowboyHatTiltModifer để điều chỉnh góc nghiêng
        Quaternion currentHatRotation = _cowboyHatSpriteTransform.rotation;
        Quaternion targetHatRotation = Quaternion.Euler(currentHatRotation.eulerAngles.x, currentHatRotation.eulerAngles.y, -targetAngle /_cowboyHatTiltModifer);
        _cowboyHatSpriteTransform.rotation = Quaternion.Lerp(currentHatRotation, targetHatRotation, _tiltSpeed * _cowboyHatTiltModifer * Time.deltaTime);
    }

    private void PlayPoofDustVFX() {
        _poofDustVFX.Play();
    }
}
