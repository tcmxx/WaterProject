using UnityEngine;
using System.Collections;

namespace Water2DTool
{
    public class CheckPoint : MonoBehaviour
    {
        void OnTriggerEnter2D(Collider2D other)
        {
            if (other.tag == "Player")
            {
                PlatformerCharacter2D player = other.GetComponent<PlatformerCharacter2D>();

                player.SetCheckPoint(transform.position);
            }
        }

        void OnTriggerEnter(Collider other)
        {
            if (other.tag == "Player")
            {
                PlatformerCharacter2D player = other.GetComponent<PlatformerCharacter2D>();

                player.SetCheckPoint(transform.position);
            }
        }
    }
}
