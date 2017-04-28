using UnityEngine;
using System.Collections;

namespace Water2DTool
{
    // This script will display 2 buttons on the Game Screen.
    // Must be attached to the Main Camera.
    // You must add the scenes to the Build and Run first, otherwise you will
    // get an error when pressing the buttons.
    public class SceneMenu : MonoBehaviour
    {
        void OnGUI()
        {
            if (GUI.Button(new Rect(Screen.width / 2f - 380, 25, 100f, 30f), "2D Water"))
            {
                Application.LoadLevel("2D_Water");
            }

            if (GUI.Button(new Rect(Screen.width / 2f - 270, 25, 100f, 30f), "2.5D Water"))
            {
                Application.LoadLevel("2.5D_Water");
            }

            if (GUI.Button(new Rect(Screen.width / 2f - 160, 25, 100f, 30f), "Water Flow"))
            {
                Application.LoadLevel("WaterFlow");
            }

            if (GUI.Button(new Rect(Screen.width / 2f - 50, 25, 100f, 30f), "Big Waves"))
            {
                Application.LoadLevel("BigWavesInteraction");
            }

            if (GUI.Button(new Rect(Screen.width / 2f + 60, 25, 100f, 30f), "Control Point"))
            {
                Application.LoadLevel("ControlPoints");
            }

            if (GUI.Button(new Rect(Screen.width / 2f + 170, 25, 100f, 30f), "Animation 1"))
            {
                Application.LoadLevel("SandBox_01");
            }

            if (GUI.Button(new Rect(Screen.width / 2f + 280, 25, 100f, 30f), "Animation 2"))
            {
                Application.LoadLevel("SandBox_02");
            }
        }
    }
}
