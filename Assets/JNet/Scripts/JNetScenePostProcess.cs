using System.Collections;
using System.Collections.Generic;
using UnityEditor.Callbacks;
using UnityEngine;

public class JNetScenePostProcess : MonoBehaviour {

    ///<summary>
    /// Copied from UNET, 
    ///</summary>
    [PostProcessScene]
    public static void OnPostProcessScene()
    {
        uint nextSceneId = 1;
        foreach (JNetIdentity netID in FindObjectsOfType<JNetIdentity>())
        {
            if (netID.GetComponent<JNetInternal.JNetManager>() != null)
            {
                Debug.LogError("JNetManager has a JNetIdentity component. Not good.");
            }

            // Since no netID assigned, we unactivate objects until assigned
            // i.e. Awake will be called before this,
            netID.gameObject.SetActive(false);
            netID.SetSceneID(nextSceneId++);
        }
    }
}
