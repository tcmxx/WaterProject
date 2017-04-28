using UnityEngine;
using System.Collections;

namespace Water2DTool
{
    // A simple script that will allow you to click on objects 
    // and move them in the Game Window(Panel).
    public class MoveObjects : MonoBehaviour
    {

        private bool clickedOn;
        private Rigidbody2D objRigidbody2D;
        private Rigidbody objRigidbody;
        private Vector2 offset;

        // Use this for initialization
        void Start()
        {
            objRigidbody2D = GetComponent<Rigidbody2D>();
            objRigidbody = GetComponent<Rigidbody>();
        }

        // Update is called once per frame
        void Update()
        {
            if (clickedOn)
                Dragging();
        }

        void OnMouseDown()
        {
            clickedOn = true;

            if (objRigidbody2D != null)
                objRigidbody2D.isKinematic = true;
            if (objRigidbody != null)
                objRigidbody.isKinematic = true;

            Vector3 mouseWorldPoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            offset = transform.position - mouseWorldPoint;
        }

        void OnMouseUp()
        {
            clickedOn = false;

            if (objRigidbody2D != null)
                objRigidbody2D.isKinematic = false;
            if (objRigidbody != null)
                objRigidbody.isKinematic = false;
        }

        void Dragging()
        {
            Vector3 mouseWorldPoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            transform.position = new Vector3(mouseWorldPoint.x + offset.x, mouseWorldPoint.y + offset.y, transform.position.z);
        }
    }
}
