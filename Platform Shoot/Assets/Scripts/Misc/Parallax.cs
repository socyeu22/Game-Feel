using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Parallax : MonoBehaviour
{
    [SerializeField] private float _parallaxOffset = -0.1f; // Hệ số offset của Parallax cho từng layer
    private Vector2 _startPos; // Vị trí ban đầu của layer
    private Camera _mainCamera; // Camera chính của game(theo dõi người chơi)

    private Vector2 _travel => (Vector2)_mainCamera.transform.position - _startPos; // Khoảng cách di chuyển của camera so với vij trí ban đầu của layer, cập nhật mỗi frame

    private void Awake() {
        _mainCamera = Camera.main; // Lấy camera chính của game
    }

    private void Start() {
        _startPos = transform.position; // Vị trí ban đầu của layer
    }

    private void FixedUpdate() {
        Vector2 newPosition = _startPos + new Vector2(_travel.x * _parallaxOffset, 0f); // Tính vị trí mới của layer bằng cách cộng vị trí ban đầu với khoảng cách di chuyển của camera nhân với hệ số offset(mỗi layer sẽ có hệ số offset khác nhau layer càng xa camera thì hệ số càng nhỏ có nghĩa là di chuyển chậm hơn)
        transform.position = new Vector2(newPosition.x, transform.position.y); // Cập nhật vị trí mới của layer theo trục x và giữ nguyên trục y.
    }
}
