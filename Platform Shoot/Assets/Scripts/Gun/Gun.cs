using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;
using UnityEngine.Pool;
using Cinemachine;
using Cinemachine.Editor;

public class Gun : MonoBehaviour
{
    public static Action OnShoot; // Sử dụng Action để tạo một Observer Pattern, khi súng bắn thì sẽ gọi tất cả các hàm mà đã đăng ký vào OnShoot


    public Transform BulletSpawnPoint => _bulletSpawnPoint;
    [SerializeField] private Transform _bulletSpawnPoint;
    [SerializeField] private Bullet _bulletPrefab;
    [SerializeField] private float _gunFireCD = .5f; // Biến này dùng để điều chỉnh tốc độ bắn của súng(thời gian giữa 2 lần bắn)
    [SerializeField] private GameObject _muzzleFlash; // Hiệu ứng chớp sáng khi bắn
    [SerializeField] private float _muzzleFlashTime; // Thời gian chớp sáng khi bắn

    private Vector2 _mousePos;
    private float _lastFireTime = 0f; // Biến này dùng để lưu thời gian bắn cuối cùng của súng
    private static readonly int FIRE_HASH = Animator.StringToHash("Fire"); // Sư dụng để lấy Hash của Animation trong Animator, việc này giúp tối ưu hiệu suất mỗi lần gọi hàm Play của Animator, vì nó sẽ truyền vào Hash thay vì chuỗi

    private CinemachineImpulseSource _impulseSource; // Đối tượng này dùng để tạo hiệu ưng rung cho camera khi bắn
    private Animator _animator; // Animator của súng

    private ObjectPool<Bullet> _bulletPool; //Pool để tái sử dụng bullet sử dụng thư viện ObjectPool trong UnityEngine.Pool
    
    private void Awake()
    {
        _animator = GetComponent<Animator>(); // Lấy đối tượng Animator từ GameObject
        _impulseSource = GetComponent<CinemachineImpulseSource>();// Lấy đối tượng CinemachineImpulseSource từ GameObject
    }

    private void Start()
    {
        CreateBulletPool();
    }
    private void Update()
    {   

        Shoot(); // Gọi liên tục trong hàm update
        RotateGun(); // Gọi liên tục trong hàm update
    }

    public void ReleaseBulletFromPool(Bullet bullet)
    {
        _bulletPool.Release(bullet);
    }
    private void CreateBulletPool()
    {
        _bulletPool = new ObjectPool<Bullet>(() => { //Biểu thức lambda để khởi tạo viên đạn khi pool rỗng
                return Instantiate(_bulletPrefab); // Khởi tạo viên đạn mới
            }, bullet => { // Biểu thức lambda này sẽ được gọi khi lấy viên đạn từ Pool
                bullet.gameObject.SetActive(true);
            }, bullet => { //Biểu thức lambda này sẽ được gọi khi trả về pool
                bullet.gameObject.SetActive(false);
            }, bullet => { //Biểu thức lambda này sẽ được gọi khi viên đạn không thể đưa trở lại pool(có thể do vượt quá số lượng trong pool)
                Destroy(bullet);
            },
            false, //Không kiểm tra trùng lặp trong pool
            20, //Số lượng ban đầu của pool
            40 // Số lượng tối đa của pool
            
        );
    }
    private void OnEnable() {
        OnShoot += ShootProjectile;//Đăng ký hàm ShootProjectile vào sự kiện OnShoot(như một người quan sát trong Observer Pattern)
        OnShoot += ResetLastFireTime;//Đăng ký hàm ShootProjectile vào sự kiện OnShoot(như một người quan sát trong Observer Pattern)
        OnShoot += FireAnimation; // Đăng ký hàm FireAnimation vào sự kiện OnShoot(như một người quan sát trong Observer Pattern)
        OnShoot += GunScreenShake; // Đăng ký hàm ShakeCamera vào sự kiện OnShoot(như một người quan sát trong Observer Pattern)
        OnShoot += MuzzleFlashHandler;
    }

    private void OnDisable() {
        OnShoot -= ShootProjectile; // Hủy đăng ký hàm ShootProjectile vào sự kiện OnShoot(như một người quan sát trong Observer Pattern)
        OnShoot -= ResetLastFireTime; // Hủy đăng ký hàm ShootProjectile vào sự kiện OnShoot(như một người quan sát trong Observer Pattern)
        OnShoot -= FireAnimation; // Hủy đăng ký hàm FireAnimation vào sự kiện OnShoot(như một người quan sát trong Observer Pattern)
        OnShoot -= GunScreenShake; // Hủy đăng ký hàm ShakeCamera vào sự kiện OnShoot(như một người quan sát trong Observer Pattern)
        OnShoot -= MuzzleFlashHandler;
    }

    private void Shoot()
    {
        if (Input.GetMouseButton(0) && Time.time > _lastFireTime) // So sánh thời gian hiện tại với thời gian bắn cuối cùng của súng, nếu thời gian hiện tại lớn hơn _lastFireTime(_lastFireTime này đã được tăng lên 1 phần bằng đúng _gunFireCD) thì sẽ bắn
        {
            OnShoot?.Invoke(); // Gọi tất cả các hàm đã đăng ký vào sự kiện OnShoot(Tất cả các hàm đã đăng ký sẽ được gọi khi súng bắn trong đó có hàm ShootProjectile và ResetLastFireTime)
        }
    }

    //Hàm sinh viên đạn và thiết lập những thuộc tính cần thiết cho viên đạn khi được sinh ra
    private void ShootProjectile()
    {
        // Bullet newBullet = Instantiate(_bulletPrefab, _bulletSpawnPoint.position, Quaternion.identity); // Khởi tạo viên đạn tại vị trí của _bulletSpawnPoint
        Bullet newBullet = _bulletPool.Get(); // Lấy một viên đạn từ Pool
        newBullet.Init(this, _bulletSpawnPoint.position, _mousePos); // Thiết lập hướng bay của viên đạn dựa theo vị trí của chuột và vị trí khởi tạo viên đạn
    }

    // Hàm chạy Animation bắn của súng
    private void FireAnimation()
    {
        _animator.Play(FIRE_HASH, 0, 0); // Chạy Animation bắn của súng sử dụng Hash của Animation
    }

    private void GunScreenShake()
    {
        _impulseSource.GenerateImpulse(); // Tạo hiệu ứng rung cho camera khi bắn
    }

    private void ResetLastFireTime()
    {
        _lastFireTime = Time.time + _gunFireCD; // Cập nhật thời gian bắn cuối cùng của súng dựa theo thời gian hiện tại  cộng thêm một khoảng là _gunFireCD

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


    private IEnumerator MuzzleFlash()
    {
        if(_muzzleFlash.activeSelf) yield break; // Nếu hiệu ứng chớp sáng đang hoạt động thì thoát khỏi hàm
        _muzzleFlash.SetActive(true);
        yield return new WaitForSeconds(_muzzleFlashTime);
        _muzzleFlash.SetActive(false);
    }

    private void MuzzleFlashHandler()
    {
        StartCoroutine(MuzzleFlash());
    }
}
