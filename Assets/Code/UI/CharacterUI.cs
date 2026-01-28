using UnityEngine;
using UnityEngine.UI;

public class CharacterUI : MonoBehaviour
{
    [SerializeField] private Character character;
    [SerializeField] private Slider healthBar;

    private void Update()
    {
        if (character == null || healthBar == null)
        {
            return;
        }
        healthBar.value = (float)character.currentHP / (character.baseStatus.hp + character.aditionalStatus.hp);
    }
}