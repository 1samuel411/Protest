using UnityEngine;

namespace AndroidGoodiesExamples
{
    public class EnableOnClickIOS : MonoBehaviour
    {
        public bool enable;

        public GameObject target;

        public void OnClick()
        {
            target.SetActive(enable);
        }
    }
}