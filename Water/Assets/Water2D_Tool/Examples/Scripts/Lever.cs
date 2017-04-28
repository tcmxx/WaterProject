using UnityEngine;
using System.Collections;

namespace Water2DTool
{
    public class Lever : MonoBehaviour
    {

        [HideInInspector]
        public Animator animator;
        private bool playerDetected = false;
        private bool rightFlow = false;
        public Water2D_Simulation water2D;
        public Lever secondLever;

        void Start()
        {
            animator = GetComponent<Animator>();
        }

        // Update is called once per frame
        void Update()
        {

            if (playerDetected)
            {
                if (Input.GetKeyDown(KeyCode.LeftAlt) || Input.GetKeyDown(KeyCode.LeftAlt) || Input.GetKeyDown(KeyCode.LeftControl) || Input.GetKeyDown(KeyCode.LeftControl))
                {
                    if (rightFlow)
                    {
                        animator.SetBool("RightFlow", false);
                        rightFlow = false;
                        water2D.flowDirection = Water2D_FlowDirection.Left;

                        secondLever.animator.SetBool("RightFlow", false);
                        secondLever.rightFlow = false;
                    }
                    else
                    {
                        animator.SetBool("RightFlow", true);
                        rightFlow = true;
                        water2D.flowDirection = Water2D_FlowDirection.Right;

                        secondLever.animator.SetBool("RightFlow", true);
                        secondLever.rightFlow = true;
                    }
                }
            }
        }

        void OnTriggerEnter2D(Collider2D other)
        {
            if (other.tag == "Player")
            {
                playerDetected = true;
            }
        }

        void OnTriggerExit2D(Collider2D other)
        {
            if (other.tag == "Player")
            {
                playerDetected = false;
            }
        }

        void OnTriggerEnter(Collider other)
        {
            if (other.tag == "Player")
            {
                playerDetected = true;
            }
        }
        void OnTriggerExit(Collider other)
        {
            if (other.tag == "Player")
            {
                playerDetected = false;
            }
        }
    }
}
