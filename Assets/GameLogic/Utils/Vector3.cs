using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ThirdEyeSoftware.GameLogic;

namespace GameLogic.Utils
{
    public class Vector3 : IVector3
    {
        public float X { get; set; }
        public float Y { get; set; }
        public float Z { get; set; }

        public Vector3(float x, float y, float z)
        {
            X = x;
            Y = y;
            Z = z;
        }
    }
}
