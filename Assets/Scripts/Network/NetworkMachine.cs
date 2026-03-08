
using Deforestation.Machine;
using Photon.Pun;
using System;
using UnityEngine;

namespace Deforestation.Network
{
	public class NetworkMachine : MonoBehaviourPun
	{
        #region Fields

        // Referencias a todos los componentes del Player:
        [SerializeField] private MachineController _machine;
		public Transform _machineFollow;
		private NetworkGameController _gameController;
		#endregion

		#region Properties
		#endregion

		#region Unity Callbacks	
		private void Start()
		{
            // Se buscará en la escena el "NetworkGameController" (INCLUSO EN LOS OBJETOS DESACTIVADOS (true)):
            _gameController = FindObjectOfType<NetworkGameController>(true);
            // Si localmente estás controlando a tu Player, :
            if (photonView.IsMine)
			{
                // Inicializamos el Player local con las variables del "GameController":
                // Además, le pasamos al método los componentes del Player (en este caso):
                _gameController.InitializeMachine(_machineFollow, _machine);
                // Activamos el "GameController" si la Máquina es nuestra (aparte, es la ultima en instanciarse):
                _gameController.gameObject.SetActive(true);
				_machine.enabled = true;
				_machine.WeaponController.enabled = true;
				_machine.GetComponent<MachineMovement>().enabled = true;
				//Autoridad de la vida en local:
				_machine.HealthSystem.OnHealthChanged += SyncHealth; // El evento de cambio de vida se suscribe al método SyncHealth.
				//Autoridad de disparos en local:
                _machine.WeaponController.OnMachineShoot += SyncShoot; // El evento de disparo se suscribe al método SyncHealth.
				//_machine.OnMachineAnimations += SyncAnimations;
            }
			else
			{
				//---
			}
		}

        #endregion

        #region Private Methods
        private void SyncShoot()
		{
			//Capturar la direccion del cañon
			//TODO: refactorizar!
			RaycastHit hit;
			Ray ray = GameController.Instance.MainCamera.ScreenPointToRay(Input.mousePosition); // Guarda la posición del ratón antes del hit.
			Physics.Raycast(ray, out hit); // Genera el raycast desce-hasta.

			//Mandar RPC
			photonView.RPC("OthersShoot", RpcTarget.Others, hit.point);
		}

		[PunRPC]
		private void OthersShoot(Vector3 shootDirection)
		{
			_machine.WeaponController.Shoot(shootDirection);
		}

		private void SyncHealth(float value)
		{
            //			  Nombre del método, Target         , Parámetros
            photonView.RPC("RefreshHealth", RpcTarget.Others, value);
		}

		[PunRPC]
		private void RefreshHealth(float value)
		{
			// Se le pasará un valor de vida al método "SetHealth":
			_machine.HealthSystem.SetHealth(value);
		}
		#endregion

		#region Public Methods
		#endregion

	}

}