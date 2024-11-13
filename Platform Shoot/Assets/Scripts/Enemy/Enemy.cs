using System.Collections;
using UnityEngine;

public class Enemy : MonoBehaviour /*,IDamageable*/
{
    // [SerializeField] private float _moveSpeed = 3f;
    [SerializeField] private float _jumpForce = 7f;
    [SerializeField] private float _jumpInterval = 4f;
    [SerializeField] private float _changeDirectionInterval = 3f; //Biến này sẽ quy định thời gian giữa mỗi lần thay đổi hướng di chuyển của Enemy
    [SerializeField] private int _damageAmount = 1;
    [SerializeField] private float _knockbackThrust = 25f;

    private Movement _movement;

    // private int _currentDirection;

    private Rigidbody2D _rigidBody;
    private ColorChanger _colorChanger;
    // private Knockback _knockback;
    // private Flash _flash;
    // private Health _health;


    private void Awake()
    {
        _rigidBody = GetComponent<Rigidbody2D>();
        _movement = GetComponent<Movement>();
        _colorChanger = GetComponent<ColorChanger>();
        // _knockback = GetComponent<Knockback>();
        // _flash = GetComponent<Flash>();
        // _health = GetComponent<Health>();
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

    private void OnCollisionEnter2D(Collision2D other) {
        PlayerController player = other.gameObject.GetComponent<PlayerController>();
        if(!player) return;
        Movement playerMovement = player.GetComponent<Movement>();
        if(playerMovement.CanMove) {
            IHitable iHitable = other.gameObject.GetComponent<IHitable>(); // Tham chiếu đến interface IHitable của đối tượng Player được kế thừa trong lớp Health.cs gắn trong Player
            iHitable?.TakeHit();

            IDamageable iDamageable = other.gameObject.GetComponent<IDamageable>(); // Tham chiếu đến interface IDamageable của đối tượng Player được kế thừa trong lớp Health.cs gắn trong Player
            iDamageable?.TakeDamage(transform.position, _damageAmount, _knockbackThrust);

            AudioManager.Instance.Enemy_OnPlayerHit();
            }
    }
    public void Init(Color color){
        _colorChanger.SetDefaultColor(color);
    }
    private IEnumerator ChangeDirectionRoutine()
    {
        while (true)
        {
            float _currentDirection = Random.Range(0, 2) * 2 - 1; // 1 or -1
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

    // public void TakeDamage(int damageAmount, float knockbackThrust)
    // {
    //     _health.TakeDamage(damageAmount);
    //     _knockback.GetKnockBack(PlayerController.Instance.transform.position, knockbackThrust);

    // }
    // public void TakeHit()
    // {
    //     _flash.StartFalsh();
    // }
}
