using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChooseObject : MonoBehaviour
{
    private ProgrammManager ProgrammManagerScript;

    private Button button;

    public GameObject ChoosedObject;

    void Start()
    {
        /*ссылка на основной скрипт*/
        ProgrammManagerScript = FindObjectOfType<ProgrammManager>();

        /*помещаем кнопку*/
        button = GetComponent<Button>();

        /*при нажатии запускаем функцию ChooseObjectFunction*/
        button.onClick.AddListener(ChooseObjectFunction);

    }

    void ChooseObjectFunction()
    {
        /*компоненту ObjectToSpawn присваемваем объект, "висящий" на кнопке*/
        ProgrammManagerScript.ObjectToSpawn = ChoosedObject;

        /*объект выбран*/
        ProgrammManagerScript.ChooseObject = true;

        /*отключаем выпадающий список*/
        ProgrammManagerScript.ScrollView.SetActive(false);
    }
}