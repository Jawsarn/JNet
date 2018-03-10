using System.Collections;
using System.Collections.Generic;
using UnityEditor.Callbacks;
using UnityEngine;

namespace JNetInternal
{
    public class JNetScenePostProcess : MonoBehaviour
    {

        ///<summary>
        /// Copied from UNET, 
        ///</summary>
        [PostProcessScene]
        public static void OnPostProcessScene()
        {
            // Remove old objects
            JNetSceneHandler.ClearSceneObjects();

            // Add scene objects to scene handler
            uint nextSceneId = 1;
            foreach (JNetIdentity netID in FindObjectsOfType<JNetIdentity>())
            {
                if (netID.GetComponent<JNetInternal.JNetManager>() != null)
                {
                    Debug.LogError("JNetManager has a JNetIdentity component. Not good.");
                }

                // Can't add the IDs on awake, cause no ID assigned
                netID.SetNetID(nextSceneId++);
                JNetSceneHandler.AddNetObject(netID);
            }
        }
    }
}