using UnityEngine;
using UnityEngine.UIElements;

public class ScoreUI : MonoBehaviour
{
    private Label score0Label;
    private Label score1Label;
    private void Awake()
    {
        var uiDoc = GetComponent<UIDocument>();
        var root = uiDoc.rootVisualElement;

        score0Label = root.Q<Label>("score-0");
        score1Label = root.Q<Label>("score-1");

        score0Label.text = "Player 1: 0";
        score1Label.text = "Player 2: 0";
    }

    public void UpdateScore(int playerIndex, int score)
    {
        if (playerIndex == 0 && score0Label != null)
            score0Label.text = $"Player 1: {score}";
        else if (playerIndex == 1 && score1Label != null)
            score1Label.text = $"Player 2: {score}";

        if (score == 10)
            Debug.Log($"Player {playerIndex} Win");
    }
}
