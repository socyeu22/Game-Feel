using UnityEngine;

[CreateAssetMenu()]
public class SoundSO : ScriptableObject
{
    public enum AudioTypes {
        SFX,
        Music
    }

    public AudioTypes AudioType;
    public AudioClip Clip;
    public bool Loop = false;
    public bool RandommizePitch = false; // Biến này dùng để random cao độ của âm thanh khi phát
    [Range(0f, 1f)] // Tạo một slider từ 0 đến 1 để chọn giá trị trong inspector
    public float RandomPitchRangeModifier = 0.1f; // Biến này dùng để lưu khoảng random cao độ của âm thanh khi phát
    [Range(.1f, 12f)] // Tạo một slider từ 0 đến 1 để chọn giá trị trong inspector
    public float Volume = 1f;
    [Range(.1f, 3f)] // Tạo một slider từ 0 đến 1 để chọn giá trị trong inspector
    public float Pitch = 1f;
}
