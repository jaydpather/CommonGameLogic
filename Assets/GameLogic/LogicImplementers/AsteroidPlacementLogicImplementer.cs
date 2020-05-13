using GameLogic.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ThirdEyeSoftware.GameLogic;
using ThirdEyeSoftware.GameLogic.Utils;
//using UnityEngine;

namespace GameLogic.LogicImplementers
{
    public class AsteroidPlacement
    {
        public float ZCoord { get; set; }
        public int NumAsteroids { get; set; }
        public List<MinMax> XCoordRanges { get; set; }
        public string LineName { get; set; }
    }

    public class MinMax
    {
        public float Min { get; set; }
        public float Max { get; set; }
    }

    public interface IAsteroidPlacementLogicImplementer
    {
        IGameObject InitAsteroids(int length, int numAsteroidLines);
        IGameObject SourceAsteroid { set; }
        Action<string> LogToDebugOutput { set; }
        List<IGameObject> AllAsteroids { get; set; }
        int AsteroidDensity { set; }
    }

    public class AsteroidPlacementLogicImplementer : IAsteroidPlacementLogicImplementer
    {
        private float X_MIN;
        private float X_MAX;
        private float ASTEROID_WIDTH;
        private readonly float SHIP_WIDTH;

        private IGameObject _sourceAsteroid;
        public IGameObject SourceAsteroid
        {
            set
            {
                _sourceAsteroid = value;
                ASTEROID_WIDTH = value.GetSize().X;
            }
        }
        public List<IGameObject> AllAsteroids
        {
            get; set;
        }

        private IGameEngineInterface _gameEngineInterface;
        private RandomNumberGenerator _initialRandomGenrator = new RandomNumberGenerator(0.15f); //used to place the inital asteroid on the line
        private RandomNumberGenerator _subsequentRandomGenerator = new RandomNumberGenerator(0.5f); //used to place subsequent asteroids on the line
        private int _asteroidDensity;
        public int AsteroidDensity
        {
            set
            {
                _asteroidDensity = value;
            }
        }

        private Action<string> _logToDebugOutput;
        public Action<string> LogToDebugOutput
        {
            set
            {
                _logToDebugOutput = value;
            }
        }

        public AsteroidPlacementLogicImplementer(IGameEngineInterface gameEngineInterface, int asteroidDensity)
        {
            _gameEngineInterface = gameEngineInterface;
            _asteroidDensity = asteroidDensity;

            var playerShip = gameEngineInterface.FindGameObject("PlayerShip");
            SHIP_WIDTH = playerShip.GetSize().X;

            AllAsteroids = new List<IGameObject>();
        }

        /// <summary>
        /// calculates the world-space x-coordinates for the left edge (X_MIN) and right edge (X_MAX) of the screen
        /// </summary>
        /// <returns>a float array, where index 0 is X_MIN and index 1 is X_MAX</returns>
        private float[] CalculateXMinAndXMax()
        {
            var retVal = new float[2];

            var camera = _gameEngineInterface.FindGameObject("MainCamera"); //todo next game: this should probably be passed in as a property
            var distanceFromCamera = camera.Transform.Position.Y - _sourceAsteroid.Transform.Position.Y;

            var screenPointXMin = new Utils.Vector3(0, 0, distanceFromCamera);
            var worldPointXMin = _gameEngineInterface.ScreenUtils.GetWorldPointFromScreenPoint(screenPointXMin);
            retVal[0] = worldPointXMin.X;

            var screenPointXMax = new Utils.Vector3(_gameEngineInterface.Screen.Width, 0, distanceFromCamera);
            var worldPointXMax = _gameEngineInterface.ScreenUtils.GetWorldPointFromScreenPoint(screenPointXMax);
            retVal[1] = worldPointXMax.X;

            return retVal;
        }

        public AsteroidPlacement GetAsteroidPlacement(int lineNo)
        {
            return new AsteroidPlacement
            {
                NumAsteroids = 2, 
                XCoordRanges = new List<MinMax>()
                {
                    new MinMax(),
                    new MinMax()
                }
            };
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="length"></param>
        /// <param name="numAsteroidLines"></param>
        /// <returns>returns the last asteroid. (asteroid that is furthest from the player)</returns>
        public IGameObject InitAsteroids(int length, int numAsteroidLines)
        {
            var xMinMax = CalculateXMinAndXMax();
            X_MIN = xMinMax[0];
            X_MAX = xMinMax[1];


            const int Z_MIN = 5;
            IGameObject lastAsteroid = null;
            float? prevRandomPct = null;

            var random = new System.Random(DateTime.Now.Millisecond);
            AllAsteroids.Clear();
            for (int i = 0; i < numAsteroidLines; i++)
            {
                var asteroidPlacement = GetAsteroidPlacement(i);
                asteroidPlacement.ZCoord = GetZCoord(length, numAsteroidLines, Z_MIN, random, i);
                asteroidPlacement.LineName = i.ToString();

                var asteroids = PlaceAsteroidsOnLine(random, asteroidPlacement, ref prevRandomPct);

                foreach(var curAsteroid in asteroids)
                {
                    AllAsteroids.Add(curAsteroid);

                    lastAsteroid = curAsteroid; //this will keep getting updated each time we place a new asteroid or new line of asteroids
                    //old version used to assume the last line would contain the last asteroid
                    //but it's possible the last line contains 0 asteroids,
                    //in which case asteroids.Length == 0, so the old version got an IndexOutOfBoundsException
                }
            }

            _sourceAsteroid.SetActive(false); //we don't want the original asteroid in the game. it exists only so that we can clone it.

            return lastAsteroid;
        }

        private IGameObject[] PlaceAsteroidsOnLine(System.Random random, AsteroidPlacement asteroidPlacement, ref float? prevRandomPct)
        {
            var retVal = new List<IGameObject>();
            var xcoordRangeIndex = 0;
            var prevXCoord = X_MIN; //we want the asteroids to start at the left edge of the screen, but the source asteroid is in the center of the screen, so we need to start with this offset value. (X_MIN is negative)

            Utils.Vector3 transVector;
            do
            {               
                var isFirstAsteroidOnLine = (xcoordRangeIndex == 0);
                transVector = GetAsteroidPosition(random, asteroidPlacement, prevXCoord, isFirstAsteroidOnLine); //todo 2nd game: should GetAsteroidPosition be moved to a different class? That way, we can mock its return value and test the loop logic

                if (transVector != null) //null value means we've placed all the asteroids we can fit on this line
                {
                    var curAsteroid = _gameEngineInterface.Clone(_sourceAsteroid, asteroidPlacement.LineName + xcoordRangeIndex.ToString());

                    //curAsteroid.Transform.Translate(10, 0, -100); //source asteroid's initial position is -10, 0, 0
                    curAsteroid.Transform.Translate(transVector.X, transVector.Y, transVector.Z);
                    retVal.Add(curAsteroid);

                    prevXCoord = transVector.X;
                    xcoordRangeIndex++;
                }
            } while (transVector != null);

            return retVal.ToArray();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="random"></param>
        /// <param name="asteroidPlacement"></param>
        /// <param name="prevXCoord"></param>
        /// <param name="isFirstAsteroidOnLine"></param>
        /// <returns>returns a translation vector, telling you how far to translate an asteroid from its source position</returns>
        private Utils.Vector3 GetAsteroidPosition(System.Random random, AsteroidPlacement asteroidPlacement, float prevXCoord, bool isFirstAsteroidOnLine)
        {
            var distance = getCurDistance(_asteroidDensity, isFirstAsteroidOnLine);
            var xCoord = prevXCoord + distance;

            if (xCoord > X_MAX - (ASTEROID_WIDTH/2))
            {
                //if we placed an asteroid at this xCoord, it would be partially or completely off-screen. (so we're done placing asteroids on this line)
                return null;
            }
            else
            {
                return new Utils.Vector3(xCoord, 0, asteroidPlacement.ZCoord);
            }
        }

        private float getCurDistance(int densityLevel, bool isFirstAsteroidOnLine)
        {
            var retVal = 0f;

            var minDistancePerfect = SHIP_WIDTH + ASTEROID_WIDTH; //the minimum distance the asteroids need to be for the perfect player, who can fit his ship between the asteroids with no room to spare

            float minDistanceDefault = minDistancePerfect * 2f;
            float maxDistanceDefault = minDistancePerfect * 2.5f;

            var maxDistance = 0f;
            var minDistance = 0f;
            switch (densityLevel)
            {
                case 1:
                    maxDistance = maxDistanceDefault * 1.436f;//1.515f;
                    minDistance = minDistanceDefault * 1.436f;//1.515f;
                    break;
                case 2:
                    maxDistance = maxDistanceDefault * 1.202f;//1.139f;
                    minDistance = minDistanceDefault * 1.202f;//1.139f;
                    break;
                case 3:
                    maxDistance = maxDistanceDefault * 0.968f;
                    minDistance = minDistanceDefault * 0.968f;
                    break;
            }

            if (isFirstAsteroidOnLine)
            {
                minDistance = 0 + (ASTEROID_WIDTH/2f); //first asteroid can be all the way up to the left edge of the screen, but not hanging off the edge
                var screenWidth = X_MAX - X_MIN;
                var clampMax = screenWidth - (ASTEROID_WIDTH / 2f);
                //maxDistance = Mathf.Clamp(maxDistance, minDistance, clampMax); //by clamping here (instead of after we've returned, clamped asteroids will not always be at the right edge of the screen)

                retVal = _initialRandomGenrator.GetRandomValue(minDistance, maxDistance);
            }
            else
            {
                retVal = _subsequentRandomGenerator.GetRandomValue(minDistance, maxDistance);
            }

            return retVal;
        }

        private float GetZCoord(int length, int numAsteroidLines, int zMin, System.Random random, int i) 
        {
            var zOffset = ((float)length / numAsteroidLines) * i;

            //todo real game: add randomized z-offset:
            //var zOffsetRandomPct = GetRandomPct(random, null, 0.0f);
            //var zOffsetRandomMin = 0.5f;
            //var zOffsetRandomMax = 1.5f;
            //var zRandomOffset = zOffsetRandomMin + (zOffsetRandomPct * (zOffsetRandomMax - zOffsetRandomMin));

            var zCoord = zMin + zOffset;
            return zCoord;
        }
    }
}
