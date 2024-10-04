using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gun : MonoBehaviour
{
    public Transform BulletSpawnPoint => _bulletSpawnPoint;
    public Vector2 MousePos => _mousePos;

    [SerializeField] private Transform _bulletSpawnPoint;
    [SerializeField] private Bullet _bulletPrefab;
    private Vector2 _mousePos;
    
    private void Update()
    {   

        Shoot(); // Gọi liên tục trong hàm update
        RotateGun(); // Gọi liên tục trong hàm update
    }

    private void Shoot()
    {
        if (Input.GetMouseButtonDown(0)) {

            ShootProjectile();
        }
    }

    private void ShootProjectile()
    {
        Bullet newBullet = Instantiate(_bulletPrefab, _bulletSpawnPoint.position, Quaternion.identity); // Khởi tạo viên đạn tại vị trí của _bulletSpawnPoint
        newBullet.Init(_bulletSpawnPoint.position, _mousePos); // Thiết lập hướng bay của viên đạn dựa theo vị trí của chuột và vị trí khởi tạo viên đạn
    }


    // Hàm sử dụng để xoay Súng theo hướng của chuột(hoặc sau này là điểm chạm của người chơi)
    private void RotateGun()
    {
        _mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition); // Tính toán vị trí của chuột trong thế giới 2D dựa thêm vào vị trí của Camera.main và điểm chạm của chuột
        // Vector2 direction = _mousePos - (Vector2) PlayerController.Instance.transform.position; // Tính toán hướng của súng dựa theo vị trí của chuột và vị trí của người chơi, cần phải ép kiểu của vị trí của người chơi về Vector2
        Vector2 direction = PlayerController.Instance.transform.InverseTransformPoint(_mousePos); // Đây là một cách tính góc quay của súng khác, chúng ta sẽ sử dụng hàm InverseTransformPoint để chuyển đổi vị trí của chuột từ thế giới 2D về không gian cục bộ của người chơi
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg; // Tính toán góc quay của súng dựa theo vector direction và chuyển đổi từ radian sang độ
        transform.localRotation = Quaternion.Euler(0, 0, angle); // Gán góc quay cho súng
    }
}
