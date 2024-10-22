using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flash : MonoBehaviour
{
    [SerializeField] private Material _defaultMaterial; // Material mặc định của object
    [SerializeField] private Material _whiteFlashMaterial; // Material khi trúng đạn của object
    [SerializeField] private float _flashTime = 0.1f; // Thời gian flash của object

    private SpriteRenderer[] _spriteRenderers; //Danh sách cách sprite renderer của object(enemy có nhiều hơn 1 sprite)
    private ColorChanger _colorChanger;

    private void Awake() {
        _spriteRenderers = GetComponentsInChildren<SpriteRenderer>(); //Lấy danh sách các component sprite renderer của các game object con của đối tượng hiện tại sử dụng hàm GetComponentsInChildren (chú ý là có thêm "s" ở cuối để lấy tất cả các component sprite renderer của các game object con)
        _colorChanger = GetComponent<ColorChanger>();
    }

    public void StartFalsh() {
        StartCoroutine(FlashRoutine());
    }

    private IEnumerator FlashRoutine() {
        foreach (SpriteRenderer sr in _spriteRenderers) {
            sr.material = _whiteFlashMaterial;
            if(_colorChanger) {
                _colorChanger.SetColor(Color.white);
            }
        }
        yield return new WaitForSeconds(_flashTime);

        SetDefaultMaterial();
    }

    private void SetDefaultMaterial() {
        foreach (SpriteRenderer sr in _spriteRenderers) {
            sr.material = _defaultMaterial;
            if(_colorChanger) 
                _colorChanger.SetColor(_colorChanger.DefaultColor);
        }
    }
}
