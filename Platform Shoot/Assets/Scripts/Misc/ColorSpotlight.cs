using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PlayerLoop;
using UnityEngine.Rendering.Universal;

public class ColorSpotlight : MonoBehaviour
{
    [SerializeField] private GameObject _spotlightHead; // Đối tượng đại diện cho đầu của đèn
    [SerializeField] private float _rotationSpeed = 20f; // Tốc độ quay của đèn
    [SerializeField] private float _discoRotationSpeed = 120f; // Tốc độ quay của đèn khi bật chế độ disco
    [SerializeField] private float _maxRotation = 45f; // Góc quay tối đa của đèn
    // [SerializeField] Color[] colors;


    private float _currenRotation; // Giá trị dùng để tính toán trong hàm Mathf.PingPong


    private void Start() {
        RandomStartingRotation(); // Gọi hàm RandomStartingRotation để đặt góc quay ban đầu của đèn
    }
    private void Update() {
        RotateHead();
    }
    // Hàm xoay đèn theo biên độ _maxRotation
    private void RotateHead() {
        _currenRotation += Time.deltaTime * _rotationSpeed; // Tăng giá trị _currenRotation theo thời gian
        float z = Mathf.PingPong(_currenRotation, _maxRotation); // Sử dụng hàm Mathf.PingPong để lấy giá trị góc quay từ 0 đến _maxRotation
        _spotlightHead.transform.localRotation = Quaternion.Euler(0, 0, z); // Đặt góc quay cho đèn
    }
    // private void SetRandomColor() {
    //     int randomIndex = Random.Range(0, colors.Length);
    //     Color randomColor = colors[randomIndex];
    //     _light2D.color = randomColor;
    // }

    // Hàm này sẽ đặt góc quay ban đầu của đèn ngẫu nhiên
    private void RandomStartingRotation() {
        float randomStartingZ = Random.Range(-_maxRotation, _maxRotation); // Lấy một giá trị ngẫu nhiên từ -_maxRotation đến _maxRotation
        _spotlightHead.transform.localRotation = Quaternion.Euler(0, 0, randomStartingZ); // Đặt góc quay cho đèn
        _currenRotation = randomStartingZ + _maxRotation; // Gán giá trị _currenRotation để tính toán trong hàm Mathf.PingPong
    }


    public IEnumerator SpotLightDiscoParty(float discoPartyTime) {
        float defautRotationSpeed = _rotationSpeed;
        _rotationSpeed = _discoRotationSpeed;
        yield return new WaitForSeconds(discoPartyTime);
        _rotationSpeed = defautRotationSpeed;
    }

}
