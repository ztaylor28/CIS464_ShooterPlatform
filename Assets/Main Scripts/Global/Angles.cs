using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameDevDave
{
   public class Angles : MonoBehaviour
    {
        public static float AngleTowards (float x, float y)
        {
            // Value
            float v = ((float)(Mathf.Atan2(x, y) / Math.PI) * 180f);
            if(v < 0) v += 360f;

            return v;
        }

        public static int XYTo4Angle (float x, float y)
        {
            // Value
            float v = ((float)(Mathf.Atan2(x, y) / Math.PI) * 180f);
            if(v < 0) v += 360f;
            
            // 4-angle setup (360 / 4)
            if (v > 45 && v < 135) return 0; // Right
            if (v > 135 && v < 225) return 1; // Bottom
            if (v > 225 && v < 315) return 2; // Left
            if (v > 315 || v < 45) return 3; // Top

            // Default
            return 0;
        }

        public static int XYTo4Angle (float x, float y, Quaternion rotation)
        {
            // Value v
            float v = ((float)(Mathf.Atan2(x, y) / Math.PI) * 180f);   

            // Convert the Quaternion to Euler
            Vector3 upE = rotation.eulerAngles;

            // Value up
            if(upE.z < 0) upE.z += 360f;
            v += upE.z;

            // Keep it 360
            if(v < 0) v += 360f;
            
            // 4-angle setup (360 / 4)
            if (v > 45 && v < 135) return 0; // Right
            if (v > 135 && v < 225) return 1; // Bottom
            if (v > 225 && v < 315) return 2; // Left
            if (v > 315 || v < 45) return 3; // Top

            // Default
            return 0;
        }
        public static int XYTo6AngleRotated (float x, float y)
        {
            // Value
            float v = ((float)(Mathf.Atan2(x, y) / Math.PI) * 180f);
            if(v < 0) v += 360f;
            
            // 6-angle setup (360 / 6)
            if (v > 45 && v < 135) return 0; // Right
            if (v > 135 && v < 180) return 1; // Bottom Right 
            if (v > 180 && v < 225) return 2; // Bottom Left 
            if (v > 225 && v < 315) return 3; // Left
            if (v > 315) return 4; // Top Left 
            if (v > 0 && v < 45) return 5; // Top Right 

            // Default
            return 0;
        }

        public static int XYTo6AngleRotated (float x, float y, Quaternion rotation)
        {
            // Value v
            float v = ((float)(Mathf.Atan2(x, y) / Math.PI) * 180f);   

            // Convert the Quaternion to Euler
            Vector3 upE = rotation.eulerAngles;

            // Value up
            if(upE.z < 0) upE.z += 360f;
            v += upE.z;

            // Keep it 360
            if(v < 0) v += 360f;
            
            // 6-angle setup (360 / 6)
            if (v > 45 && v < 135) return 0; // Right
            if (v > 135 && v < 180) return 1; // Bottom Right 
            if (v > 180 && v < 225) return 2; // Bottom Left 
            if (v > 225 && v < 315) return 3; // Left
            if (v > 315) return 4; // Top Left 
            if (v > 0 && v < 45) return 5; // Top Right 

            // Default
            return 0;
        }

        public static int XYTo8Angle (float x, float y)
        {
            // Value v
            float v = ((float)(Mathf.Atan2(x, y) / Math.PI) * 180f);   
            if(v < 0) v += 360f;
            
            // 8-angle setup (360 / 8)
            if (v > 67.5 && v < 112.5) return 0; // Right
            if (v > 112.5 && v < 157.5) return 1; // Bottom Right
            if (v > 157.5 && v < 202.5) return 2; // Bottom
            if (v > 202.5 && v < 247.5) return 3; // Bottom Left
            if (v > 247.5 && v < 292.5) return 4; // Left
            if (v > 292.5 && v < 337.5) return 5; // Top Left
            if (v > 337.5 || v < 22.5) return 6; // Top
            if (v > 22.5 && v < 67.5) return 7; // Top Right

            // Default
            return 0;
        }
        public static int XYTo8Angle (float x, float y, Quaternion rotation)
        {
            // Value v
            float v = ((float)(Mathf.Atan2(x, y) / Math.PI) * 180f);   

            // Convert the Quaternion to Euler
            Vector3 upE = rotation.eulerAngles;

            // Value up
            if(upE.z < 0) upE.z += 360f;
            v += upE.z;

            // Keep it 360
            if(v < 0) v += 360f;
            
            // 8-angle setup (360 / 8)
            if (v > 67.5 && v < 112.5) return 0; // Right
            if (v > 112.5 && v < 157.5) return 1; // Bottom Right
            if (v > 157.5 && v < 202.5) return 2; // Bottom
            if (v > 202.5 && v < 247.5) return 3; // Bottom Left
            if (v > 247.5 && v < 292.5) return 4; // Left
            if (v > 292.5 && v < 337.5) return 5; // Top Left
            if (v > 337.5 || v < 22.5) return 6; // Top
            if (v > 22.5 && v < 67.5) return 7; // Top Right

            // Default
            return 0;
        }
        public static int XYTo8AngleTight (float x, float y)
        {
            // Value v
            float v = ((float)(Mathf.Atan2(x, y) / Math.PI) * 180f);   
            if(v < 0) v += 360f;
            
            // 8-angle setup (360 / 8)
            if (v > 45 && v < 135) return 0; // Right
            if (v > 135 && v < 168.75) return 1; // Bottom Right
            if (v > 168.75 && v < 191.25) return 2; // Bottom
            if (v > 191.25 && v < 225) return 3; // Bottom Left
            if (v > 225 && v < 315) return 4; // Left
            if (v > 315 && v < 348.75) return 5; // Top Left
            if (v > 348.75 || v < 11.25) return 6; // Top
            if (v > 11.25 && v < 45) return 7; // Top Right

            // Default
            return 0;
        }
        public static int XYTo8AngleTight (float x, float y, Quaternion rotation)
        {
            // Value v
            float v = ((float)(Mathf.Atan2(x, y) / Math.PI) * 180f);   

            // Convert the Quaternion to Euler
            Vector3 upE = rotation.eulerAngles;

            // Value up
            if(upE.z < 0) upE.z += 360f;
            v += upE.z;

            // Keep it 360
            if(v < 0) v += 360f;
            
            // 8-angle setup (360 / 8)
            if (v > 45 && v < 135) return 0; // Right
            if (v > 135 && v < 168.75) return 1; // Bottom Right
            if (v > 168.75 && v < 191.25) return 2; // Bottom
            if (v > 191.25 && v < 225) return 3; // Bottom Left
            if (v > 225 && v < 315) return 4; // Left
            if (v > 315 && v < 348.75) return 5; // Top Left
            if (v > 348.75 || v < 11.25) return 6; // Top
            if (v > 11.25 && v < 45) return 7; // Top Right

            // Default
            return 0;
        }
    }
}