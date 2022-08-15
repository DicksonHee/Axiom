using UnityEngine;
using UnityEngine.VFX;

namespace Axiom.Player.Movement
{
   public class MovementVFX : MonoBehaviour
   {
      [SerializeField] private VisualEffect speedLineVFX;

      public void SetSpeedLineSpawnRate(float rate)
      {
         speedLineVFX.SetFloat("SpawnRate", rate);
      }
   }
}
