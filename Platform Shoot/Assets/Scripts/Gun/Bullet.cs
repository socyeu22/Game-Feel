using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField] private float _moveSpeed = 10f;
    [SerializeField] private int _damageAmount = 1;
    [SerializeField] private float _knockbackThrust = 20f;
    [SerializeField] private GameObject _hitVFX;

    private Vector2 _fireDirection;

    private Rigidbody2D _rigidBody;

    private Gun _gun;

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
        Health health = other.gameObject.GetComponent<Health>();
        health?.TakeDamage(_damageAmount);

        Knockback knockback = other.gameObject.GetComponent<Knockback>();
        knockback?.GetKnockBack(PlayerController.Instance.transform.position,_knockbackThrust);

        Flash flash = other.gameObject.GetComponent<Flash>();
        flash?.StartFalsh();

        GameObject hitVFX = Instantiate(_hitVFX, transform.position, transform.rotation);

        _gun.ReleaseBulletFromPool(this);

    }
}