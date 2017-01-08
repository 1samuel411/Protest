using UnityEngine;
using System;
using UnityEngine.UI;
using DeadMosquito.IosGoodies;
using System.Collections.Generic;

namespace DeadMosquito.IosGoodies.Example
{
    public class IosGoodiesDemoController : MonoBehaviour
    {
        #if UNITY_IOS
        public GameObject mainMenuPanel;

        public GameObject mapsPanel;
        public GameObject uiPanel;
        public GameObject sharePanel;
        public GameObject openAppsPanel;

        List<GameObject> _windows;

        void Awake()
        {
            InitWindows();
        }

        private void InitWindows()
        {
            _windows = new List<GameObject>
            {
                mainMenuPanel,
                mapsPanel,
                uiPanel,
                sharePanel
            };
            _windows.ForEach(w => w.SetActive(false));
            mainMenuPanel.SetActive(true);
        }

        public void OnMapsPanel()
        {
            mapsPanel.SetActive(true);
        }

        public void OnUiPanel()
        {
            uiPanel.SetActive(true);
        }

        public void OnSharePanel()
        {
            sharePanel.SetActive(true);
        }

        public void OnOpenAppsPanel()
        {
            openAppsPanel.SetActive(true);
        }
        #endif
    }
}