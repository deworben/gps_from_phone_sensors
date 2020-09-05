
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
// using CodeMonkey.Utils;

public class Window_Graph : MonoBehaviour {

    [SerializeField] private Sprite circleSprite;
    private RectTransform graphContainer;
    private GameObject gotest;

    private List<GameObject> golist;

    public List<int> valueList;

    private void Awake() {
        graphContainer = transform.Find("graph_container").GetComponent<RectTransform>();

        List<float> valueList = new List<float>() { 5, 98, 56, 45, 30, 22, 17, 15, 13};
        golist = new List<GameObject>();
        ShowGraph(valueList);
    }
    // public void plsStart() {
    //     graphContainer = transform.Find("graph_container").GetComponent<RectTransform>();

    //     List<int> valueList = new List<int>() { 5, 98, 56, 45, 30, 22, 17, 15, 13};
    //     ShowGraph(valueList);
    // }
    public void updateValues(List<float> newList){

        for (int i = 0; i < golist.Count; i++) {

            //Try do destroy the item if it's there
            try{
                Destroy(golist[i]);
                }
            catch (Exception ex){
                //Do nothing if there aren't any more gamebjects.
            }
        }
        ShowGraph(newList);
        return;
    }

    private GameObject CreateCircle(Vector2 anchoredPosition) {
        GameObject gameObject = new GameObject("circle", typeof(Image));
        gameObject.transform.SetParent(graphContainer, false);
        gameObject.GetComponent<Image>().sprite = circleSprite;
        RectTransform rectTransform = gameObject.GetComponent<RectTransform>();
        rectTransform.anchoredPosition = anchoredPosition;
        rectTransform.sizeDelta = new Vector2(5, 5);
        rectTransform.anchorMin = new Vector2(0, 0);
        rectTransform.anchorMax = new Vector2(0, 0);
        return gameObject;
    }

    private void ShowGraph(List<float> valueList) {
        float graphHeight = graphContainer.sizeDelta.y;
        float yMaximum = 0.5f;
        float xSize = 25f;

        GameObject lastCircleGameObject = null;
        for (int i = 0; i < valueList.Count; i++) {
            float xPosition = xSize + i * xSize;
            float yPosition = (valueList[i] / yMaximum) * graphHeight;
            GameObject circleGameObject = CreateCircle(new Vector2(xPosition, yPosition));
            
            // add object to gameobject list
            golist.Add(circleGameObject);

            //Create a connector line if this is not the last circle object
            if (lastCircleGameObject != null) {
                CreateDotConnection(lastCircleGameObject.GetComponent<RectTransform>().anchoredPosition, circleGameObject.GetComponent<RectTransform>().anchoredPosition);
            }
            lastCircleGameObject = circleGameObject;
            
        }
    }

    private void CreateDotConnection(Vector2 dotPositionA, Vector2 dotPositionB) {
        GameObject gameObject = new GameObject("dotConnection", typeof(Image));
        golist.Add(gameObject); //Add the object to the game object list
        gameObject.transform.SetParent(graphContainer, false);
        gameObject.GetComponent<Image>().color = new Color(1,1,1, .5f);
        RectTransform rectTransform = gameObject.GetComponent<RectTransform>();
        Vector2 dir = (dotPositionB - dotPositionA).normalized;
        float distance = Vector2.Distance(dotPositionA, dotPositionB);
        rectTransform.anchorMin = new Vector2(0, 0);
        rectTransform.anchorMax = new Vector2(0, 0);
        rectTransform.sizeDelta = new Vector2(distance, 3f);
        rectTransform.anchoredPosition = dotPositionA + dir * distance * .5f;
        // rectTransform.localEulerAngles = new Vector3(0, 0, UtilsClass.GetAngleFromVectorFloat(dir));
        float angle = (float)(Math.Atan2(dir[1], dir[0])*(180/3.1415)); // Radians to degrees
        // print(dir[0].ToString() +" ---- " + dir[1].ToString());
        rectTransform.localEulerAngles = new Vector3(0, 0, angle);
    }

}