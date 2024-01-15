using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PartyManager : MonoBehaviour
{
    public static PartyManager instance; //Singleton behaviour

    public List<GameObject> party;
    public List<IPartyMemberBehaviour> partyBehaviours; //as Unity doesn't display interface lists in the editor, for some reason
    public List<CharacterClass> classes;
    public List<CharacterSelectButton> characterButtons;
    public AStarPathfinder pathfinder;
    public PartyCharacter partyLeader;
    public List<AStarNode> path;

    public float speedModifier;
    public float turnRateModifier;
    public bool unlimitedStamina = true;
    public bool interactable = true;

    private bool partyReachedDestination;
    private int currentNodeIndex;

    private void Start()
    {
        instance = this;
        partyReachedDestination = true;
        partyBehaviours = new();
        InitializePartyMembers();
    }

    private void InitializePartyMembers()
    {
        for (int i = 0; i < party.Count; i++)
        {
            if (party[i].GetComponent<IPartyMemberBehaviour>() is IPartyMemberBehaviour partyMember)
            {
                partyBehaviours.Add(partyMember);
                partyMember.Initialize(classes[UnityEngine.Random.Range(0, classes.Count)]);
            }
            if (party[i].GetComponent<PartyCharacter>() is PartyCharacter partyCharacter)
            {
                characterButtons[i].Initialize(partyCharacter);
            }
        }

        characterButtons[0].OnSelect(true);

        Debug.Log($"Party count: {partyBehaviours.Count}");
    }

    private void Update()
    {
        if (partyReachedDestination) return;

        MoveParty();
    }

    private void MoveParty()
    {
        partyLeader.MoveTo(path[currentNodeIndex]);

        for (int i = 1; i < partyBehaviours.Count; i++)
        {
            try
            {
                partyBehaviours[i].FollowPartyMember(partyBehaviours[i-1]);
            }
            catch (ArgumentOutOfRangeException)
            {
                //Do nothing, it means the path just started and no previous positions are recorded
            }
        }

        if (partyLeader.CurrentPosition() == path[currentNodeIndex].worldPosition && currentNodeIndex != path.Count - 1)
        {
            currentNodeIndex++;
            pathfinder.RecalculateNodes();
        }

        partyReachedDestination = AllMembersReachedDestination();

        if (partyReachedDestination)
        {
            SetInteractability(true);
        }
    }

    private void SetInteractability(bool interactable)
    {
        this.interactable = interactable;
        characterButtons.ForEach(button => button.SetButton(interactable));
    }

    private bool AllMembersReachedDestination()
    {
        if (path.Count > 0 && partyLeader.transform.position != path[^1].worldPosition) return false;
        foreach(IPartyMemberBehaviour member in partyBehaviours)
        {
            if (!member.AtFinalDestination()) return false;
        }
        return true;
    }

    public void StartMoving()
    {
        foreach (IPartyMemberBehaviour partyMember in partyBehaviours)
        {
            partyMember.Reset();
        }

        SetInteractability(false);
    }

    public void SetPath(List<AStarNode> newPath)
    {
        path = newPath;
        partyReachedDestination = false;
        currentNodeIndex = 0;
    }
}
