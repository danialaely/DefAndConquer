using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StrongHoldNetwork : MonoBehaviourPun, IPunObservable
{
    Vector3 realPosition = Vector3.zero;
    int dispID;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (photonView.IsMine)
        {
            //Do Nothing

        }
        else
        {
            transform.position = Vector3.Lerp(transform.position, realPosition, 0.1f);
            //  transform.GetComponent<DisplayCard>().displayId = dispID;
            //  Debug.Log("Display id:"+ transform.GetComponent<DisplayCard>().displayId);
        }
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        //this is our player
        if (stream.IsWriting)
        {
            stream.SendNext(transform.position);
            //stream.SendNext(transform.GetComponent<DisplayCard>().displayId);
        }
        //if this is other player
        else
        {
            realPosition = (Vector3)stream.ReceiveNext();
            // photonView.RPC("UpdateDisplayId", RpcTarget.AllBuffered, (int)stream.ReceiveNext());
            // transform.GetComponent<DisplayCard>().UpdateCardInformation();
        }
    }
}
