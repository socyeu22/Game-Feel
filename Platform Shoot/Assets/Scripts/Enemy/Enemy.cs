using System.Collections;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    // [SerializeField] private float _moveSpeed = 3f;
    [SerializeField] private float _jumpForce = 7f;
    [SerializeField] private float _jumpInterval = 4f;
    [SerializeField] private float _changeDirectionInterval = 3f; //Biến này sẽ quy định thời gian giữa mỗi lần thay đổi hướng di chuyển của Enemy

    private Movement _movement;

    // private int _currentDirection;

    private Rigidbody2D _rigidBody;

    private void Awake()
    {
        _rigidBody = GetComponent<Rigidbody2D>();
        _movement = GetComponent<Movement>();
    }

    private void Start() {
        StartCoroutine(ChangeDirectionRoutine());
        StartCoroutine(RandomJumpRoutine());
    }

    // private void Update()
    // {
    //     Movement();
    // }
    // private void FixedUpdate()
    // {
    //     Move();
    // }

    // private void Move()
    // {
    //     Vector2 newVelocity = new(_currentDirection * _moveSpeed, _rigidBody.velocity.y);
    //     _rigidBody.velocity = newVelocity;
    // }
    // private void Movement(){
    //     _movement.SetCurrentDirection(_currentDirection);
    // }

    private IEnumerator ChangeDirectionRoutine()
    {
        while (true)
        {
            float _currentDirection = UnityEngine.Random.Range(0, 2) * 2 - 1; // 1 or -1
            _movement.SetCurrentDirection(_currentDirection);
            yield return new WaitForSeconds(_changeDirectionInterval);
        }
    }

    private IEnumerator RandomJumpRoutine() 
    {
        while (true)
        {
            yield return new WaitForSeconds(_jumpInterval);
            float randomDirection = Random.Range(-1, 1);
            Vector2 jumpDirection = new Vector2(randomDirection, 1f).normalized;
            _rigidBody.AddForce(jumpDirection * _jumpForce, ForceMode2D.Impulse);
        }
    }
}
