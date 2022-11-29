using UnityEngine;

namespace Axiom.Player.Movement
{
    public class MoveCamera : MonoBehaviour
    {
        [SerializeField] private bool _alsoRotate;
        public bool alsoRotate { get { return _alsoRotate; } set { _alsoRotate = value; print(value); } }

        [SerializeField] private Transform cameraPosition;

        private void Update()
        {
            transform.position = cameraPosition.position;
            if(alsoRotate) transform.rotation = cameraPosition.rotation;
        }

        public void ForceUpdate()
        {
            transform.position = cameraPosition.position;
            if (alsoRotate) transform.rotation = cameraPosition.rotation;
        }
    }
}
