using System;
using FlatRedBall;
using FlatRedBall.Input;
using FlatRedBall.Instructions;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Specialized;
using FlatRedBall.Audio;
using FlatRedBall.Screens;
using StarBlaster.Entities;
using StarBlaster.Screens;
using FlatRedBall.Math.Geometry;

namespace StarBlaster.Entities
{
	public partial class EnemyShip
	{
        void OnAfterEnemyInfoSet (object sender, EventArgs e)
        {
            var enemyInfo = this.EnemyInfo;
            this.health = enemyInfo.Health;
            this.SpriteInstance.CurrentChainName = enemyInfo.AnimationChain;

            Collision.Clear();

            switch (enemyInfo.Name)
            {
                case DataTypes.EnemyInfo.Regular:
                    AddToCollision(RegularCollision);
                    break;
                case DataTypes.EnemyInfo.Big:
                    AddToCollision(BigCollision);
                    break;
                case DataTypes.EnemyInfo.ExtraBig:
                    AddToCollision(ExtraBigCollision);
                    break;
                case DataTypes.EnemyInfo.Boss:
                    AddToCollision(BossCollision);
                    break;
            }
        }

        void AddToCollision(Circle circle)
        {
            this.Collision.Circles.Add(circle);
        }

        void AddToCollision(Polygon polygon)
        {
            this.Collision.Polygons.Add(polygon);
        }
		
	}
}
