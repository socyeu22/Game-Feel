using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Knockback : MonoBehaviour
{
    public Action OnKnockbackStart; // Sự kiện này sẽ được gọi khi đối tượng bị đầy lùi( sử dụng observer), có thể gọi được ở nhiều nơi(các lớp khác)
    public Action OnKnockbackEnd; // Sự kiện này sẽ được gọi khi đối tượng hết bị đầy lùi( sử dụng observer), có thể gọi được ở nhiều nơi(các lớp khác)
    [SerializeField] private float _knockbackTime = 0.2f; // Thời gian bị đầy lùi

    private Vector3 _hitDirection; // Hướng bị đầy lùi
    private float _knockbackThrust = 10f; //Lực tác động khi bị đẩy lùi

    private Rigidbody2D _rigidBody; //Rigidbody của đối tượng được gắn script(enemy)

    private void Awake(){
        _rigidBody = GetComponent<Rigidbody2D>();
    }

    private void OnEnable(){
        OnKnockbackStart += ApplyKnockbackForce;
        OnKnockbackEnd += StopKnocRoutine; 
    }

    private void OnDisable(){
        OnKnockbackStart -= ApplyKnockbackForce;
        OnKnockbackEnd -= StopKnocRoutine;//
    }

    public void GetKnockBack(Vector3 hitDirection, float knockbackThrust){
        _hitDirection = hitDirection;
        _knockbackThrust = knockbackThrust;

        OnKnockbackStart?.Invoke(); // Gọi sự kiên OnKnockbackStart
    }

    private void ApplyKnockbackForce(){
        Vector3 difference = (transform.position - _hitDirection).normalized * _knockbackThrust * _rigidBody.mass; // Tính hướng và lực tác động khi bị đẩy lùi dựa trên trọng lượng của đối tượng
        _rigidBody.AddForce(difference, ForceMode2D.Impulse); // Áp dụng lực tác động lên đối tượng
        StartCoroutine(KnockRoutine()); // Bắt đầu coroutine KnockRoutine
    }

    private IEnumerator KnockRoutine(){
        yield return new WaitForSeconds(_knockbackTime); // Chờ trong thời gian _knocbackTime
        OnKnockbackEnd?.Invoke(); // Gọi sự kiện OnKnockbackEnd
    }

    private void StopKnocRoutine(){
        _rigidBody.velocity = Vector2.zero;  // Đặt vận tốc của đối tượng về 0
    }
}
