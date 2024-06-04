using System.Collections;
using System.Collections.Generic;
using Unity.Netcode.Components;
using UnityEngine;

public class NewBehaviourScript : NetworkAnimator
{
    protected override bool OnIsServerAuthoritative()
    {
        return false;
    }

}
