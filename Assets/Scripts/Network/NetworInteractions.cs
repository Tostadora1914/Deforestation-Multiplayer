using Deforestation.Interaction;
using Deforestation.Machine;
using Photon.Pun;
using Unity.VisualScripting;
using UnityEngine;

namespace Deforestation.Network
{

	public class NetworInteractions : MachineInteraction
	{
		#region Fields
        private PhotonView _photonView;
        #endregion

        #region Properties
        #endregion

        #region Unity Callbacks
        #endregion

        #region Private Methods
        #endregion

        #region Public Methods

        // Después de hacer un método "virutal" (en otro script), si tu intención es sobrescribir el método, deberás de escribir "override":
        public override void Interact()
		{
			if (_type == MachineInteractionType.Door)
			{
				//Move Door
				transform.position = _target.position;
			}
			if (_type == MachineInteractionType.Stairs)
			{
				//Teleport Player
				GameController.Instance.TeleportPlayer(_target.position);
			}
			if (_type == MachineInteractionType.Machine) // El panel de control de la Máquina:
            {
                MachineController machine = _target.GetComponent<MachineController>();
				Transform follow = _target.GetComponent<NetworkMachine>()._machineFollow;
				(NetworkGameController.Instance as NetworkGameController).InitializeMachine(follow, machine);
				GameController.Instance.MachineMode(true);
            }
            //else
            //             _networkPlayer._3dAvatar.SetActive(true);

        }


        

		#endregion

	}

}