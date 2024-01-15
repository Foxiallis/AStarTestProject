using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


/* Interface for main things - initialization and following behaviour. Can be used to create, for example, flying units,
 * non-character party members (that cannot be party leaders), etc. */
public interface IPartyMemberBehaviour
{
    void FollowPartyMember(IPartyMemberBehaviour member);
    void Reset();
    void Initialize(CharacterClass characterClass);
    bool AtFinalDestination();
    List<AStarNode> PreviousNodes { get; set; }
    Vector3 CurrentPosition();
}

public class PartyCharacter : MonoBehaviour, IPartyMemberBehaviour
{
    public CharacterClass characterClass;
    public float currentStamina;

    private List<AStarNode> previousNodes;
    private Vector3 destination;
    private AStarNode currentNode;
    private IPartyMemberBehaviour memberToFollow;

    public void Initialize(CharacterClass characterClass)
    {
        this.characterClass = characterClass;
        GetComponentInChildren<Renderer>().material = characterClass.pawnMaterial;
    }

    public void MoveTo(AStarNode node)
    {
        if (!CanWalk()) return;

        currentNode = node;
        destination = node.worldPosition;

        if (AtDestination()) return;

        float rotationAngle = RotationAngle();

        if (rotationAngle > 0.1f)
        {
            RotateTo();
        }

        if (rotationAngle < 50f) //to avoid "drifting" when turning backwards
        {
            WalkTo();
        }
    }

    private bool AtDestination()
    {
        return transform.position == destination;
    }

    public bool AtFinalDestination()
    {
        if (currentStamina == 0) return true; //though not at the destination, he can't move further so for the purpose of determining the end of a character's path it returns true
        if (memberToFollow == null || memberToFollow.PreviousNodes.Count < 2 || previousNodes.Count >= memberToFollow.PreviousNodes.Count - 1) return AtDestination();
        return transform.position == memberToFollow.PreviousNodes[^2].worldPosition;
    }

    private float RotationAngle()
    {
        Vector3 currentPosition = transform.position;
        currentPosition.y = 0; 

        Vector3 targetPosition = destination;
        targetPosition.y = 0;

        Vector3 directionToTarget = (targetPosition - currentPosition).normalized;
        Vector3 forward = transform.forward;
        forward.y = 0;

        float angle = Vector3.Angle(forward, directionToTarget);

        return angle;
    }

    private void RotateTo()
    {
        Vector3 direction = destination - transform.position;
        direction.y = 0;

        float stepSize = characterClass.turnRate * PartyManager.instance.turnRateModifier * Time.deltaTime;

        Quaternion targetRotation = Quaternion.LookRotation(direction);

        transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, stepSize);
    }

    private void WalkTo()
    {
        float stepSize = characterClass.speed * PartyManager.instance.speedModifier * Time.deltaTime;

        Vector3 newPosition = Vector3.MoveTowards(transform.position, destination, stepSize);

        transform.position = newPosition;

        if (AtDestination() && !previousNodes.Contains(currentNode))
        {
            previousNodes.Add(currentNode);
        }

        if (PartyManager.instance.unlimitedStamina) return;

        currentStamina = Mathf.Clamp(currentStamina - stepSize, 0, characterClass.stamina);
    }

    /* While not as full-proof and perfect as each having their own pathfinding behaviour, for our purposes it works well 
     * and reduces complexity from O(x * n) to O(x) where x is number of nodes that the leader goes through, and n is the number of characters in the party */
public void FollowPartyMember(IPartyMemberBehaviour member)
    {
        memberToFollow = member;

        List<AStarNode> unvisitedNodes = member.PreviousNodes;
        unvisitedNodes = unvisitedNodes.Except(previousNodes).ToList();

        if (unvisitedNodes.Count == 0) return;

        MoveTo(unvisitedNodes[0]);
    }

    public bool CanWalk()
    {
        if (currentStamina <= 0) return false;
        if (memberToFollow != null && previousNodes.Count >= memberToFollow.PreviousNodes.Count - 1) return false; //for members to stop before the one that they follow
        return true;
    }

    public Vector3 CurrentPosition()
    {
        return transform.position;
    }

    public void Reset()
    {
        previousNodes = new List<AStarNode>() { PartyManager.instance.pathfinder.grid.NodeFromWorldPoint(transform.position) };
        memberToFollow = null;
        currentNode = null;
        destination = Vector3.zero;
        currentStamina = characterClass.stamina;
    }

    public List<AStarNode> PreviousNodes
    {
        get => previousNodes;
        set => previousNodes = value;
    }
}
