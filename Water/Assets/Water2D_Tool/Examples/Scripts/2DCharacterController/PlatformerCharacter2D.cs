using System;
using UnityEngine;

namespace Water2DTool
{
    // This is a simplified version of the 2D Character Controler that can be found in the
    // Unity Standard Assets
    public class PlatformerCharacter2D : MonoBehaviour
    {
        [SerializeField]
        private float m_MaxSpeed = 10f;                    // The fastest the player can travel in the x axis.
        [SerializeField]
        private float m_JumpForce = 400f;                  // Amount of force added when the player jumps.
        [SerializeField]
        private LayerMask m_WhatIsGround;                  // A mask determining what is ground to the character

        private Transform m_GroundCheck;    // A position marking where to check if the player is grounded.
        const float k_GroundedRadius = .2f; // Radius of the overlap circle to determine if grounded
        private bool m_Grounded;            // Whether or not the player is grounded.
        const float k_CeilingRadius = .01f; // Radius of the overlap circle to determine if the player can stand up
        private Rigidbody m_Rigidbody;
        private Rigidbody2D m_Rigidbody2D;
        private bool m_FacingRight = true;  // For determining which way the player is currently facing.

        private Vector3 checkPoint;
        public bool use2DColliders = true;

        private void Awake()
        {
            // Setting up references.
            m_GroundCheck = transform.Find("GroundCheck");

            if (GetComponent<Rigidbody>())
                m_Rigidbody = GetComponent<Rigidbody>();

            if (GetComponent<Rigidbody2D>())
                m_Rigidbody2D = GetComponent<Rigidbody2D>();

            checkPoint = transform.position;
        }


        private void FixedUpdate()
        {
            m_Grounded = false;

            // The player is grounded if a circlecast to the groundcheck position hits anything designated as ground
            // This can be done using layers instead but Sample Assets will not overwrite your project settings.
            Collider[] colliders3D = null;
            Collider2D[] colliders2D = null;

            if (use2DColliders)
                colliders2D = Physics2D.OverlapCircleAll(m_GroundCheck.position, k_GroundedRadius, m_WhatIsGround);
            else
                colliders3D = Physics.OverlapSphere(m_GroundCheck.position, k_GroundedRadius, m_WhatIsGround);

            if (use2DColliders)
            {
                for (int i = 0; i < colliders2D.Length; i++)
                {
                    if (colliders2D[i].gameObject != gameObject)
                        m_Grounded = true;
                }
            }
            else
            {
                for (int i = 0; i < colliders3D.Length; i++)
                {
                    if (colliders3D[i].gameObject != gameObject)
                        m_Grounded = true;
                }
            }
        }


        public void Move(float move, bool crouch, bool jump)
        {
            // Move the character
            if (move != 0)
            {
                if (use2DColliders)
                    m_Rigidbody2D.velocity = new Vector3(move * m_MaxSpeed, m_Rigidbody2D.velocity.y, 0);
                else
                    m_Rigidbody.velocity = new Vector2(move * m_MaxSpeed, m_Rigidbody.velocity.y);
            }

            // If the input is moving the player right and the player is facing left...
            if (move > 0 && !m_FacingRight)
            {
                // ... flip the player.
                Flip();
            }
            // Otherwise if the input is moving the player left and the player is facing right...
            else if (move < 0 && m_FacingRight)
            {
                // ... flip the player.
                Flip();
            }
            //}
            // If the player should jump...
            if (m_Grounded && jump)
            {
                // Add a vertical force to the player.
                m_Grounded = false;
                //m_Anim.SetBool("Ground", false);

                if(use2DColliders)
                    m_Rigidbody2D.AddForce(new Vector3(0f, m_JumpForce, 0));
                else
                    m_Rigidbody.AddForce(new Vector3(0f, m_JumpForce, 0));
            }
        }


        private void Flip()
        {
            // Switch the way the player is labelled as facing.
            m_FacingRight = !m_FacingRight;

            // Multiply the player's x local scale by -1.
            Vector3 theScale = transform.localScale;
            theScale.x *= -1;
            transform.localScale = theScale;
        }

        public void ResetPlayerPosition()
        {
            transform.position = new Vector3( checkPoint.x, checkPoint.y, transform.position.z);
        }

        public void SetCheckPoint(Vector3 pos)
        {
            checkPoint = pos;
        }
    }
}
