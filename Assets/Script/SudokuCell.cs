using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SudokuCell : MonoBehaviour
{
    public int X;
    public int Y;

    public bool isRotation = false;

    public bool RotateState = false;

    private const float fspeed = 15.0f;
    GameObject cellText;

    // Start is called before the first frame update
    void Start()
    {
        //PlayRotation();
    }

    public void PlaySame()
    {
        isRotation = true;
        Invoke("StopSame",0.8f);
    }

    void StopSame()
    {
        isRotation = RotateState;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (cellText == null)
        {
            cellText = transform.GetComponentInChildren<Text>().gameObject;
        }
        if (isRotation)
        {
            cellText.transform.Rotate(Vector3.back, 30 * Time.fixedDeltaTime * fspeed);
        }
        else
        {
            cellText.transform.localEulerAngles = Vector3.zero;
        }
    }
}
