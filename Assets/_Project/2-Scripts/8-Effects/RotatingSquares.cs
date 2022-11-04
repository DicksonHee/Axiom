using DG.Tweening;
using UnityEngine;

public class RotatingSquares : MonoBehaviour
{
    public bool isRotating;
    public SquareAnim squareAnim;

    public bool debugEnabled;
    private bool isAtEnd;
    private Vector3 stoppedRotation;
    private float normalizeAmount;

    private void Start()
    {
        transform.rotation = new Quaternion(Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f));
    }

    private void Update()
    {
        if (isRotating || isAtEnd) return;

        transform.localRotation = Quaternion.Euler(Vector3.Lerp(stoppedRotation, Vector3.zero, (squareAnim.GetLerpAmount() - normalizeAmount) / (1 - normalizeAmount)));
    }

    public void RandomRotate()
    {
        if (Random.Range(0, 2) == 0)
        {
            transform
                .DOLocalRotate(new Vector3(Random.Range(0, 360), Random.Range(0, 360), Random.Range(0, 360)), Random.Range(40f, 50f), RotateMode.FastBeyond360)
                .SetLoops(-1, LoopType.Incremental).SetEase(Ease.Linear);
        }
        else
        {
            transform
                .DOLocalRotate(new Vector3(Random.Range(0, 360), Random.Range(0, 360), Random.Range(0, 360)) * -1, Random.Range(40f, 50f), RotateMode.FastBeyond360)
                .SetLoops(-1, LoopType.Incremental).SetEase(Ease.Linear);
        }
    }

    public void KillRotation()
    {
        isAtEnd = true;
    }

    public void SetIsRotating(bool val, float normalizeVal)
    {
        bool wasRotating = isRotating;
        isRotating = val;

        if (wasRotating && !isRotating)
        {
            transform.DOKill();
            stoppedRotation = transform.localRotation.eulerAngles;
            normalizeAmount = normalizeVal;
        }
        else if(!wasRotating && isRotating)
        {
            RandomRotate();
            isAtEnd = false;
        }
    }
}