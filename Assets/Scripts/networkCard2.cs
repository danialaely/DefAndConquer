using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class networkCard2 : MonoBehaviourPun, IPunObservable
{
    private Vector3 realPosition = Vector3.zero;
    private float lerpSpeed = 0.25f;  // Increased Lerp speed for faster updates

    void Update()
    {
        // Only apply Lerp if this is not the owner
        if (!photonView.IsMine)
        {
            transform.position = Vector3.Lerp(transform.position, realPosition, lerpSpeed);
        }
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            // Send position data
            stream.SendNext(transform.position);
        }
        else
        {
            // Receive and apply position data
            realPosition = (Vector3)stream.ReceiveNext();
        }
    }
}
