using UnityEngine;

namespace SO_Scripts
{
    [CreateAssetMenu(fileName = "GameModeData", menuName = "ScriptableObjects/GameModeData", order = 0)]
    public class GameModeData : ScriptableObject
    {
        public string mainMenuSceneName;
        public string levelSelectionSceneName;
        public string gamePlaySceneName;
        public string optionSceneName;

    }
}