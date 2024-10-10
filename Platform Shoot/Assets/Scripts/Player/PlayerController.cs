using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public static PlayerController Instance;

    [SerializeField] private Transform _feetTransform; // Đây là transform của hộp dưới chân nhân vật, dùng để kiểm tra xem nhân vật có đang đứng trên mặt đất không
    [SerializeField] private Vector2 _groundCheck; // Kích thước của hộp kiểm tra xem nhân vật có đang đứng trên mặt đất không
    [SerializeField] private LayerMask _groundLayer; // Layer của mặt đất
    // [SerializeField] private float _moveSpeed = 5f;
    [SerializeField] private float _jumpStrength = 7f;
    [SerializeField] private float _extraGravity = 700f; // Trọng lực thêm khi nhân vật rơi xuống
    [SerializeField] private float _gravityDelay = 0.2f; // Thời gian trước khi trọng lực thêm được áp dụng(sau khi nhảy lên được 0.2s thì sẽ áp dụng lực thêm)

    private float _timeInAir; // Thời gian nhân vật ở trạn thái trên không

    private PlayerInput _playerInput;
    private FrameInput _frameInput;

    private bool _isGrounded = false;
    // private Vector2 _movement;

    private Rigidbody2D _rigidBody; // Rigidbody của nhân vật, cũng chính là Rigidbody ở Movement.cs vì cũng gắn vào cùng GameObject Player
    private Movement _movement; // Tham chiếu đến component Movement của chính GameObject Player cũng chứa PlayerController này

    public void Awake() {
        if (Instance == null) { Instance = this; }

        _rigidBody = GetComponent<Rigidbody2D>();
        _playerInput = GetComponent<PlayerInput>();
        _movement = GetComponent<Movement>();
    }

    private void Update()
    {
        GatherInput();
        Movement();
        Jump();
        HandleSpriteFlip();
        GravityDelay();
    }
    private void OnDrawGizmos() {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(_feetTransform.position, _groundCheck);
    }
    private void FixedUpdate() {
        // Move();
        ExtraGravity();
    }

    private bool CheckGrounded()
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
            _rigidBody.AddForce(new Vector2(0f, -_extraGravity * Time.fixedDeltaTime));
        }
    }

    private void GatherInput()
    {
        // float moveX = Input.GetAxis("Horizontal");
        // _movement = new Vector2(moveX * _moveSpeed, _rigidBody.velocity.y);
        _frameInput = _playerInput.FrameInput;
        // _movement = new Vector2(_frameInput.Move.x * _moveSpeed, _rigidBody.velocity.y);

    }

    private void Movement() {

        // _rigidBody.velocity = new Vector2(_movement.x, _rigidBody.velocity.y);
        _movement.SetCurrentDirection(_frameInput.Move.x);
    }

    private void Jump()
    {
        if(!_frameInput.Jump) return;
        if(CheckGrounded()){
            _rigidBody.AddForce(Vector2.up * _jumpStrength, ForceMode2D.Impulse);
        }
        // if (Input.GetKeyDown(KeyCode.Space) && CheckGrounded()) {
        //     _rigidBody.AddForce(Vector2.up * _jumpStrength, ForceMode2D.Impulse);
        // }
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
}
