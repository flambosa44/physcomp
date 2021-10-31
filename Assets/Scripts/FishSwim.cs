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
    public GameObject target, fishDest;
    private GameObject effect;
    private Water waterScript;
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
        effect = target.transform.Find("Floating_Particles").gameObject;
        waterScript = this.gameObject.transform.parent.GetComponentInChildren<Water>();
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
                if (!effect.activeSelf)
                    effect.SetActive(true);
                restTimer += Time.deltaTime;
                if (restTimer <= 3.0f)
                {
                    Water.SetEffectAlpha(effect.GetComponent<ParticleSystem>(), Lerp(0, 3, 0.3f, 1, restTimer));
                    float ratio = Lerp(0, 3.0f, 0.5f, 0, restTimer);
                    fishAnim.speed = EnterDetect.calmAnimSpeed * ratio < 0.1f ? 0.1f : EnterDetect.calmAnimSpeed * ratio;
                    //float newScale = Lerp(0, 3, 1, 1.5f, restTimer);
                    //trans.localScale = originalScale * newScale;
                    //fishAnim.speed = originalAnimSpeed * (1.7f - newScale);
                }
                else
                    state++;
                break;
            case 5:
                time += Time.deltaTime;
                if(time >= 3.0f)
                    state++;
                break;
            case 6:
                if(fishAnim.speed != 1)
                    fishAnim.speed = 1;
                tempQ = Quaternion.LookRotation(Vector3.RotateTowards(trans.forward, fishDest.transform.position - trans.position, speed * 3 * Time.deltaTime, 0f));
                trans.rotation = tempQ;
                trans.position = Vector3.MoveTowards(trans.position, fishDest.transform.position, speed * Time.deltaTime);
                Water.SetEffectAlpha(effect.GetComponent<ParticleSystem>(), 
                    Lerp(0, Vector3.Distance(effect.transform.position, fishDest.transform.position), 0, 1, Vector3.Distance(trans.position, fishDest.transform.position)));
                if (trans.position == fishDest.transform.position)
                    state++;
                break;
            default:
                trans.gameObject.SetActive(false);
                effect.SetActive(false);
                waterScript.ActivateAnemone();
                break;

        }

    }

    public static float Lerp(float startX, float endX, float startY, float endY, float x)
    {
        if (x < startX)
            x = startX;
        else if (x > endX)
            x = endX;
        float m = (endY - startY) / (endX - startX);
        return startY + m * (x - startX);
    } 
}
