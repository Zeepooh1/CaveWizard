using System.Collections.Generic;
using System.Security.Cryptography;

namespace CaveWizard.Game
{
    public class Generator
    {
        private int _levelWidth;
        private int _levelHeight;
        private int _levelNumber;
        public List<List<char>> LevelPresented { get; }
        
        public Generator(int levelNumber)
        {
            _levelWidth = 10;
            _levelHeight = 10;
            _levelWidth += (levelNumber / 2) * 4;
            _levelHeight += (levelNumber / 2) * 4;
            LevelPresented = new List<List<char>>(_levelHeight);
            for (int i = 0; i < _levelHeight; i++)
            {
                LevelPresented.Add(new List<char>(_levelWidth));
                for (int j = 0; j < _levelWidth; j++)
                {
                    LevelPresented[i].Add('.');
                }
            }

            setFloors();
            setPlayerStart();
            setPlayerFinish();
            generateHoles();
            generateEnemies();
        }

        private void setPlayerFinish()
        {
            int playerFinish;
            do
            {
                playerFinish = RandomNumberGenerator.GetInt32(_levelWidth);
            } while (LevelPresented[_levelHeight - 2][playerFinish] != '.');

            LevelPresented[_levelHeight - 2][playerFinish] = 'F';
        }

        private void generateEnemies()
        {
            int maxNumOfEnemies = _levelWidth / 3;
            int numOfEnemies;
            int enemyPos;
            for (int i = 2; i < _levelHeight; i += 2)
            {
                numOfEnemies = RandomNumberGenerator.GetInt32(maxNumOfEnemies + 1);
                for (int j = 0; j < numOfEnemies; j++)
                {
                    do
                    {
                        enemyPos = RandomNumberGenerator.GetInt32(_levelWidth);
                    } while (LevelPresented[i][enemyPos] != '.' || LevelPresented[i + 1][enemyPos] == '.' );

                    LevelPresented[i][enemyPos] = 'E';
                }

            }
        }

        private void setPlayerStart()
        {
            int start = RandomNumberGenerator.GetInt32(_levelWidth);
            LevelPresented[0][start] = 'S';
        }

        private void generateHoles()
        {
            int numOfHoles = _levelWidth / 5;
            List<int> holes = new List<int>();
            for (int i = 1; i < _levelHeight - 1; i += 2)
            {
                for (int j = 0; j < numOfHoles; j++)
                {
                    int hole;
                    if (i == 1)
                    {
                        do
                        {
                            hole = RandomNumberGenerator.GetInt32(_levelWidth);
                        } while (LevelPresented[i - 1][hole] == 'S');
                    }
                    else
                    {
                        do
                        {
                            hole = RandomNumberGenerator.GetInt32(_levelWidth);
                        } while (LevelPresented[i - 2][hole] == '.');
                    }

                    LevelPresented[i][hole] = '.';
                }
            }
        }


        private void setFloors()
        {
            for (int i = 1; i < _levelHeight; i += 2)
            {
                for (int j = 0; j < _levelWidth; j++)
                {
                    LevelPresented[i][j] = '#';
                }
            }


        }
        public void printGenerator()
        {
            foreach (List<char> list in LevelPresented)
            {
                foreach (char c in list)
                {
                    System.Console.Write(c);
                }

                System.Console.WriteLine();
            }
        }
        
        
    }
}