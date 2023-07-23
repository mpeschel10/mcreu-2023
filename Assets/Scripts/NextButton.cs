using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NextButton : MonoBehaviour, MouseSelector.Clickable
{
    public interface HasOnNext
    {
        void OnNext();
    }
    [SerializeField] GameObject pillarControllerObject;
    HasOnNext pillarController;
    void Awake()
    {
        pillarController = pillarControllerObject.GetComponent<HasOnNext>();
    }

    public void Click()
    {
        // Debug.Log("Next click");
        pillarController.OnNext();
    }
}
