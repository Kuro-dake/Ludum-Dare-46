using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using YamlDotNet.RepresentationModel;
public class EncounterGenerator : MonoBehaviour
{
    
    void SetSpecificData(YamlMappingNode party_data_node, Party party)
    {
        party.Initialize();
        YamlMappingNode specs = party_data_node.GetNode<YamlMappingNode>("specs");
        foreach (KeyValuePair<YamlNode, YamlNode> kv in specs)
        {
            int position = 0;
            Character c = null;
            string character_name = kv.Key.ToString();

            try
            {
                position = ((YamlMappingNode)kv.Value).GetInt("position");
                c = party[position];
                c.name = character_name;
            }
            catch (KeyNotFoundException)
            {
                try
                {
                    c = party[character_name];
                }
                catch (System.NullReferenceException)
                {
                    throw new UnityException("Neither position nor a name identify a character");
                }
            }

            c.SetCharacterFromYAMLNode((YamlMappingNode)kv.Value);
            c.Heal(c.max_hp);
        }
    }

    public void GenerateEncounter()
    {
        YamlMappingNode encounter = Setup.GetFile("encounters/encounter");


        YamlMappingNode player_data = encounter.GetNode<YamlMappingNode>("player");

        foreach (Character c in GM.party.members.members)
        {
            c.SetCharacterFromYAMLNode(player_data);
        }

        SetSpecificData(player_data, GM.party);

        EnemyParty ep = Instantiate(GM.characters.enemy_party_prefab);


        YamlMappingNode enemy_data = encounter.GetNode<YamlMappingNode>("enemies");
        for(int i = 0; i < enemy_data.GetInt("number"); i++)
        {
            Character c = Instantiate(GM.characters.GetEnemyPrefab("impaler1"));
            c.gameObject.name = c.gameObject.name.Replace("(Clone)", "");
            c.position = i;
            c.transform.SetParent(ep.members.transform);
            c.transform.localPosition = Vector2.zero;

            
            c.SetCharacterFromYAMLNode(enemy_data);
            if (i == 0)
            {
                UnityEditor.Selection.activeTransform = c.transform.GetChild(0);
            }
        }

        SetSpecificData(enemy_data, ep);

        GM.game.StartCombat(ep);

    }

    
}


