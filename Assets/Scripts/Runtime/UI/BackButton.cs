using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using System.Collections;
using XCharts.Runtime;

namespace UnityEngine.XR.ARFoundation.Samples
{
    public class BackButton : MonoBehaviour
    {
        [SerializeField]
        GameObject m_BackButton;

        public GameObject backButton
        {
            get => m_BackButton;
            set => m_BackButton = value;
        }

        [SerializeField]
        GameObject m_LineChart;

        public GameObject LineChart
        {
            get => m_LineChart;
            set => m_LineChart= value;
        }

        void Start()
        {
            //if (Application.CanStreamedLevelBeLoaded(MenuLoader.GetMenuSceneName()))
            m_BackButton.SetActive(true);
        }

        void Update()
        {
            // Handles Android physical back button
            if (Keyboard.current != null && Keyboard.current.escapeKey.wasPressedThisFrame)
                BackButtonPressed();
        }

        public void BackButtonPressed()
        {

            if (m_LineChart.gameObject.activeSelf)
                m_LineChart.GetComponent<LineChart>().ClearData();

            m_LineChart.gameObject.SetActive(!m_LineChart.activeSelf);

            //string menuSceneName = MenuLoader.GetMenuSceneName();
            //if (Application.CanStreamedLevelBeLoaded(menuSceneName))
            //    SceneManager.LoadScene(menuSceneName, LoadSceneMode.Single);
        }
    }
}
