using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class TeleporterReceiver : NetworkBehaviour
{
    [SyncVar]
    public int receiverNumber;

}
