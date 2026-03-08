
using Deforestation.UI;
using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Deforestation.Network
{
	public class UINetwork : MonoBehaviour
	{
        #region Fields
        // Referencias al panel de conexión:
        [SerializeField] private GameObject _connectingPanel;
		[SerializeField] private Button _exitButton;
		[SerializeField] private Button _retryButton;

		#endregion

		#region Properties
		public GameObject EndGamePanel;
		public GameObject LastMachine;
		#endregion

		#region Unity Callbacks	
		private void Awake()
		{
			_exitButton.onClick.AddListener(Exit);
			_retryButton.onClick.AddListener(Retry);
		}

		private void Retry()
		{
			SceneManager.LoadScene(0);
		}

		private void Exit()
		{
			Application.Quit();
		}
		#endregion

		#region Private Methods
		#endregion

		#region Public Methods
		public void LoadingComplete()
		{
            // Desactivamos el panel de conexión:
            _connectingPanel.SetActive(false);
		}
		#endregion

	}
}
