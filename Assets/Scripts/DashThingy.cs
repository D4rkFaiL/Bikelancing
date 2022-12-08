using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DashThingy : MonoBehaviour
{
    [SerializeField] AnimationCurve _deathCurve;
    [SerializeField] float _deathVelocity;
    [SerializeField] Shader shader;
    IEnumerator Start(){

        Renderer renderer = GetComponent<Renderer>();
        renderer.material = new Material(shader);

        float time = 0;
        float animTime = 0;
        while (animTime != 1)
        {
            time += Time.deltaTime * _deathVelocity;
            animTime = _deathCurve.Evaluate(time);
            if(animTime >= 2.0){
                transform.parent = null;
            }
            renderer.material.SetFloat("_FadeLength",animTime);
            yield return null;
        }

        Destroy(gameObject);
    }
}
