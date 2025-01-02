using UnityEngine;
using Photon.Pun;
using System.Collections;

public class ActivateMultipleGameObjects : MonoBehaviourPun
{
    public GameObject[] targetObjects;

    void Start()
    {
        // Start the coroutine to activate the GameObjects after 2 seconds
        StartCoroutine(DeactivateObj());
        StartCoroutine(ActivateObjectsAfterDelay());
    }

    private IEnumerator ActivateObjectsAfterDelay()
    {
        yield return new WaitForSeconds(2f);
        photonView.RPC("ActivateObjects", RpcTarget.All);
    }

    private IEnumerator DeactivateObj() 
    {
        yield return new WaitForSeconds(0f);
        photonView.RPC("DecativateObjects", RpcTarget.All);
    }

    [PunRPC]
    void DecativateObjects() 
    {
        foreach (GameObject obj in targetObjects)
        {
            obj.SetActive(false);
        }
    }

    [PunRPC]
    void ActivateObjects()
    {
        foreach (GameObject obj in targetObjects)
        {
            obj.SetActive(true);
        }
    }
}
