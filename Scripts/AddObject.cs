using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AddObject : MonoBehaviour
{
    private Button button;
    private ProgrammManager ProgrammManagerScript;

    // Start is called before the first frame update
    void Start()
    {
        /*ссылка на основной скрипт*/
        ProgrammManagerScript = FindObjectOfType<ProgrammManager>();

        button = GetComponent<Button>();

        /*при нажатии запускаем функцию AddObjectFunction*/
        button.onClick.AddListener(AddObjectFunction);
    }

    // Update is called once per frame
    void AddObjectFunction()
    {
        /*включаем выпадающий список*/
        ProgrammManagerScript.ScrollView.SetActive(true);
    }
}
