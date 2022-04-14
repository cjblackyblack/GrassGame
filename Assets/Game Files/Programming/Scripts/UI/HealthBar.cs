using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour {

    [Header("Object References")]
    [SerializeField] private SmartObject smartObject;
    [SerializeField] private RectTransform mainBar, secondaryBar;
    [SerializeField] private RectTransform rectMask;

    [Header("Settings")]
    [SerializeField] private bool fillLeftToRight;
    [SerializeField] private float delayTime;
    [SerializeField] [Tooltip("Speed in percent/second")] private float lerpSpeed;

    private float currentPercent;

    // -----------------------------------------------------------------------------------------------------------

    private void Awake() {
        currentPercent = 1;

        if(fillLeftToRight) {
            mainBar.localScale = new Vector3(-1f, 1f, 1f);
            mainBar.anchorMin = new Vector2(0f, 0.5f);
            mainBar.anchorMax = new Vector2(0f, 0.5f);
            secondaryBar.localScale = new Vector3(-1f, 1f, 1f);
            secondaryBar.anchorMin = new Vector2(0f, 0.5f);
            secondaryBar.anchorMax = new Vector2(0f, 0.5f);
        } else {
            mainBar.localScale = Vector3.one;
            mainBar.anchorMin = new Vector2(1f, 0.5f);
            mainBar.anchorMax = new Vector2(1f, 0.5f);
            secondaryBar.localScale = Vector3.one;
            secondaryBar.anchorMin = new Vector2(1f, 0.5f);
            secondaryBar.anchorMax = new Vector2(1f, 0.5f);
        }
        PositionBar(mainBar, currentPercent);
        PositionBar(secondaryBar, currentPercent);
    }

    /*private void Update() {
        if(Input.GetKeyDown(KeyCode.Space)) {
            OnTakeDamage();
        }
    }*/

    public void SetSmartObject(SmartObject obj) {
        if(obj != smartObject) {
            if(smartObject)
                smartObject.OnTakeDamage -= OnTakeDamage;

            smartObject = obj;
            if(smartObject) {
                obj.OnTakeDamage += OnTakeDamage;
                currentPercent = smartObject.Stats.HP / (float)smartObject.Stats.MaxHP;
                PositionBar(mainBar, currentPercent);
                PositionBar(secondaryBar, currentPercent);
            }
        }
    }

    public void OnTakeDamage() {
        float oldPercent = currentPercent;
        currentPercent = smartObject.Stats.HP / (float)smartObject.Stats.MaxHP;
        PositionBar(mainBar, currentPercent);
        StopAllCoroutines();
        if(gameObject.activeInHierarchy)
            StartCoroutine(LerpSecondaryBar());

        IEnumerator LerpSecondaryBar() {
            yield return new WaitForSeconds(delayTime);
            while(oldPercent > currentPercent) {
                oldPercent -= lerpSpeed * Time.deltaTime / 100f;
                PositionBar(secondaryBar, oldPercent);
                yield return new WaitForEndOfFrame();
            }
            PositionBar(secondaryBar, currentPercent);
        }
    }

    private void PositionBar(RectTransform bar, float percent) {
        if(fillLeftToRight)
            bar.anchoredPosition = new Vector3(percent * rectMask.rect.width, 0f, 0f);
        else
            bar.anchoredPosition = new Vector3((1f - percent) * rectMask.rect.width, 0f, 0f);
    }

}
