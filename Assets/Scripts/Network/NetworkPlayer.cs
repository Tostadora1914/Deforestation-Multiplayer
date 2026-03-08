using Deforestation.Interaction;
using Deforestation.Recolectables;
using Photon.Pun;
using StarterAssets;
using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Deforestation.Network
{
	public class NetworkPlayer : MonoBehaviourPun
	{
        #region Fields

        // Referencias a la mayoría (por no decir todos) de componentes del Player:
        [SerializeField] private HealthSystem _health;
		[SerializeField] private Inventory _inventory;
		[SerializeField] private InteractionSystem _interactions;
		[SerializeField] private CharacterController _controller;
		[SerializeField] private FirstPersonController _fps;
		[SerializeField] private StarterAssetsInputs _inputs;
		[SerializeField] private PlayerInput _inputsPlayer;
		[SerializeField] public GameObject _3dAvatar;
		[SerializeField] private Transform _playerFollow; // Es el transform que la cámara seguirá.
        private NetworkGameController _gameController;
		private Animator _anim;
		#endregion

		#region Properties
		#endregion

		#region Unity Callbacks	
		private void Awake() // No se puede preguntar si somos locales en el "Awake".
        {
            // Que se almacene el componente "Animator" del objeto que contenga el script:
            _anim = _3dAvatar.GetComponent<Animator>(); // El "Animator" está en el modelo.
        }
		private void Start()
		{
            // Se buscará en la escena el "NetworkGameController" (INCLUSO EN LOS OBJETOS DESACTIVADOS (true)):
            _gameController = FindObjectOfType<NetworkGameController>(true);
            // Si localmente estás controlando a tu Player, :
            if (photonView.IsMine)
			{
                // Inicializamos el Player local con las variables del "GameController":
                // Además, le pasamos al método los componentes del Player (en este caso):
                _gameController.InitializePlayer(_health, _controller, _inventory, _interactions, _playerFollow);

                // Un par de suscripciones a eventos del Sistema de Salud:
                _health.OnHealthChanged += Hit;
				_health.OnDeath += Die;
				
				_health.enabled = true;
				_inventory.enabled = true;
				_interactions.enabled = true;
				_fps.enabled = true;
				_controller.enabled = true;

				Invoke(nameof(AddInitialCrystals), 1);

            }

            // Si no estoy siendo controlado de manera local, :
            else
            {
				DisconectPlayer();
			}
		}

		private void AddInitialCrystals()
		{
			_inventory.AddRecolectable(RecolectableType.SuperCrystal, 7);
			_inventory.AddRecolectable(RecolectableType.HyperCrystal, 3);
		}

		private void DisconectPlayer()
		{
            //if (_gameController.OnMachineModeChange != null)
            //    photonView.RPC("SeePlayer", RpcTarget.Others);
            //photonView.RPC("SeePlayer", RpcTarget.Others);
            // No queremos que el Player local sufra distintos daños en las demás realidades:
            Destroy(_health);
			Destroy(_inventory);
			Destroy(_interactions);
			Destroy(_fps);
			Destroy(_controller);
			Destroy(_inputs);
			Destroy(_inputsPlayer);
            //Destroy(_3dAvatar); //TODO: Lo puse yo (Elimina el avatar del jugador que tu no controlas).
        }
        private void Update()
		{
            // SOLAMENTE SI ES MIO/MIA, :
            if (photonView.IsMine)
			{
                // Si nos estamos moviendo en cualquier eje, :
                if (Input.GetAxis("Horizontal") != 0 || Input.GetAxis("Vertical") != 0)
				{
                    // Ejecutamos la animación de correr:
                    _anim.SetBool("Run", true);
				}
				else
				{
                    // Si no, dejamos de ejecutar la animación de correr:
                    _anim.SetBool("Run", false);
				}
				if (Input.GetKeyUp(KeyCode.Space))
					_anim.SetTrigger("Jump");  // Ejecutamos la animación de saltar.
			}
			
		}

        #endregion

        #region Private Methods


        private void Die()
		{
			_anim.SetTrigger("Die");
			DisconectPlayer();
			this.enabled = false;
		}

		private void Hit(float obj)
		{
			_anim.SetTrigger("Hit");
		}

		[PunRPC]

		private void ShowAvatar(bool show) // Si comentas este void, ocurrirán errores.
		{
			_3dAvatar.SetActive(show);
		}
		#endregion

		#region Public Methods
		#endregion

	}

}