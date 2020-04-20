using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IntroText : MonoBehaviour
{
    [SerializeField]
    GameContainer game;
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            game.gameObject.SetActive(true);
            GM gm = game.GetComponentInChildren<GM>(true);

            Destroy(gameObject);
        }
    }
}
