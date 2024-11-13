using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField] private GameObject _bulletVFX; // Prefab của hiệu ứng khi nổ tung viên đạn khi va chạm
    [SerializeField] private float _moveSpeed = 10f; // Tốc độ di chuyển của viên đạn
    [SerializeField] private int _damageAmount = 1; //Sát thương của viên đạn
    [SerializeField] private float _knockbackThrust = 20f; //Lực đẩy lùi tác động lên đối tượng trúng đạn

    private Vector2 _fireDirection; // Hướng bắn của viên đạn(sử dụng để tác động lực đẩy lùi)

    private Rigidbody2D _rigidBody; //RIgidbody của viên đạn

    private Gun _gun; // Tham chiếu đều đối tượng Gun

    private void Awake()
    {
        _rigidBody = GetComponent<Rigidbody2D>();
        _gun = FindObjectOfType<Gun>();
    }

    private void FixedUpdate()
    {
        _rigidBody.velocity = _fireDirection * _moveSpeed;
    }

    // Hàm này sẽ được gọi từ hàm ShootProjectile() của Gun.cs, Init(Initialize - khởi tạo) khởi tạo những giá trị cần thiết cho viên đạn trong trường hợp này là hướng bay của viên đạn
    public void Init(Gun gun, Vector2 bulletSpawnPoint, Vector2 mousePos)
    {
        _gun = gun;
        transform.position = bulletSpawnPoint; // Đặt vị trí của viên đạn bằng vị trí khởi tạo của viên đạn
        _fireDirection = (mousePos - bulletSpawnPoint).normalized; // Tính hướng bay theo hướng của chuột và vị trí khởi tạo viên đạn. .normalized sẽ giúp chúng ta chuẩn hóa hướng bay về 1
    }


    private void OnTriggerEnter2D(Collider2D other) {
        Instantiate(_bulletVFX, transform.position, transform.rotation);

        IHitable iHitable = other.gameObject.GetComponent<IHitable>();
        iHitable?.TakeHit();

        IDamageable iDamageable = other.gameObject.GetComponent<IDamageable>();
        iDamageable?.TakeDamage(transform.position ,_damageAmount, _knockbackThrust);
        // Health health = other.gameObject.GetComponent<Health>();
        // health?.TakeDamage(_damageAmount);

        // Knockback knockback = other.gameObject.GetComponent<Knockback>();
        // knockback?.GetKnockBack(PlayerController.Instance.transform.position,_knockbackThrust);

        // Flash flash = other.gameObject.GetComponent<Flash>();
        // flash?.StartFalsh();


        _gun.ReleaseBulletFromPool(this);
    }
}