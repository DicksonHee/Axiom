using System.Collections;
using DG.Tweening;
using UnityEngine;

namespace Axiom.UI.MainMenu
{
    public class AxiomAnimator : MonoBehaviour
    {
        public GameObject aLetter;
        public GameObject xLetter;
        public GameObject iLetter;
        public GameObject oLetter;
        public GameObject mLetter;

        private void Awake()
        {
            TweenParams tParms = new TweenParams().SetLoops(-1, LoopType.Incremental).SetEase(Ease.InFlash).SetEase(Ease.OutQuad);
            aLetter.transform.DOLocalRotate(new Vector3(0f, -180f, 0f), 5f, RotateMode.FastBeyond360).SetAs(tParms);
            xLetter.transform.DOLocalRotate(new Vector3(360f, 0f, 0f), 5f, RotateMode.LocalAxisAdd).SetAs(tParms);
            iLetter.transform.DOLocalRotate(new Vector3(360f, 0f, 0f), 5f, RotateMode.LocalAxisAdd).SetAs(tParms);
            oLetter.transform.DOLocalRotate(new Vector3(0f, 0f, 360f), 5f, RotateMode.LocalAxisAdd).SetAs(tParms);
            mLetter.transform.DOLocalRotate(new Vector3(0f, 360f, 0f), 5f, RotateMode.LocalAxisAdd).SetAs(tParms);
        }

        public void StopAnim()
        {
            aLetter.transform.DOKill();
            xLetter.transform.DOKill();
            iLetter.transform.DOKill();
            oLetter.transform.DOKill();
            mLetter.transform.DOKill();
        
            TweenParams tParms = new TweenParams().SetEase(Ease.InExpo).SetEase(Ease.OutBack);
            aLetter.transform.DOLocalRotate(new Vector3(0f, 0f, 0f), 2f).SetAs(tParms);
            xLetter.transform.DOLocalRotate(new Vector3(0f, 0f, 0f), 2f).SetAs(tParms);
            iLetter.transform.DOLocalRotate(new Vector3(0f, 0f, 0f), 2f).SetAs(tParms);
            oLetter.transform.DOLocalRotate(new Vector3(0f, 0f, 0f), 2f).SetAs(tParms);
            mLetter.transform.DOLocalRotate(new Vector3(0f, 0f, 0f), 2f).SetAs(tParms);
        }

        public void ZoomLogo()
        {
            StartCoroutine(ZoomLogo_CO());
        }

        private IEnumerator ZoomLogo_CO()
        {
            aLetter.transform.DOKill();
            xLetter.transform.DOKill();
            iLetter.transform.DOKill();
            oLetter.transform.DOKill();
            mLetter.transform.DOKill();

            TweenParams tParms = new TweenParams().SetEase(Ease.InExpo);
            aLetter.transform.DOMoveZ(-20f, 1f).SetAs(tParms);
            yield return new WaitForSeconds(0.1f);
            xLetter.transform.DOMoveZ(-20f, 1f).SetAs(tParms);
            yield return new WaitForSeconds(0.1f);
            iLetter.transform.DOMoveZ(-20f, 1f).SetAs(tParms);
            yield return new WaitForSeconds(0.1f);
            oLetter.transform.DOMoveZ(-20f, 1f).SetAs(tParms);
            yield return new WaitForSeconds(0.1f);
            mLetter.transform.DOMoveZ(-20f, 1f).SetAs(tParms);
        }
    }
}
