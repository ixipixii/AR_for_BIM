using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation; /*набор скриптов для работы с дополненной реальностью*/
using UnityEngine.XR.ARSubsystems; /*набор скриптов для работы с дополненной реальностью*/

public class ProgrammManager : MonoBehaviour
{
    [Header("Put your PlaneMarker here")] /*Пояснение для UNITY*/
    [SerializeField] private GameObject PlaneMarkerPrefab; /*маркер*/

    private ARRaycastManager ARRaycastManagerScript; /*ссылка на скрипт*/

    List<ARRaycastHit> hits = new List<ARRaycastHit>();

    public GameObject ObjectToSpawn; /*объект который ставим на маркер*/

    private GameObject SelectedObject; /*выбранный объект*/

    [Header("Put ScrollView here")]
    public GameObject ScrollView; /*выпадающий список*/

    public GameObject MaketShell;
    private Vector2 TouchPosition;

    [SerializeField] private Camera ARCamera; /*камера*/

    private Quaternion YRotation; /*вращение по Y*/
 
    public bool ChooseObject = false; /*флаг выбора объекта*/

    public bool Rotation; /*объект поворота*/

    public bool Recharging; 
     
    void Start() /*запуск программы*/
    {
        ARRaycastManagerScript = FindObjectOfType<ARRaycastManager>(); /*находим скрипт*/

        PlaneMarkerPrefab.SetActive(false);

        /*отключаем выпадающий список*/
        ScrollView.SetActive(false);
    }

    void Update() 
    {
        if (ChooseObject)
        {
            ShowMarkerAndSetObject();
        }

        MoveAndRotateObject();
    }

    void ShowMarkerAndSetObject()
    {
        List<ARRaycastHit> hits = new List<ARRaycastHit>(); /*список пересечений луча с плоскостями*/

        //стреляем лучём в середину экрана, фиксируем точки, которые пересекли плоскость, помещаем информацию в hits
        ARRaycastManagerScript.Raycast(new Vector2(Screen.width / 2, Screen.height / 2), hits, TrackableType.Planes);

        // показываем маркер
        if (hits.Count > 0)
        {
            /*берём позицию маркера и присваеваем ей значение места пересечения луча с плокостью*/
            PlaneMarkerPrefab.transform.position = hits[0].pose.position;

            /*отключаем маркер*/
            PlaneMarkerPrefab.SetActive(true);
        }
        // установка объекта, фаза на начале
        if (Input.touchCount > 0 && Input.touches[0].phase == TouchPhase.Began)
        {
            /*устанваливаем объект на точку пересечения, стороной как установили в UNITY*/
            Instantiate(ObjectToSpawn, hits[0].pose.position, ObjectToSpawn.transform.rotation);

            /*объект поставлен, можно выбирать следующий*/
            ChooseObject = false;
            PlaneMarkerPrefab.SetActive(false);
        }
    }

    void MoveAndRotateObject()
    {
        /*условие, есть ли касание*/
        if (Input.touchCount > 0) 
        {
            Touch touch = Input.GetTouch(0); /*касание*/
            TouchPosition = touch.position; /*позиция касания*/

            /*фаза на начале*/
            if (touch.phase == TouchPhase.Began)
            {
                /*луч, фиксирующий объекты в touch.position*/
                Ray ray = ARCamera.ScreenPointToRay(touch.position);

                /*объекты, которые пересёк луч*/
                RaycastHit hitObject;

                // Select choosed object
                /*условие на пересечение объектов*/
                if (Physics.Raycast(ray, out hitObject))
                {
                    /*если объект не выделен - выделям*/
                    if (hitObject.collider.CompareTag("UnSelected"))
                    {
                        hitObject.collider.gameObject.tag = "Selected";
                    }
                }
            }

            /*помещаем в ячейку выбранный объект */
            SelectedObject = GameObject.FindWithTag("Selected");

            /*фаза движения одного пальца*/
            if (touch.phase == TouchPhase.Moved && Input.touchCount == 1)
            {
                if (Rotation)
                {
                    // вращение 1им пальцем
                    /*координаты поворота по X Y Z*/
                    YRotation = Quaternion.Euler(0f, -touch.deltaPosition.x * 0.1f, 0f);
                    
                    /*присваиваем значение поворота выбранному объекту*/
                    SelectedObject.transform.rotation = YRotation * SelectedObject.transform.rotation;
                }
                else
                {
                    // фиксируем точки, которые пересекли плоскость
                    ARRaycastManagerScript.Raycast(TouchPosition, hits, TrackableType.Planes);
                    
                    /*двигаем объект по оси Y*/
                    SelectedObject.transform.position = hits[0].pose.position;
                }
            }
            //вращение 2мя пальцами
            if (Input.touchCount == 2)
            {
                Touch touch1 = Input.touches[0];
                Touch touch2 = Input.touches[1];

                /*условие движения одного из пальцев*/
                if (touch1.phase == TouchPhase.Moved || touch2.phase == TouchPhase.Moved)
                {
                    /*координаты касаний*/
                    float DistanceBetweenTouches = Vector2.Distance(touch1.position, touch2.position);

                    /*расстояние между касаниями*/
                    float prevDistanceBetweenTouches = Vector2.Distance(touch1.position - touch1.deltaPosition, touch2.position - touch2.deltaPosition);

                    /*расстояние между касаниями*/
                    float Delta = DistanceBetweenTouches - prevDistanceBetweenTouches;

                    /*если дельта > 0*/
                    if (Mathf.Abs(Delta) > 0)
                    {
                        Delta *= 0.1f;
                    }
                    else
                    {
                        DistanceBetweenTouches = Delta = 0;
                    }

                    YRotation = Quaternion.Euler(0f, -touch1.deltaPosition.x * Delta, 0f);
                    SelectedObject.transform.rotation = YRotation * SelectedObject.transform.rotation;
                }

            }
            // Отменяем выбор объекта (перестаём двигать)
            if (touch.phase == TouchPhase.Ended)
            {
                if (SelectedObject.CompareTag("Selected"))
                {
                    SelectedObject.tag = "UnSelected";
                }
            }
        }
    }
}