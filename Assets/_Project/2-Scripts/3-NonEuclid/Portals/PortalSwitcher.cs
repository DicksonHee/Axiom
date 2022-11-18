using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Axiom.NonEuclidean
{
    public class PortalSwitcher : MonoBehaviour
    {
        public void ChangePortal(Portal currentEntry, Portal newExit)
        {
            currentEntry.otherPortal.otherPortal = null;
            //currentExit.otherPortal = null;
            currentEntry.otherPortal = newExit;
            newExit.otherPortal = currentEntry;

            currentEntry.ForceCreateViewTexture();
            newExit.ForceCreateViewTexture();
        }

        //public void ChangePortal()
        //{
        //    currentPortalEntry.otherPortal = newPortalExit;
        //    newPortalExit.otherPortal = currentPortalEntry;
        //    currentPortalExit.otherPortal = null;
        //}
    }

}