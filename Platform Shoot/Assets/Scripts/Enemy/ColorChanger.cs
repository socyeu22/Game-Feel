using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColorChanger : MonoBehaviour
{
    public Color DefaultColor {get; private set;} // Biến này sẽ lưu giá trị màu mặc định của đối tượng khi được sinh ra
    [SerializeField] private Color[] _colors; // Mảng chứa các màu sẽ được chọn ngẫu nhiên để gắn cho biến DefaultColor khi mới sinh ra
    [SerializeField] private SpriteRenderer _fillSpriteRenderer; // Sprite renderer của đối tượng con Fill của đối tượng hiện tại


    // Hàm này là hàm tạo màu mặc định cho đối tượng khi mới sinh ra, sẽ được gọi từ hàm Init() của lớp Enemy.cs
    public void SetDefaultColor(Color color) {
        DefaultColor = color; // Gán giá trị màu mặc định cho đối tượng từ tham số truyền vào color
        SetColor(color); // Gọi hàm SetColor() để gám màu cho _fillSpriteRenderer(sẽ đổi màu của đối tượng con Fill)
    }
    
    // Hàm này sẽ gán màu cho _fillSpriteRenderer, hàm này sẽ được gọi trong hàm gán màu mặc định để tạo màu mặc định hoặc gọi khi đối tượng bị trúng đạn để tạo hiệu ứng flash
    public void SetColor(Color color){
        _fillSpriteRenderer.color = color;
    }

    // Hàm chọn ngãu nhiên một màu trong mảng _colors để gán cho biến DefaultColor đồng thời cũng gán luôn màu đó cho _fillSpriteRenderer
    public void SetRandomColor() {
        int randomNum = Random.Range(0, _colors.Length);
        DefaultColor = _colors[randomNum];
        _fillSpriteRenderer.color = DefaultColor;
    }
}
