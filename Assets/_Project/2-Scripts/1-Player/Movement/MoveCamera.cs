using UnityEngine;

namespace Axiom.Player.Movement
{
    public class MoveCamera : MonoBehaviour
    {
        [SerializeField] private Transform cameraPosition;

        private void Update()
        {
            transform.position = cameraPosition.position;
        }
    }
}
