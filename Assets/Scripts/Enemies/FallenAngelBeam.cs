using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class FallenAngelBeam : MonoBehaviour
{
    [SerializeField] private VisualEffect beamVFX;

    public void PlayBeamVFX()
    {
        beamVFX.SendEvent("Fire");
    }
}
