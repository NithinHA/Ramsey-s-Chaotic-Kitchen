using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelController : MonoBehaviour
{
    private void Start()
    {
        AudioManager.Instance.playSound("ambient_bg");
    }
}
