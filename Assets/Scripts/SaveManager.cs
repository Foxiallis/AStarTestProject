using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class SaveManager : MonoBehaviour
{
    public static SaveManager instance; //Singleton behaviour

    private void Awake()
    {
        instance = this;
    }

    private const string SaveFileName = "save_data.json";

    public async void SaveCharacterDataAsync()
    {
        List<CharacterData> characters = PartyManager.instance.GetCharacterData();
        string path = Path.Combine(Application.persistentDataPath, SaveFileName);
        string jsonData = JsonUtility.ToJson(new SerializationWrapper<CharacterData>(characters), prettyPrint: true);

        using (StreamWriter streamWriter = File.CreateText(path))
        {
            await streamWriter.WriteAsync(jsonData);
            Debug.Log("Saved characted data!");
        }
    }

    public async void LoadCharacterDataAsync()
    {
        string path = Path.Combine(Application.persistentDataPath, SaveFileName);

        if (File.Exists(path))
        {
            using (StreamReader streamReader = File.OpenText(path))
            {
                string jsonData = await streamReader.ReadToEndAsync();
                var wrapper = JsonUtility.FromJson<SerializationWrapper<CharacterData>>(jsonData);
                PartyManager.instance.LoadPartyMembersFromSave(wrapper.items);
                Debug.Log("Loaded characted data!");
            }
        }
    }
}
