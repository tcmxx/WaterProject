using UnityEngine;
using System.Collections;

namespace Water2DTool
{
    public class Destroyer : MonoBehaviour
    {
        public Water2D_Simulation water2D;

        // Update is called once per frame
        void Update()
        {
            if(water2D != null)
            transform.position = new Vector3(transform.position.x, water2D.waterLineCurrentGlobalPos.y, transform.position.z);
        }

        void OnTriggerEnter2D(Collider2D other)
        {
            if (other.tag == "Player")
            {
                PlatformerCharacter2D player = other.GetComponent<PlatformerCharacter2D>();

                player.ResetPlayerPosition();
            }
        }

        void OnTriggerEnter(Collider other)
        {
            if (other.tag == "Player")
            {
                PlatformerCharacter2D player = other.GetComponent<PlatformerCharacter2D>();

                player.ResetPlayerPosition();
            }
        }
    }
}
