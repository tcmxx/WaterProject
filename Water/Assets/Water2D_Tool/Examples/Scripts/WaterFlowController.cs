using UnityEngine;
using System.Collections;

namespace Water2DTool
{
    public class WaterFlowController : MonoBehaviour
    {
        private bool waterFlow = true;
        private float hSliderValue = 5f;
        private bool useAngles = false;
        private float angle = 0;

        public Water2D_Simulation water2D_Sim;

        private Rect windowRect = new Rect(50, 60, 300, 350);

        void OnGUI()
        {
            windowRect = GUI.Window(0, windowRect, MyWindow, "Water Flow Options");
        }

        private void MyWindow(int windowID)
        {
            GUI.color = Color.white;
            waterFlow = GUI.Toggle(new Rect(50, 25, 100f, 30f), waterFlow, "Water Flow");
            water2D_Sim.waterFlow = waterFlow;

            if (waterFlow)
            {
                GUI.color = Color.white;

                useAngles = GUI.Toggle(new Rect(50, 50, 100f, 30f), useAngles, "Use Angles");
                water2D_Sim.useAngles = useAngles;

                if (useAngles)
                {
                    hSliderValue = GUI.HorizontalSlider(new Rect(50, 100, 100, 30), hSliderValue, 0.0F, 30.0f);
                    angle = GUI.HorizontalSlider(new Rect(50, 150, 100, 30), angle, 0.0F, 360.0f);
                    
                    hSliderValue = Mathf.Floor(hSliderValue);
                    angle = Mathf.Floor(angle);
                    
                    water2D_Sim.waterFlowForce = hSliderValue;
                    water2D_Sim.flowAngle = angle;

                    GUI.color = Color.black;
                    GUI.Label(new Rect(180, 100, 200, 30), "Flow Force " + hSliderValue);
                    GUI.Label(new Rect(180, 150, 200, 30), "Flow Angle " + angle);
                }
                else
                {
                    if (GUI.Button(new Rect(50, 100, 100f, 30f), "Left"))
                    {
                        water2D_Sim.flowDirection = Water2D_FlowDirection.Left;
                    }

                    if (GUI.Button(new Rect(50, 150, 100f, 30f), "Right"))
                    {
                        water2D_Sim.flowDirection = Water2D_FlowDirection.Right;
                    }

                    if (GUI.Button(new Rect(50, 200, 100f, 30f), "Up"))
                    {
                        water2D_Sim.flowDirection = Water2D_FlowDirection.Up;
                    }

                    if (GUI.Button(new Rect(50, 250, 100f, 30f), "Down"))
                    {
                        water2D_Sim.flowDirection = Water2D_FlowDirection.Down;
                    }


                    hSliderValue = GUI.HorizontalSlider(new Rect(50, 300, 100, 30), hSliderValue, 0.0F, 30.0f);

                    hSliderValue = Mathf.Floor(hSliderValue);
                    water2D_Sim.waterFlowForce = hSliderValue;

                    GUI.color = Color.black;

                    GUI.Label(new Rect(180, 300, 200, 30), "Flow Force " + hSliderValue);
                }
            }

            GUI.DragWindow();
        }
    }
}
