using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CharacterSelectButton : MonoBehaviour
{
    public TMP_Text characterName;
    public Image buttonImage;
    public Color selectedColor;
    public Color deselectedColor;

    private PartyCharacter character;
    private Button button;

    private void Start()
    {
        button = GetComponent<Button>();
    }

    public void Initialize(PartyCharacter character)
    {
        this.character = character;
        characterName.text = character.gameObject.name;
    }

    public void SetButton(bool interacrable)
    {
        button.interactable = interacrable;
    }

    public void OnSelect(bool selected)
    {
        if (selected) SetPartyLeader();

        buttonImage.color = selected ? selectedColor : deselectedColor;
    }

    private void SetPartyLeader()
    {
        PartyManager.instance.characterButtons.ForEach(button => button.OnSelect(false));

        PartyManager.instance.partyLeader = character;
        if (PartyManager.instance.partyBehaviours.IndexOf(character) == 0) return;
        PartyManager.instance.partyBehaviours.Remove(character);
        PartyManager.instance.partyBehaviours.Insert(0, character);
    }
}
