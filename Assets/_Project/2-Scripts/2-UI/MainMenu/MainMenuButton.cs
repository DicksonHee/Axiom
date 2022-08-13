using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Events;
using Random = UnityEngine.Random;

namespace Axiom.UI.MainMenu
{
    public class MainMenuButton : MonoBehaviour
    {
        public Camera cam;
        public string buttonName;
        public List<GameObject> buttonLetters;
    
        public UnityEvent OnClickEvent;

        private void Awake()
        {
            foreach (GameObject obj in buttonLetters)
            {
                int direction = Random.Range(0, 2) == 0 ? 1 : -1;
                obj.transform.DOLocalRotate(new Vector3(0f, direction * 360f, 0f), 5f, RotateMode.FastBeyond360).SetLoops(-1, LoopType.Incremental).SetEase(Ease.Linear);
            }
        }

        private void Update()
        {
            if (!Input.GetMouseButtonDown(0) ||
                !Physics.Raycast(cam.ScreenPointToRay(Input.mousePosition), out RaycastHit hit)) return;
            if (hit.collider.name == buttonName) Debug.Log(gameObject.name);//OnClickEvent?.Invoke();
        }
    }
}
