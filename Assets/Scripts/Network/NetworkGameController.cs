using Deforestation.Interaction;
using Deforestation.Machine;
using Deforestation.Recolectables;
using Photon.Pun;
using StarterAssets;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Deforestation.Network
{

	public class NetworkGameController : GameController
	{
        #region Fields
        #endregion

        #region Properties
        #endregion

        #region Unity Callbacks	
        #endregion

        #region Private Methods
        #endregion

        #region Public Methods

        // Este método recibe las características principales del Player (para poder inicializarlas):
        public void InitializePlayer(HealthSystem health, CharacterController player, Inventory inventory, InteractionSystem interaction, Transform playerFollow)
		{
            // Le asignamos a las variables las otras variables pasadas (cabe recalcar, que proviene del "GameController"):
            _playerHealth = health;
			_player = player;
			_inventory = inventory;
			_interactionSystem = interaction;
			_playerFollow = playerFollow;
		}

        // Le asignamos a las variables la otras variables pasada (cabe recalcar, que proviene del "GameController"):
        public void InitializeMachine(Transform follow, MachineController machine)
		{
			if (_machine != null) // Si resulta que ya hay una Máquina (Machine Controller) asignada, entonces se desuscribe de su evento de salud:
            {
				_machine.HealthSystem.OnHealthChanged -= _uiController.UpdateMachineHealth;
			}

			_machineFollow = follow;
			_machine = machine;

            // Se suscribe al evento con la nueva asignación de Máquina (Machine Controller) - Revisar notion (La Autoridad de las Armas y la Vida):	
            _machine.HealthSystem.OnHealthChanged += _uiController.UpdateMachineHealth;
			//Para refrescar la UI
			_machine.HealthSystem.TakeDamage(0);
		}

		#endregion

	}

}