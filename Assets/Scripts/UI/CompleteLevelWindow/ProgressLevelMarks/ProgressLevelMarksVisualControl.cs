using UnityEngine;


namespace Arenar.Services.UI
{
    public class ProgressLevelMarksVisualControl : MonoBehaviour
    {
        [SerializeField] private SerializableDictionary<LevelMarkType, ProgressLevelMark> _levelMarks;


        public void SetMarkSuccessStatus(LevelMarkType markType, bool status)
        {
            if (!_levelMarks.ContainsKey(markType))
            {
                Debug.LogError($"Unknown type {markType}");
                return;
            }
            
            _levelMarks[markType].SetMarkSuccessStatus(status);
        }
    }
}