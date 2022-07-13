using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoxAnimManager : MonoBehaviour
{
    public MainMenuAnim[] menuAnims;
    public List<GameObject> showObjects;

    private IEnumerator StartBoxes()
    {
        Camera.main.orthographic = false;

        foreach (GameObject obj in showObjects)
        {
            obj.SetActive(true);
            yield return new WaitForSeconds(0.5f);
        }

        foreach (MainMenuAnim anim in menuAnims)
        {
            anim.ObjectRotation(new Vector3(360f, 360f, 360f), 1f, ObjectAnimMode.Random);
            yield return new WaitForSeconds(0.1f);
        }

        float startTime = Time.time;
        while(Time.time < startTime + 2f)
        {
            foreach (MainMenuAnim anim in menuAnims) anim.transform.position -= new Vector3(0, 0, 5 * Time.deltaTime);
            yield return null;
        }
    }
}
