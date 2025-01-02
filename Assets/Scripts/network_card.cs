using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class network_card : MonoBehaviourPun, IPunObservable
{
    Vector3 realPosition = Vector3.zero;
    int dispID;
    float lerpSpeed = 0.25f;  // Increased Lerp speed for faster updates

    void Update()
    {
        if (!photonView.IsMine)
        {
            transform.position = Vector3.Lerp(transform.position, realPosition, lerpSpeed);
        }
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            // Send position only if it has changed significantly
            stream.SendNext(transform.position);
        }
        else
        {
            realPosition = (Vector3)stream.ReceiveNext();
        }
    }
}
