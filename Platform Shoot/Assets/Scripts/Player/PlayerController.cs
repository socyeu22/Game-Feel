using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

public class PlayerController : MonoBehaviour
{

    public Vector2 MoveInput => _frameInput.Move;
    public static Action OnJump;
    public static Action OnJetpack;
    public static PlayerController Instance;

    [SerializeField] private TrailRenderer _jetpackTrailRenderer;
    [SerializeField] private Transform _feetTransform; // Đây là transform của hộp dưới chân nhân vật, dùng để kiểm tra xem nhân vật có đang đứng trên mặt đất không
    [SerializeField] private Vector2 _groundCheck; // Kích thước của hộp kiểm tra xem nhân vật có đang đứng trên mặt đất không
    [SerializeField] private LayerMask _groundLayer; // Layer của mặt đất
    // [SerializeField] private float _moveSpeed = 5f;
    [SerializeField] private float _jumpStrength = 7f;
    [SerializeField] private float _extraGravity = 700f; // Trọng lực thêm khi nhân vật rơi xuống
    [SerializeField] private float _gravityDelay = 0.2f; // Thời gian trước khi trọng lực thêm được áp dụng(sau khi nhảy lên được 0.2s thì sẽ áp dụng lực thêm)
    [SerializeField] private float _coyoteTime = 0.5f; // Thời gian nhân vật có thế nhảy sau khi rời khỏi mặt đất
    [SerializeField] private float _jetpackTime = 0.6f; // Thời gian nhân vật có thể sử dụng jetpack sau khi nhảy
    [SerializeField] private float _jetpackStrength = 11f;
    [SerializeField] private float _maxFallSpeedVelocity = -20f;
    private float _coyoteTimer; // Biến này dùng để đếm thời gian nhân vật rời khởi mặt đất dùng để so sánh với _coyoteTime 
    private bool _doubleJumpAvailable = true;
    private float _timeInAir; // Thời gian nhân vật ở trạn thái trên không

    private PlayerInput _playerInput;
    private FrameInput _frameInput;
    private Coroutine _jetpackRoutine;

    // private bool _isGrounded = false;
    // private Vector2 _movement;

    private Rigidbody2D _rigidBody; // Rigidbody của nhân vật, cũng chính là Rigidbody ở Movement.cs vì cũng gắn vào cùng GameObject Player
    private Movement _movement; // Tham chiếu đến component Movement của chính GameObject Player cũng chứa PlayerController này


    public void Awake() {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        _rigidBody = GetComponent<Rigidbody2D>();
        _playerInput = GetComponent<PlayerInput>();
        _movement = GetComponent<Movement>();
    }
    private void OnEnable(){
        OnJump += ApplyJumpForce;
        OnJetpack += StartJetpack;
    }

    private void OnDisable(){
        OnJump -= ApplyJumpForce;
        OnJetpack -= StartJetpack;
    }

    private void Update()
    {
        GatherInput();
        Movement();
        CoyoteTimer();
        HandleJump();
        HandleSpriteFlip();
        GravityDelay();
        Jetpack();
    }
    private void OnDrawGizmos() {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(_feetTransform.position, _groundCheck);
    }
    private void FixedUpdate() {
        // Move();
        ExtraGravity();
    }

    // Hàm OnDestroy sẽ được gọi khi GameObject chứa script này bị Destroy
    private void OnDestroy() {
        Fade fade = FindObjectOfType<Fade>();
        fade?.FadeInAndOut();
    }
    public bool CheckGrounded()
    {
        Collider2D isGrounded = Physics2D.OverlapBox(_feetTransform.position, _groundCheck, 0f, _groundLayer);
        return isGrounded;
    }

    public bool IsFacingRight()
    {
        return transform.eulerAngles.y == 0;
    }

    // Hàm tính toán thời gian nhân vật ở trạng thái trên không
    private void GravityDelay(){
        if(!CheckGrounded()){ // Nếu nhân vật đang trên không
            _timeInAir += Time.deltaTime; // Tăng thời gian nhân vật ở trạng thái trên không
        } else {
            _timeInAir = 0f; // Nếu nhân vật đang ở trên mặt đất thì reset thời gian trên không về 0
        }
    }

    private void ExtraGravity(){
        if(_timeInAir > _gravityDelay){
            _rigidBody.AddForce(new Vector2(0f, -_extraGravity * Time.deltaTime));
        }
        if(_rigidBody.velocity.y < _maxFallSpeedVelocity) {
            _rigidBody.velocity = new Vector2(_rigidBody.velocity.x, _maxFallSpeedVelocity);
        }
    }
    
    // Hàm này sẽ gọi để lấy Input từ người chơi được quy định trong PlayerInput.cs
    private void GatherInput()
    {
        // float moveX = Input.GetAxis("Horizontal");
        // _movement = new Vector2(moveX * _moveSpeed, _rigidBody.velocity.y);
        _frameInput = _playerInput.FrameInput; // Set giá trị của _frameInput bằng giá trị FrameInput từ PlayerInput.cs
        // _movement = new Vector2(_frameInput.Move.x * _moveSpeed, _rigidBody.velocity.y);

    }

    private void Movement() {

        // _rigidBody.velocity = new Vector2(_movement.x, _rigidBody.velocity.y);
        _movement.SetCurrentDirection(_frameInput.Move.x);
    }

    private void HandleJump()
    {
        if(!_frameInput.Jump) return;
        if(CheckGrounded()){
            // _doubleJumpAvailable = false;
            OnJump?.Invoke();
        } else if(_coyoteTimer > 0f){
            OnJump?.Invoke();
        } else if(_doubleJumpAvailable){
            _doubleJumpAvailable = false;
            OnJump?.Invoke();
        }
        // if (Input.GetKeyDown(KeyCode.Space) && CheckGrounded()) {
        //     _rigidBody.AddForce(Vector2.up * _jumpStrength, ForceMode2D.Impulse);
        // }
    }

    private void CoyoteTimer(){
        if(CheckGrounded()){
            _coyoteTimer = _coyoteTime;
            _doubleJumpAvailable = true;
        }else{
            _coyoteTimer -= Time.deltaTime;
        }
    }

    // Hàm sẽ gọi để tác dụng lục nhảy lên nhân vật
    private void ApplyJumpForce(){
            _rigidBody.velocity = Vector2.zero; // Reset vận tốc của nhân vật trước khi nhảy
            _timeInAir = 0f; // Reset thời gian nhân vật ở trạng thái trên không về 0
            _coyoteTimer = 0f; // Reset thời gian nhân vật rời khỏi mặt đất về 0
            _rigidBody.AddForce(Vector2.up * _jumpStrength, ForceMode2D.Impulse); //Tác dụng lực nhảy lên nhân vật
    }

    private void HandleSpriteFlip()
    {
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        if (mousePosition.x < transform.position.x)
        {
            transform.eulerAngles = new Vector3(0f, -180f, 0f);
        }
        else
        {
            transform.eulerAngles = new Vector3(0f, 0f, 0f);
        }
    }

    private void Jetpack() {
        if(!_frameInput.Jetpack || _jetpackRoutine != null) return;
        OnJetpack?.Invoke();
    }

    private void StartJetpack() {
        _jetpackTrailRenderer.emitting = true;
        _jetpackRoutine = StartCoroutine(JetpackRoutine());

    }

    private IEnumerator JetpackRoutine() {
        float jetTime = 0f;
        while(jetTime <_jetpackTime) {
            jetTime += Time.deltaTime;
            _rigidBody.velocity = Vector2.up * _jetpackStrength;
            yield return null;
        }
        _jetpackTrailRenderer.emitting = false;
        _jetpackRoutine = null;
    }
}
