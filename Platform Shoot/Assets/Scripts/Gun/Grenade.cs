using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using Cinemachine;

public class Grenade : MonoBehaviour
{
    // Sự kiện sẽ chỉ nhận vào quả lựu đạn hiện tại (Grenade)
    public Action OnExplode;
    public Action OnBeep;
    
    [SerializeField] private GameObject _explodeVFX;
    [SerializeField] private GameObject _grenadeLight;
    [SerializeField] private float _launchForce = 10f;
    [SerializeField] private float _torqueAmount = 2f;
    [SerializeField] private int _damageAmount = 3; 
    [SerializeField] private float _explosionRadius = 5f;
    [SerializeField] private LayerMask _enemyLayer;


    [SerializeField] private float _lightBlinkTime = 0.15f;
    [SerializeField] private float _totalBlinks = 3f;
    [SerializeField] private float _explodeTime = 3f;
    private int _currentBinks;
    private CinemachineImpulseSource _cinemachineImpulseSource;

    private Rigidbody2D _rigidBody;



    private void Awake()
    {
        _rigidBody = GetComponent<Rigidbody2D>();
        _cinemachineImpulseSource = GetComponent<CinemachineImpulseSource>();
    }

    private void Start() {
        LaunchGrenade();
        StartCoroutine(CoudownExplodeRoutine());
    }

    private void OnEnable()
    {
        OnExplode += Explosion;
        OnExplode += GrenadeScreenShake;
        OnExplode += DamageNearby;
        OnExplode += AudioManager.Instance.Grenade_OnExplode;

        OnBeep += BlinkLight;
        OnBeep += AudioManager.Instance.Grenade_OnBeep;
    }

    private void OnDisable()
    {
        OnExplode -= Explosion;
        OnExplode -= GrenadeScreenShake;
        OnExplode -= DamageNearby;
        OnExplode -= AudioManager.Instance.Grenade_OnExplode;

        OnBeep -= BlinkLight;
        OnBeep -= AudioManager.Instance.Grenade_OnBeep;

    }

    public void LaunchGrenade()
    {
        Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 directionToMouse = (mousePos - (Vector2)transform.position).normalized;
        _rigidBody.AddForce(directionToMouse * _launchForce, ForceMode2D.Impulse);
        _rigidBody.AddTorque(_torqueAmount, ForceMode2D.Impulse);
    }



    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.GetComponent<Enemy>())
        {
            OnExplode?.Invoke(); // Kích hoạt vụ nổ cho lựu đạn hiện tại
        }
    }

    private void Explosion()
    {
        Instantiate(_explodeVFX, transform.position, transform.rotation);
        Destroy(gameObject);
    }

    private void GrenadeScreenShake()
    {
        _cinemachineImpulseSource.GenerateImpulse();
    }

    private void DamageNearby() {
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, _explosionRadius, _enemyLayer); // Tham số thứ 3 trong hàm này là một layer mask để xác định những object nào sẽ bị ảnh hưởng bởi vụ nổ, những obj thuộc layer này sẽ được trả về trong mảng hits(là những collider2D)
        foreach (Collider2D hit in hits) {
            Health health = hit.GetComponent<Health>();
            health.TakeDamage(_damageAmount);
        }
    }

    private IEnumerator CoudownExplodeRoutine() {
        while (_currentBinks < _totalBlinks) {
            yield return new WaitForSeconds(_explodeTime / _totalBlinks);
            OnBeep?.Invoke();
            yield return new WaitForSeconds(_lightBlinkTime);
            _grenadeLight.SetActive(false);
        }
        OnExplode?.Invoke();
    }

    private void BlinkLight() {
        _grenadeLight.SetActive(true);
        _currentBinks++;
    }
}
