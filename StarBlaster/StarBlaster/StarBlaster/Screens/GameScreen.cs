#region Usings

using System;
using System.Collections.Generic;
using System.Text;
using FlatRedBall;
using FlatRedBall.Input;
using FlatRedBall.Instructions;
using FlatRedBall.AI.Pathfinding;
using FlatRedBall.Graphics.Animation;
using FlatRedBall.Graphics.Particle;

using FlatRedBall.Math.Geometry;
using FlatRedBall.Math.Splines;

using Cursor = FlatRedBall.Gui.Cursor;
using GuiManager = FlatRedBall.Gui.GuiManager;
using FlatRedBall.Localization;
using StarBlaster.Entities;
using StarBlaster.Factories;
using StarBlaster.DataTypes;
using OcularPlane;
using OcularPlane.Networking.WcfTcp.Host;
using System.Collections.Specialized;

#if FRB_XNA || SILVERLIGHT
using Keys = Microsoft.Xna.Framework.Input.Keys;
using Vector3 = Microsoft.Xna.Framework.Vector3;
using Texture2D = Microsoft.Xna.Framework.Graphics.Texture2D;
#endif
#endregion

namespace StarBlaster.Screens
{
	public partial class GameScreen
	{

		void CustomInitialize()
        {
            // We'll handle it ourselves since we will have enemy and player bullets
            BulletFactory.ScreenListReference = null;

            this.PlayerShipList.CollectionChanged += HandleCollectionChanged;

            CreatePlayer();

            AddEnemySpawning();

            CalculateSplineValues();

        }

        private void HandleCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Add)
            {
                foreach (var item in e.NewItems)
                {
                    containerManager.AddObjectToContainer(nameof(GameScreen),
                        item, nameof(ExplosionInstance));
                }
            }
        }

        private void AddEnemySpawning()
        {
            var values = new EnemyCreationValues();
            //values.Spline = SplinePaths.Find(item=>item.Name == "LtoRLoop");
            //values.Count = 5;
            //values.Delay = 3;
            //values.EnemyInfo = GlobalContent.EnemyInfo[EnemyInfo.Regular];

            //CreateEnemyShips(values);


            //for(int i = 0; i < 10; i++)
            //{
            //    values = new EnemyCreationValues();
            //    values.Spline = SplinePaths.Find(item=>item.Name == "DownThenUp");
            //    values.Count = 1;
            //    values.Delay = 10 + i/2.0f;
            //    values.EnemyInfo = GlobalContent.EnemyInfo[EnemyInfo.Regular];
            //    values.Offset = new Vector3(-300 + i * 600 / 10, 0, 0);

            //    CreateEnemyShips(values);

            //}

            values = new EnemyCreationValues();
            values.Delay = 1;
            //values.Delay = 15;
            values.EnemyInfo = GlobalContent.EnemyInfo[EnemyInfo.ExtraBig];
            values.Spline = SplinePaths.Find(item => item.Name == "LeftToRight");
            values.SplinePointToLoopTo = 2;
            values.Count = 1;
            CreateEnemyShips(values);

        }

        private static void CalculateSplineValues()
        {
            foreach (var spline in SplinePaths)
            {
                spline.Visible = false;
                // The smaller the number, the more accurate the spline position estimate becomes, but
                // it take smore time to precalculate. 
                spline.CalculateAccelerations();
                spline.CalculateDistanceTimeRelationships(.1f);
            }
        }

        void CustomActivity(bool firstTimeCalled)
		{
            CollisionActivity();

            KeepPlayersInScreen();

            BulletOffScreenDestructionActivity();
		}

        private void BulletOffScreenDestructionActivity()
        {
            var camera = Camera.Main;

            const float leeway = 30;

            var left = camera.AbsoluteLeftXEdgeAt(0) - leeway;
            var right = camera.AbsoluteRightXEdgeAt(0) + leeway;
            var top = camera.AbsoluteTopYEdgeAt(0) + leeway;
            var bottom = camera.AbsoluteBottomYEdgeAt(0) - leeway;

            // makes sure the bullets are fully out of screen

            for (int i = EnemyBulletList.Count - 1; i > -1; i--)
            {
                var bullet = EnemyBulletList[i];

                if(bullet.X < left || bullet.X > right || bullet.Y < bottom || bullet.Y > top)
                {
                    bullet.Destroy();
                }
            }

            for (int i = PlayerBulletList.Count - 1; i > -1; i--)
            {
                var bullet = PlayerBulletList[i];

                if (bullet.X < left || bullet.X > right || bullet.Y < bottom || bullet.Y > top)
                {
                    bullet.Destroy();
                }
            }
        }

        private void KeepPlayersInScreen()
        {
            var camera = Camera.Main;

            var left = camera.AbsoluteLeftXEdgeAt(0);
            var right = camera.AbsoluteRightXEdgeAt(0);
            var top = camera.AbsoluteTopYEdgeAt(0);
            var bottom = camera.AbsoluteBottomYEdgeAt(0);

            // all players have the same radius:
            var radius = PlayerShipList[0].CollisionRadius;

            foreach(var player in PlayerShipList)
            {
                player.X = Math.Max(player.X, left + radius);
                player.X = Math.Min(player.X, right - radius);

                player.Y = Math.Max(player.Y, bottom + radius);
                player.Y = Math.Min(player.Y, top - radius);
            }
        }

        private void CollisionActivity()
        {
            PlayerBulletVsEnemyShipCollision();
        }

        private void PlayerBulletVsEnemyShipCollision()
        {
            for(int i = PlayerBulletList.Count -1; i > -1; i--)
            {
                var bullet = PlayerBulletList[i];

                for(int j = EnemyShipList.Count -1; j > -1; j--)
                {
                    var enemy = EnemyShipList[j];

                    if (bullet.CollideAgainst(enemy))
                    {
                        enemy.TakeDamage(1);
                        bullet.Destroy();
                        break;
                    }
                }
            }
        }

        void CustomDestroy()
		{


		}

        static void CustomLoadStaticContent(string contentManagerName)
        {


        }

        void CreateEnemyShips(EnemyCreationValues values)
        {
           this.Call(() =>
           {
               for (int i = 0; i < values.Count; i++)
               {
                   this.Call(() =>
                   {
                       var enemyShip = EnemyShipFactory.CreateNew();
                        // Move it off screen, it'll adjust its positing
                        // in its custo activity using the spline
                       enemyShip.X = -10000;
                       enemyShip.SplinePointToLoopTo = values.SplinePointToLoopTo;
                       enemyShip.SplineOffset = values.Offset;
                       enemyShip.SplineFollowing = values.Spline;
                       enemyShip.EnemyInfo = values.EnemyInfo;
                   })
                   .After(i * values.TimeSeparation);

               }
           }).After(values.Delay);
        }

        public void CreatePlayer()
        {
            PlayerShip playerShip = new PlayerShip();
            this.PlayerShipList.Add(playerShip);
            playerShip.Y = -260;
            playerShip.AddKeyboardControls();

            playerShip.BulletCreated += HandleBulletCreated;
        }

        private void HandleBulletCreated(Bullet newBullet)
        {
            PlayerBulletList.Add(newBullet);
        }
    }
}
