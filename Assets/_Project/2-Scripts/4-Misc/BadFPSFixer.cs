using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Formats.Alembic.Importer;

public class BadFPSFixer : MonoBehaviour
{
    public AlembicStreamPlayer alembic;
    public Animator anim;
    void Start()
    {
        Invoke(nameof(EnableStuff), 8f);
    }

    void EnableStuff()
    {
        alembic.enabled = true;
        anim.enabled = true;
    }
}
