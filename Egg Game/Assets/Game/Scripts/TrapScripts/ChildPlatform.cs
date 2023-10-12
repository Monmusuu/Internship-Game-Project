using UnityEngine;
using Mirror;

public class ChildPlatform : NetworkBehaviour
{
    [SyncVar(hook = nameof(OnParentChanged))]
    private NetworkIdentity _parentIdentity;

    public NetworkIdentity ParentIdentity
    {
        get { return _parentIdentity; }
        set { _parentIdentity = value; }
    }

    private void OnParentChanged(NetworkIdentity oldParent, NetworkIdentity newParent)
    {
        // Set the new parent of this child object
        if (newParent != null)
            transform.SetParent(newParent.transform);
        else
            transform.SetParent(null); // Unparent if newParent is null
    }
}