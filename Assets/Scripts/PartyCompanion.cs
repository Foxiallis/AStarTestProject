using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PartyCompanion : MonoBehaviour//, IPartyMemberBehaviour
{
    public void FollowPartyMember(Vector3 memberLocation)
    {
        
    }

    public Vector3 CurrentPosition()
    {
        return transform.position;
    }

    public void ResetStamina()
    {
        throw new System.NotImplementedException();
    }
}
