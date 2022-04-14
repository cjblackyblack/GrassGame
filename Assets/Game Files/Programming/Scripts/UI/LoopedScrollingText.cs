using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class LoopedScrollingText : MonoBehaviour {

    public bool IsScrolling;

    [Header("Scroll Settings")]
    [SerializeField] private float scrollSpeed;
    [SerializeField] private bool scrollLeft;
    [SerializeField] private bool startOffScreen;
    [SerializeField] [Range(2, 10)] private int textCount;

    [Header("Text Settings")]
    [SerializeField] [TextArea(2,4)] private string textToLoop;
    [SerializeField] private float textSpacing;

    [Header("Object References")]
    [SerializeField] private GameObject firstTextObject;
    [SerializeField] private List<GameObject> textObjects;
    private RectTransform firstRectTransform;

    private RectTransform _rectTransform;

    // -----------------------------------------------------------------------------------------------------------

    private void Awake() {
        _rectTransform = GetComponent<RectTransform>();

        // Set up first object
        firstTextObject.GetComponent<TextMeshProUGUI>().text = textToLoop;
        firstRectTransform = firstTextObject.GetComponent<RectTransform>();
        firstRectTransform.anchorMin = new Vector2(scrollLeft ? 1f : 0f, 0.5f);
        firstRectTransform.anchorMax = new Vector2(scrollLeft ? 1f : 0f, 0.5f);
        firstRectTransform.anchoredPosition = scrollLeft ? new Vector2(startOffScreen ? firstRectTransform.rect.width + 5f : _rectTransform.rect.width * -1f, 0f)
            : new Vector2(startOffScreen ? firstRectTransform.rect.width * -1f - 5f : _rectTransform.rect.width, 0f);

        // Create new objects
        for(int i = textObjects.Count; i < textCount; i++) {
            textObjects.Add(Instantiate(firstTextObject, firstTextObject.transform.parent));
            textObjects[i].name = $"Text ({i})";
        }

        // Position new objects
        for(int i = 1; i < textObjects.Count; i++) {
            SetTextParent(textObjects[i], textObjects[i - 1]);
        }
    }

    private void LateUpdate() {
        if(!IsScrolling)
            return;

        firstRectTransform.anchoredPosition += new Vector2(scrollSpeed * Time.deltaTime * (scrollLeft ? -1f : 1f), 0f);
        if((scrollLeft && firstRectTransform.anchoredPosition.x < _rectTransform.rect.width * -1f) || 
            (!scrollLeft && firstRectTransform.anchoredPosition.x > _rectTransform.rect.width + firstRectTransform.rect.width)) {
            MoveFirstTextToEnd();
        }
    }

    // -----------------------------------------------------------------------------------------------------------

    private void SetTextParent(GameObject child, GameObject newParent) {
        SetTextParent(child.GetComponent<RectTransform>(), newParent.GetComponent<RectTransform>());
    }

    private void SetTextParent(RectTransform child, RectTransform newParent) {
        foreach(Transform t in child)
            t.parent = child.parent;
        child.parent = newParent;
        child.anchoredPosition = new Vector2(scrollLeft ? child.rect.width + textSpacing : -textSpacing, child.anchoredPosition.y);
    }

    private void MoveFirstTextToEnd() {
        GameObject first = textObjects[0];
        SetTextParent(first, textObjects[textObjects.Count - 1]);
        textObjects.RemoveAt(0);
        textObjects.Add(first);
        firstRectTransform = textObjects[0].GetComponent<RectTransform>();
    }

}
