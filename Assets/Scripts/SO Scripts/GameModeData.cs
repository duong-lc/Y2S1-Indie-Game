using Sirenix.OdinInspector;
using UnityEngine;

namespace SO_Scripts
{
    [CreateAssetMenu(fileName = "GameModeData", menuName = "ScriptableObjects/GameModeData", order = 0)]
    public class GameModeData : ScriptableObject
    {
        [TitleGroup("Scene Data")]
        public string mainMenuSceneName;
        public string levelSelectionSceneName;
        public string gamePlaySceneName;
        public string optionSceneName;
        
        [TitleGroup("Margin Of Error")]
        [Header("Early Margin Of Error")]
        public float earlyMOEBegin;
        public float earlyMOEEnd;
        
        [Header("Perfect Margin Of Error")]
        public float perfectMOEBegin;
        public float perfectMOEEnd;
        
        [Header("Late Margin Of Error")]
        public float lateMOEBegin;
        public float lateMOEEnd;

        [TitleGroup("Key Input")] 
        public KeyCode inputLane1;
        public KeyCode inputLane2;
        public KeyCode inputLane3;
        public KeyCode inputLane4;
        
        
    }
}