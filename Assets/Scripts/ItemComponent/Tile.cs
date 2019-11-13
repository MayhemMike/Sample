using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour {

    public Vector3 target;
    public int xPosition = 0;
    public int yPosition = 0;
    public bool moving = false;

    void Update() {
        if(moving) {
            float step = 4f * Time.deltaTime;
            transform.localPosition = Vector3.MoveTowards(this.transform.localPosition, target, step);

            if(Vector3.Distance(transform.localPosition, target) < 0.001f) {
                transform.localPosition = target;
                moving = false;
            }
        }
    } 

    public void Move() {
        target = new Vector3(transform.localPosition.x, yPosition, transform.localPosition.z);
        moving = true;
    }
}

    