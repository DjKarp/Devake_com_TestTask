using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
/// <summary>
/// Скрипт слежения за показателями GU интерфейса.
/// </summary>
public class GUI_Manager : MonoBehaviour
{
    
    private Image sliderHP;
    private TextMeshProUGUI textHP;
    
    private GameObject tempGO;

    
    private void Awake()
    {

        tempGO = GameObject.Find("SliderHP");
        sliderHP = tempGO.GetComponent<Image>();
        tempGO = GameObject.Find("HP Text (TMP)");
        textHP = tempGO.GetComponent<TextMeshProUGUI>();

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

    }

    private void Start()
    {
        

    }

    public void SetHPBarValue()
    {

        sliderHP.fillAmount = GameManager.Instance.GetPlayerPawn().GetCurrentHP() / GameManager.Instance.GetPlayerPawn().GetMaxtHP();
        textHP.SetText(GameManager.Instance.GetPlayerPawn().GetCurrentHP() + " / " + GameManager.Instance.GetPlayerPawn().GetMaxtHP());

    }
    
}
