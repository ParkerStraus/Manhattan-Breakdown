using System.Collections;
using System.Collections.Generic;
using Unity.Netcode.Components;
using UnityEngine;

namespace Unity.Multiplayer.Samples.Utilities.ClientAuthority
{

    [DisallowMultipleComponent]
    public class ClientNetworkTransform : NetworkTransform
    {
    // Start is called before the first frame update
        protected override bool OnIsServerAuthoritative()
        { 
        
            return false;
        }
    }

}
