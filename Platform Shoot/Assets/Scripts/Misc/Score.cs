using UnityEngine;
using TMPro;

public class Score : MonoBehaviour
{
    private int _score = 0;
    private TextMeshProUGUI _scoreText;

    private void Awake() {
        _scoreText = GetComponent<TextMeshProUGUI>();
    }

    private void OnEnable() {
        Health.OnDeath += EnemyDestroyed;
    }

    private void EnemyDestroyed(Health sender) {
        _score++;
        SetScoreText();
    }

    private void SetScoreText() {
        _scoreText.text = _score.ToString("D3");
    }
}
