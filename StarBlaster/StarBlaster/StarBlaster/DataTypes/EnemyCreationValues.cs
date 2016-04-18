using FlatRedBall.Math.Splines;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StarBlaster.DataTypes
{
    class EnemyCreationValues
    {
        public Spline Spline;
        public int Count = 1;
        public float Delay;
        public EnemyInfo EnemyInfo;
        public Vector3 Offset;
        public float TimeSeparation = .5f;

        public int? SplinePointToLoopTo = null;
    }
}
