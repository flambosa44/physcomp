using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FishSwim : MonoBehaviour
{
    // Start is called before the first frame update
    public float angularSpeed = 0.3f; // 1 rads/second
    public float speed = 0.001f;
    private float radius = 0.3f;
    private float angle = 0;
    private Transform trans, targetTrans;
    public GameObject target;
    private int state;
    private float restTimer = 0;
    Vector3 originalScale;
    private Animator fishAnim;
    private float originalAnimSpeed, time;
    void Start()
    {
        trans = this.transform;
        targetTrans = target.transform;
        state = 1;
        originalScale = trans.localScale;
        fishAnim = trans.gameObject.GetComponent<Animator>();
        originalAnimSpeed = fishAnim.speed;
        time = 0;
    }

    // Update is called once per frame
    void Update()
    {
        switch(state)
        {
            case 1:
                angle += angularSpeed * Time.deltaTime;
                trans.localRotation = Quaternion.Euler(angle * 180 / Mathf.PI, 90, 90);
                trans.localPosition = new Vector3(Mathf.Sin(angle), Mathf.Cos(angle), 0) * radius;
                if (angle >= 2 * Mathf.PI)
                    state++;
                break;
            case 2:
                trans.LookAt(targetTrans, Vector3.up);
                trans.position = Vector3.MoveTowards(trans.position, targetTrans.position, speed * Time.deltaTime);
                if (trans.position == targetTrans.position)
                    state++;
                break;
            case 3:
                Quaternion tempQ = Quaternion.LookRotation(Vector3.RotateTowards(trans.forward, trans.parent.position - trans.position, speed * Time.deltaTime, 0f));
                if (trans.rotation != tempQ)
                    trans.rotation = tempQ;
                else
                    state++;
                break;
            case 4:
                restTimer += Time.deltaTime;
                if (restTimer <= 3.0f)
                {
                    float newScale = Lerp(0, 3, 1, 1.5f, restTimer);
                    trans.localScale = originalScale * newScale;
                    fishAnim.speed = originalAnimSpeed * (1.7f - newScale);
                }
                else
                    state++;
                break;
            default:
                time += Time.deltaTime;
                if(time >= 3.0f)
                    this.gameObject.SetActive(false);
                break;

        }

    }

    public static float Lerp(float startX, float endX, float startY, float endY, float x)
    {
        float m = (endY - startY) / (endX - startX);
        return startY + m * (x - startX);
    } 
}
