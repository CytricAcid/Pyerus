using UnityEngine;
using System;
using System.Collections.Generic;
using System.Text;

namespace STB.ADAOPS
{
    ///////////////////////////////////////////////////////////////////////////////////////////////////////
    /// <summary>
    /// Class: GenericSplineFunction
    /// # Function to generate an spline giving points to interpolate positions between them
    /// </summary>
    ///////////////////////////////////////////////////////////////////////////////////////////////////////
    public class GenericSplineFunction
    {
        // const
        const int MAX_KEYPOINTS = 9;
        const int MAX_CUBICS = 900;

        // private
        Vector3[] keyPointVector = new Vector3[MAX_KEYPOINTS];
        List<Vector3> pathVector = new List<Vector3>();
        int keyPointCount = 0;
        int pathCount = 0;
        float actualLength = 0;
        int totalLength = 1000;
        bool endState = false;
        Vector3 lastValue = Vector3.zero;
        int intLength = 0;
        float multiplicadorExtra = 0.4f;
        float suma = 0;
        Vector3[] positions = new Vector3[MAX_KEYPOINTS];
        float k;
        Vector3[] gamma = new Vector3[MAX_CUBICS];
        Vector3[] delta = new Vector3[MAX_CUBICS];
        Vector3[] D = new Vector3[MAX_CUBICS];
        Cubic[] pos_cubic = new Cubic[MAX_CUBICS];
        Cubic[] C = new Cubic[MAX_CUBICS];
        bool initializeCubics = true;
        int intermediateSteps = 400;


        ////////////////////////////////////////////////////////////////////////////////////
        /// <summary> //////////////////////////////////////////////////////////////////////
        /// Constructor 2
        /// </summary> /////////////////////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////////////////   
        public GenericSplineFunction()
        {
            Initialize();
        }
        ////////////////////////////////////////////////////////////////////////////////////
        /// <summary> //////////////////////////////////////////////////////////////////////
        /// Constructor 2
        /// </summary> /////////////////////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////////////////   
        public GenericSplineFunction(int intermediateSteps)
        {
            this.intermediateSteps = intermediateSteps;
            this.multiplicadorExtra = 0.4f * this.intermediateSteps / 400;

            Initialize();
        }
        ////////////////////////////////////////////////////////////////////////////////////
        /// <summary> //////////////////////////////////////////////////////////////////////
        /// Initialize
        /// </summary> /////////////////////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////////////////   
        void Initialize()
        {
            if (initializeCubics)
            {
                initializeCubics = false;

                for (int i = 0; i < MAX_CUBICS; i++)
                {
                    pos_cubic[i] = new Cubic();
                    C[i] = new Cubic();
                }
            }
        }
        ////////////////////////////////////////////////////////////////////////////////////
        /// <summary> //////////////////////////////////////////////////////////////////////
        /// SetAleatLength
        /// </summary> /////////////////////////////////////////////////////////////////////
        //////////////////////////////////////////////////////////////////////////////////// 
        public void SetAleatLength()
        {
            actualLength = totalLength * ((float)UnityEngine.Random.Range(0, 1000)) / 1000.0f;
        }
        ////////////////////////////////////////////////////////////////////////////////////
        /// <summary> //////////////////////////////////////////////////////////////////////
        /// EndState
        /// </summary> /////////////////////////////////////////////////////////////////////
        //////////////////////////////////////////////////////////////////////////////////// 
        public bool EndState
        {
            get { return endState; }
        }
        ////////////////////////////////////////////////////////////////////////////////////
        /// <summary> //////////////////////////////////////////////////////////////////////
        /// TotalLength
        /// </summary> /////////////////////////////////////////////////////////////////////
        //////////////////////////////////////////////////////////////////////////////////// 
        public int TotalLength
        {
            get { return totalLength; }
            set { totalLength = value; }
        }
        ////////////////////////////////////////////////////////////////////////////////////
        /// <summary> //////////////////////////////////////////////////////////////////////
        /// ActualLength
        /// </summary> /////////////////////////////////////////////////////////////////////
        //////////////////////////////////////////////////////////////////////////////////// 
        public float ActualLength
        {
            get { return actualLength; }
            set { actualLength = value; }
        }
        ////////////////////////////////////////////////////////////////////////////////////
        /// <summary> //////////////////////////////////////////////////////////////////////
        /// AddPoint
        /// </summary> /////////////////////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////////////////
        public void AddPoint(Vector3 point, bool close)
        {
            keyPointVector[keyPointCount] = point;
            keyPointCount++;

            if (close)
            {
                Build();
            }
        }
        ////////////////////////////////////////////////////////////////////////////////////
        /// <summary> //////////////////////////////////////////////////////////////////////
        /// Clear
        /// </summary> /////////////////////////////////////////////////////////////////////
        //////////////////////////////////////////////////////////////////////////////////// 
        public void Clear()
        {
            keyPointCount = 0;
        }
        ////////////////////////////////////////////////////////////////////////////////////
        /// <summary> //////////////////////////////////////////////////////////////////////
        /// Restart
        /// </summary> /////////////////////////////////////////////////////////////////////
        //////////////////////////////////////////////////////////////////////////////////// 
        public void Restart()
        {
            actualLength = 0;
            endState = false;
        }
        ////////////////////////////////////////////////////////////////////////////////////
        /// <summary> //////////////////////////////////////////////////////////////////////
        /// GetPoint
        /// </summary> /////////////////////////////////////////////////////////////////////
        //////////////////////////////////////////////////////////////////////////////////// 
        public Vector3 GetPoint(float length)
        {
            intLength = (int)length;
            if ((intLength < 0) || (intLength > TotalLength))
            {
                return lastValue;
            }
            else if (intLength < pathCount)
            {
                lastValue = pathVector[intLength];
                return pathVector[intLength];
            }
            return lastValue;
        }
        ////////////////////////////////////////////////////////////////////////////////////
        /// <summary> //////////////////////////////////////////////////////////////////////
        /// GetActualPoint
        /// </summary> /////////////////////////////////////////////////////////////////////
        //////////////////////////////////////////////////////////////////////////////////// 
        public Vector3 GetActualPoint()
        {
            return GetPoint(actualLength);
        }
        ////////////////////////////////////////////////////////////////////////////////////
        /// <summary> //////////////////////////////////////////////////////////////////////
        /// Constructor
        /// </summary> /////////////////////////////////////////////////////////////////////
        //////////////////////////////////////////////////////////////////////////////////// 
        public float GetLengthProportion()
        {
            return (actualLength / (float)totalLength);
        }
        ////////////////////////////////////////////////////////////////////////////////////
        /// <summary> //////////////////////////////////////////////////////////////////////
        /// ForceEnd
        /// </summary> /////////////////////////////////////////////////////////////////////
        //////////////////////////////////////////////////////////////////////////////////// 
        public void ForceEnd()
        {
            actualLength = totalLength;
            endState = true;
        }
        ////////////////////////////////////////////////////////////////////////////////////
        /// <summary> //////////////////////////////////////////////////////////////////////
        /// UpdateUntilEnd
        /// </summary> /////////////////////////////////////////////////////////////////////
        //////////////////////////////////////////////////////////////////////////////////// 
        public void UpdateUntilEnd(float time)
        {
            if (!endState)
            {
                suma = multiplicadorExtra * time;
                actualLength += suma;
                if (actualLength > totalLength)
                {
                    actualLength = totalLength;
                    endState = true;
                }
            }
            if (actualLength == 0)
            {
                endState = false;
            }
        }
        ////////////////////////////////////////////////////////////////////////////////////
        /// <summary> //////////////////////////////////////////////////////////////////////
        /// MiddleStatePassed
        /// </summary> /////////////////////////////////////////////////////////////////////
        //////////////////////////////////////////////////////////////////////////////////// 
        public bool MiddleStatePassed
        {
            get { return (actualLength > 0.5f * totalLength); }
        }
        ////////////////////////////////////////////////////////////////////////////////////
        /// <summary> //////////////////////////////////////////////////////////////////////
        /// Update
        /// </summary> /////////////////////////////////////////////////////////////////////
        //////////////////////////////////////////////////////////////////////////////////// 
        public void Update(float time)
        {
            actualLength += multiplicadorExtra * time;
            if (actualLength > totalLength - 1)
            {
                Restart();
            }
        }
        ////////////////////////////////////////////////////////////////////////////////////
        /// <summary> //////////////////////////////////////////////////////////////////////
        /// Build
        /// </summary> /////////////////////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////////////////
        void Build()
        {
            pathVector.Clear();

            if (keyPointCount > 1)
            {
                pathCount = 0;

                for (int i = 0; i < keyPointCount; i++)
                {
                    positions[i] = keyPointVector[i];
                }
                pos_cubic = CalculateCubicSpline(keyPointCount - 1, positions);

                for (int i = 0; i < keyPointCount - 1; i++)
                {
                    for (int j = 0; j < intermediateSteps; j++)
                    {
                        k = (float)j / (float)(intermediateSteps - 1);

                        //////Console.WriteLine(pathCount.ToString());
                        pathVector.Add((pos_cubic[i].GetPointOnSpline(k)));
                        pathCount++;
                    }
                }
            }
            totalLength = pathCount - 1;
        }
        ////////////////////////////////////////////////////////////////////////////////////
        /// <summary> //////////////////////////////////////////////////////////////////////
        /// CalculateCubicSpline
        /// </summary> /////////////////////////////////////////////////////////////////////
        //////////////////////////////////////////////////////////////////////////////////// 
        Cubic[] CalculateCubicSpline(int n, Vector3[] v)
        {
            //this builds the coefficients of the left matrix
            gamma[0] = Vector3.zero;
            gamma[0].x = 1.0f / 2.0f;
            gamma[0].y = 1.0f / 2.0f;
            gamma[0].z = 1.0f / 2.0f;
            for (int i = 1; i < n; i++)
            {
                gamma[i].x = Vector3.one.x / ((4 * Vector3.one.x) - gamma[i - 1].x);
                gamma[i].y = Vector3.one.y / ((4 * Vector3.one.y) - gamma[i - 1].y);
                gamma[i].z = Vector3.one.z / ((4 * Vector3.one.z) - gamma[i - 1].z);
            }
            gamma[n].x = Vector3.one.x / ((2 * Vector3.one.x) - gamma[n - 1].x);
            gamma[n].y = Vector3.one.y / ((2 * Vector3.one.y) - gamma[n - 1].y);
            gamma[n].z = Vector3.one.z / ((2 * Vector3.one.z) - gamma[n - 1].z);

            delta[0].x = 3 * (v[1].x - v[0].x) * gamma[0].x;
            delta[0].y = 3 * (v[1].y - v[0].y) * gamma[0].y;
            delta[0].z = 3 * (v[1].z - v[0].z) * gamma[0].z;

            for (int i = 1; i < n; i++)
            {
                delta[i].x = (3 * (v[i + 1].x - v[i - 1].x) - delta[i - 1].x) * gamma[i].x;
                delta[i].y = (3 * (v[i + 1].y - v[i - 1].y) - delta[i - 1].y) * gamma[i].y;
                delta[i].z = (3 * (v[i + 1].z - v[i - 1].z) - delta[i - 1].z) * gamma[i].z;
            }
            delta[n].x = (3 * (v[n].x - v[n - 1].x) - delta[n - 1].x) * gamma[n].x;
            delta[n].y = (3 * (v[n].y - v[n - 1].y) - delta[n - 1].y) * gamma[n].y;
            delta[n].z = (3 * (v[n].z - v[n - 1].z) - delta[n - 1].z) * gamma[n].z;

            D[n] = delta[n];
            for (int i = n - 1; i >= 0; i--)
            {
                D[i].x = delta[i].x - gamma[i].x * D[i + 1].x;
                D[i].y = delta[i].y - gamma[i].y * D[i + 1].y;
                D[i].z = delta[i].z - gamma[i].z * D[i + 1].z;
            }

            // now compute the coefficients of the cubics 
            for (int i = 0; i < n; i++)
            {
                C[i].a = v[i];
                C[i].b = D[i];
                C[i].c = 3 * (v[i + 1] - v[i]) - 2 * D[i] - D[i + 1];
                C[i].d = 2 * (v[i] - v[i + 1]) + D[i] + D[i + 1];
            }
            return C;
        }
        ////////////////////////////////////////////////////////////////////////////////////
        /// <summary> //////////////////////////////////////////////////////////////////////
        /// Cubic
        /// </summary> /////////////////////////////////////////////////////////////////////
        //////////////////////////////////////////////////////////////////////////////////// 
        internal class Cubic
        {
            public Vector3 a, b, c, d; // a + b*s + c*s^2 +d*s^3 

            public Cubic()
            {
                this.a = Vector3.zero;
                this.b = Vector3.zero;
                this.c = Vector3.zero;
                this.d = Vector3.zero;
            }

            //evaluate the point using a cubic equation
            public Vector3 GetPointOnSpline(float s)
            {
                return (((d * s) + c) * s + b) * s + a;
            }
        }
    }
}