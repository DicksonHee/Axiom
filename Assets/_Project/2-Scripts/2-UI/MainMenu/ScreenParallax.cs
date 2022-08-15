using UnityEngine;

namespace Axiom.UI.MainMenu
{
    public class ScreenParallax : MonoBehaviour
    {
        private Vector3 initialPos;
    
        private float halfHeight;
        private float halfWidth;

        private float currentXValue;
        private float currentYValue;
    
        private void Awake()
        {
            initialPos = transform.position;
            halfHeight = Screen.height * 0.5f;
            halfWidth = Screen.width * 0.5f;

            Cursor.lockState = CursorLockMode.Confined;
        }

        // Update is called once per frame
        void Update()
        {
            currentXValue = (Input.mousePosition.x - halfWidth) / halfWidth;
            currentYValue = (Input.mousePosition.y - halfHeight) / halfHeight;
        
            transform.position = initialPos + new Vector3(currentXValue, currentYValue) * 0.5f;
        }
    }
}
