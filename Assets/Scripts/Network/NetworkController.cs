using Deforestation.Machine;
using Deforestation.UI;
using Photon.Pun;
using Photon.Realtime;
using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Deforestation.Network
{

	public class NetworkController : MonoBehaviourPunCallbacks
	{
		[SerializeField] private GameObject _explosionPrefab;
		[SerializeField] private UINetwork _ui; // Para mostrar el panel de muerte.
		[SerializeField] private UIGameController _uIGameController;

        //Master
        // Lista que contiene los punto de spawneo:
        [SerializeField] private List <Transform> _spawnPoints;
		private int _indexSpawns = 0;

		//Client
		private bool _waitingForSpawn = false;

		private GameObject _machine;
		private GameObject _player;

		void Start()
		{
            // Se conectará utilizando las configuraciones predeterminadas:
            PhotonNetwork.ConnectUsingSettings();
        }

        // Se lanza este método cuando nos conectamos al server:
        public override void OnConnectedToMaster()
		{
            // PhotonNetwork.GetCustomRoomList(); -> Para mostrar las salas disponibles (obtienes una lista).
            // PhotonNetwork.CreateRoom(); -> Para crear una salas.
            // PhotonNetwork.JoinRoom(); -> Para crear una salas.
            // PhotonNetwork.CreateRoom(); -> Para crear una salas.

            // Podré unirme o crear una sala:

            // Te permite unirte a una sala, o crearla si no existe:
				// "DeforestationRoom" -> Nombre de la sala.
				// MaxPlayers -> Número máximo de Players en la sala ().
            PhotonNetwork.JoinOrCreateRoom("DeforestationRoom", new RoomOptions { MaxPlayers = 10 }, null);

            // Instancias al Player con las siguientes sobrecargas:
				// Nombre del objeto a instanciar (debe estar en Resources), en este caso es "PlayerMultiplayer".
				// La posición exacta (inicial) donde se instanciará el Player.
				// La rotación inicial del Player (Quaternion.identity = 0, 0, 0).
            //PhotonNetwork.Instantiate("PlayerMultiplayer", new Vector3(1262.54f, 150.09f, 681.99f), Quaternion.identity);
        }


        // Se llama cuando te unes a una sala:
        public override void OnJoinedRoom()
		{
            // Si somos nosotros los que hosteamos la partida, :
            if (PhotonNetwork.IsMasterClient)
			{
                // Llamas al método "SpawnMe", pasándole como parámetro el primer punto de spawneo:
                SpawnMe(_spawnPoints[0].position);
				_indexSpawns = 1;
			}

            // Si no somos "MasterClient", :
            else
            {
                // Los demás clientes le pedierán al "MasterClient" que instancie su Player:
                _waitingForSpawn = true;
				photonView.RPC("RPC_SpawnPoint", RpcTarget.MasterClient);
                

            }

            // Llamas al método "LoadingComplete" del script "UINetwork":
            _ui.LoadingComplete();
        }

        private void SpawnMe(Vector3 spawnPoint)
		{
            // Instancias al Player:
            _player = PhotonNetwork.Instantiate("PlayerMultiplayer", spawnPoint, Quaternion.identity);
            // Instancias a la Máquina:
            _machine = PhotonNetwork.Instantiate("TheMachine", spawnPoint + Vector3.back * 7, Quaternion.identity);
			
			//dead control
			_player.GetComponent<HealthSystem>().OnDeath += PlayerDie;
			_machine.GetComponent<HealthSystem>().OnDeath += MachineDie;

			_uIGameController.enabled = true;
        }


        [PunRPC]
		void RPC_SpawnPoint()
        {    
			 // Cuando se elimina un índice de una lista, la lista cambia de orden. 
             // Others -> Los Players que no son locales.
            _indexSpawns++;
			if (_indexSpawns >= _spawnPoints.Count)
				_indexSpawns = 0;
			photonView.RPC("RPC_RecivePont", RpcTarget.Others, _spawnPoints[_indexSpawns].position);
			
		}

		[PunRPC]
		void RPC_RecivePont(Vector3 spawnPos)
		{
			if (_waitingForSpawn)
			{
				_waitingForSpawn = false;
				SpawnMe(spawnPos);
			}
		}

		private void MachineDie()
		{            
            // Si resulta que estábamos conduciendo la Máquina cuando su vida era 0, :
            if (GameController.Instance.MachineModeOn)
			{
				// El método recibirá "false", es decir, dejaremos de conducir (obligatoriamente):
				GameController.Instance.MachineMode(false);
                // Le pasaremos 10000 de daño al Player (no muy efectivo, pero es para asegurarnos de que muera):
                _player.GetComponent<HealthSystem>().TakeDamage(1000);
			}
			// Se spanearán explosiones alrededor de la Máquina al morir (4 metros, 5 explosiones, distancia máxima = 5):
			SpawnExplosions(_machine.transform.position + Vector3.up * 4, 5, 5);

            PhotonNetwork.Destroy(_machine);
            photonView.RPC("NumberOfMachines", RpcTarget.All);

            //DestroyImmediate(_machine);

        }

        public void SpawnExplosions(Vector3 centerPoint, int numberOfExplosions = 4, float maxDistance = 5f)
		{
			for (int i = 0; i < numberOfExplosions; i++)
			{
				Vector3 randomDirection = Random.insideUnitSphere;				
				Vector3 spawnPosition = centerPoint + randomDirection.normalized * Random.Range(0f, maxDistance);
				Instantiate(_explosionPrefab, spawnPosition, Quaternion.identity);
			}
		}
		private void PlayerDie()
		{
			// Configuración del cursor:
			Cursor.lockState = CursorLockMode.None;
			Cursor.visible = true;

			// Se muestra el panel de muerte:
			_ui.EndGamePanel.SetActive(true);
		}

		[PunRPC]
        private void NumberOfMachines()
		{
			NetworkMachine[] machines = FindObjectsOfType<NetworkMachine>();
            if (machines.Length == 1)
			{
				//Debug.Log("Queda una máquina.");
				_ui.LastMachine.SetActive(true);
            }
			else
                _ui.LastMachine.SetActive(false);
        }

    }
}