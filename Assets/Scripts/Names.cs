using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Names")]
public class Names : ScriptableObject

{
    public GenericDictionary<Gender,List<string>> titles = new GenericDictionary<Gender, List<string>>();
    public GenericDictionary<Gender,List<string>> firstNames = new GenericDictionary<Gender, List<string>>();
    public List<string> surnames = new List<string>();

    public CharacterName GenName(Character character)
    {
        CharacterName cn = new CharacterName();
        cn.firstName = firstNames[character.gender][Random.Range(0,firstNames[character.gender].Count)];
        cn.surname = surnames[Random.Range(0,surnames.Count)];

        if(Random.Range(0,3) == 1)
        {
            if(Random.Range(0,2) == 1)
            {
                cn.title = titles[Gender.NA][Random.Range(0,titles[Gender.NA].Count)];
            }
            else{
                cn.title = titles[character.gender][Random.Range(0,titles[character.gender].Count)];
            }
        }
        return cn;

    }
}

[System.Serializable]
public class CharacterName
{
    public string title;
    public string firstName;
    public string surname;

    public string fullName()
    {return title + firstName + " "+surname;}

}