using UnityEngine;
using System.Collections.Generic;



public class Cleanbox
{
	public static bool SameSign(int x, int y)
	{
		return ((x < 0) == (y < 0));  
	}

    public static Vector3 RandomOffset(float radius, float ymultiplier = 0.1f)
    {
        return new Vector3(Random.Range(-radius, radius), Random.Range(-radius, radius), 0);
    }

    public static float dist2dCompare(Vector3 a, Vector3 b)
    {
        return (a.x - b.x) * (a.x - b.x) + (a.z - b.z) * (a.z - b.z);
    }

    public static float dist2d(Vector3 a, Vector3 b)
    {
        return Mathf.Sqrt((a.x - b.x) * (a.x - b.x) + (a.y - b.y) * (a.y - b.y));
    }

    public static float sqrMagnitude2D(Vector3 vec)
    {
        return vec.x * vec.x + vec.z * vec.z;
    }

    public static float angfrom(float x1, float y1, float x2, float y2)
    {
        return Mathf.Atan2(x2 - x1, y2 - y1); //180.0f / 3.14159f;
    }

    public static float angfrom(Vector3 a, Vector3 b)
    {
        return Mathf.Atan2(b.x - a.x, b.z - a.z); //180.0f / 3.14159f;
    }

    public static float angfromDegrees(float x1, float y1, float x2, float y2)
    {
        return Mathf.Atan2(x2 - x1, y2 - y1) * 57.32f; //180.0f / 3.14159f;
    }

    public static float angdist(float angle1, float angle2)
    {
        float angle = Mathf.Abs(angle1 - angle2);
        float oppositeangle = 6.283185f - angle;
        if (angle < oppositeangle) return angle; else return oppositeangle;
    }

    public static float angdistDegrees(float angle1, float angle2)
    {
        float angle = Mathf.Abs(angle1 - angle2);
        float oppositeangle = 360f - angle;
        if (angle < oppositeangle) return angle; else return oppositeangle;
    }

    public static float turndirection(float startangle, float endangle, float acceptablerange = 2.0f)
    {
        float result = 1.0f;

        float angle = endangle - startangle;
        float abs_angle = Mathf.Abs(angle);
        if (abs_angle < acceptablerange) return 0.0f; //close enough
        if (angle < 0.0f) result = -1.0f;

        //check if opposite direction is better
        float oppositeangle = 6.283185f - abs_angle;
        if (oppositeangle < abs_angle) result *= -1.0f;

        return result;
    }

    public static float TurnTo(float currentangle, float targetangle, float turnspeed)
    {

        //turn towards destination											//turn speed
        float turndir = turndirection(currentangle, targetangle, turnspeed);

        if (turndir == 1.0f)
        {
            currentangle = currentangle + turnspeed;
        }
        if (turndir == -1.0f)
        {
            currentangle = currentangle - turnspeed;
        }
        if (turndir == 0.0f)
        {
            currentangle = targetangle;
        }

        return currentangle;
    }

    public static int turndirectionDegrees(float startangle, float endangle, float acceptablerange = 2.0f)
    {
        int result = 1;

        float angle = endangle - startangle;
        float abs_angle = Mathf.Abs(angle);
        if (abs_angle < acceptablerange) return 0; //close enough
        if (angle < 0.0f) result = -1;

        //check if opposite direction is better
        float oppositeangle = 360f - abs_angle;
        if (oppositeangle < abs_angle) result *= -1;

        return result;
    }

    public static bool within(Vector2 position, Rect dimensions)
    {
        return position.x >= dimensions.x && position.y >= dimensions.y && position.x <= dimensions.x + dimensions.width && position.y <= dimensions.y + dimensions.height;
    }
    public static bool within(float positionx, float positiony, float left, float top, float right, float bottom)
    {
        return positionx >= left && positiony >= top && positionx <= right && positiony <= bottom;
    }

    public static float larger(float a, float b)
    {
        if (a > b) return a; else return b;
    }
    public static float smaller(float a, float b)
    {
        if (a < b) return a; else return b;
    }
    public static int larger(int a, int b)
    {
        if (a > b) return a; else return b;
    }
    public static int smaller(int a, int b)
    {
        if (a < b) return a; else return b;
    }

    public static void Swap<T>(ref T lhs, ref T rhs)
    {
        T temp;
        temp = lhs;
        lhs = rhs;
        rhs = temp;
    }

    public static float IdentifySide(Vector3 original, Vector3 test)
    {
        if (test.z * original.x > test.x * original.z)
        {
            return -1f;
        }
        else
        {
            return 1f;
        }
    }

    public static float Sinf_QuadraticCurveArc(float percentage, float archeight = 1.0f)
    {
        float c = (2.0f * percentage - 1.0f);
        return (-c * c + 1.0f) * archeight;
    }




    public static bool PointInPoly(float x, float z, LinkedList<Vector3> pointlist)
    {
        if (pointlist.Count < 3) return false;

        float x1, y1, x2, y2;       /* Coords of poly's vertices */
        int numOfCrossings = 0,     /* Count of poly's edges crossed */
        vCt = 0,            /* Vertex counter */
        numVerts = pointlist.Count;

        Vector3 point = pointlist.Last.Value;
        x1 = point.x;       /* Start with the last edge of p */
        y1 = point.z;

        /*
    * For each edge e of polygon p, see if the ray from (x,y) to (infinity,y)
    * crosses e:
    */
        foreach (Vector3 pt in pointlist)
        {
            x2 = pt.x;
            y2 = pt.z;

            /*
        * If y is between (y1,y2] (e's y-range),
        * and (x,y) is to the left of e, then
        *     the ray crosses e:
        */
            if ((((y2 <= z) && (z < y1)) || ((y1 <= z) && (z < y2)))
                && (x < (x1 - x2) * (z - y2) / (y1 - y2) + x2))
            {
                numOfCrossings++;
            }

            x1 = x2;
            y1 = y2;
            vCt++;
        } while (vCt < numVerts) ;

        return ((numOfCrossings % 2) == 1);
    }

    public static float makposang(float heading)
    {
        if (heading > 6.283185f) heading -= 6.283185f;
        else if (heading < 0f) heading += 6.283185f;
        return heading;
    }






    public static string LoadTextFile(string filename)
    {
        /*System.Text.StringBuilder sb = new System.Text.StringBuilder();
        //using (System.IO.StreamReader sr = new System.IO.StreamReader(Application.streamingAssetsPath + "/" + filename))
        using (System.IO.StreamReader sr = new System.IO.StreamReader(Application.dataPath + "/Resources/" + filename))
        {
            string line;
            // Read and display lines from the file until the end of 
            // the file is reached.
            while ((line = sr.ReadLine()) != null)
            {
                sb.AppendLine(line);
            }
        }
        return sb.ToString();*/

        TextAsset stringdata = (TextAsset)Resources.Load(filename, typeof(TextAsset));
        return stringdata.text; //returns string of entire file
    }
}
