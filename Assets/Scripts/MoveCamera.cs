using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveCamera : MonoBehaviour
{
    private Transform camTrans;
    private float speed = 1.0f, angularSpeed = 2f;

    // Start is called before the first frame update
    void Start()
    {
        camTrans = this.gameObject.transform;
    }

    // Update is called once per frame
    void Update()
    {

            if (Input.GetKey(KeyCode.W))
                camTrans.Translate(camTrans.forward * speed * Time.deltaTime);
            if (Input.GetKey(KeyCode.S))
                camTrans.Translate(-camTrans.forward * speed * Time.deltaTime);
            if (Input.GetKey(KeyCode.D))
                camTrans.Translate(camTrans.right * speed * Time.deltaTime);
            if (Input.GetKey(KeyCode.A))
                camTrans.Translate(-camTrans.right * speed * Time.deltaTime);

        //if (Input.GetMouseButton(0))
        //{
        //    Ray ray = camTrans.gameObject.GetComponent<Camera>().ScreenPointToRay(Input.mousePosition);
        //    Vector3 mousePos = camTrans.gameObject.GetComponent<Camera>().ScreenToWorldPoint(Input.mousePosition);
        //    RaycastHit hit;
        //    //Debug.Log(Physics.Raycast(ray, out hit));
        //    //Vector3 dir = hit.point - camTrans.position;
            
        //    camTrans.rotation = Quaternion.LookRotation(Vector3.RotateTowards(camTrans.position,mousePos,angularSpeed * Time.deltaTime,0f));
            
        //}
    }
}
