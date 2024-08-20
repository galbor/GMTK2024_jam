using System;
using System.Drawing;
using UnityEngine;
using UnityEngine.UIElements;

namespace DefaultNamespace
{
    public class MapDisplayer : Singleton<MapDisplayer>
    {
        [SerializeField] private string _mapPath;
        [SerializeField] private float _spacing = 0.5f;
        private float _scale = 1f;
        private Vector2 _start;
        private float _realSpacing;

        private const float _height = 5f;
        private const float _width = 7f;
        
        [SerializeField] private Pool _playerPool;
        [SerializeField] private Pool _rockPool;
        [SerializeField] private Pool _wallPool;
        [SerializeField] private Pool _airPool;
        [SerializeField] private Pool _portalPool;
        [SerializeField] private Pool _doorPool;
        [SerializeField] private Pool _keyPool;
        [SerializeField] private Pool _finishPool;
        
        
        
        private MapMatrix _mapMatrix;
        
        public void SetMap(string mapPath)
        {
            _mapPath = mapPath;
            ResetMap();
        }

        public void ResetMap()
        {
            _mapMatrix = new MapMatrix("./Assets/Maps/" + _mapPath);
            ChooseScale();
            _realSpacing = _spacing * _scale;
            _start = new Vector2(-_mapMatrix.Matrix.GetLength(0) * _realSpacing / 2, _mapMatrix.Matrix.GetLength(1) * _realSpacing / 2);
            DisplayBackground(_mapMatrix);
            DisplayMapObjects(_mapMatrix);
        }

        private void ChooseScale()
        {
            float heightscale;
            float widthscale;
            heightscale = _height/(_spacing * _mapMatrix.Matrix.GetLength(1));
            widthscale = _width/(_spacing * _mapMatrix.Matrix.GetLength(0));

            _scale = 2* Math.Min(heightscale, widthscale);
        }

        private void DisplayBackground(MapMatrix mapMatrix)
        {
            _wallPool.ReturnAll();
            _airPool.ReturnAll();
            _finishPool.ReturnAll();
            var matrix = mapMatrix.Matrix;
            for (int i = 0; i < matrix.GetLength(0); i++)
            {
                for (int j = 0; j < matrix.GetLength(1); j++)
                {
                    switch (matrix[i, j].Type)
                    {
                        case MapMatrix.CellType.Wall:
                            spawnObject(_wallPool, i, j);
                            break;
                        case MapMatrix.CellType.End:
                            spawnObject(_finishPool, i, j);
                            break;
                        default:
                            spawnObject(_airPool, i, j, 1);
                            break;
                    }
                }
            }
        }

        private void DisplayMapObjects(MapMatrix mapMatrix)
        {
            GameObject obj = this.gameObject;
            var matrix = mapMatrix.Matrix;
            _playerPool.ReturnAll();
            _rockPool.ReturnAll();
            _portalPool.ReturnAll();
            _keyPool.ReturnAll();
            _doorPool.ReturnAll();
            for (int i = 0; i < matrix.GetLength(0); i++)
            {
                for (int j = 0; j < matrix.GetLength(1); j++)
                {
                    switch (matrix[i,j].Type)
                    {
                        case MapMatrix.CellType.Player:
                            spawnObject(_playerPool, i, j);
                            break;
                        case MapMatrix.CellType.Rock:
                            spawnObject(_rockPool, i, j);
                            break;
                        case MapMatrix.CellType.Portal:
                            spawnObject(_portalPool, i, j);
                            break;
                        case MapMatrix.CellType.Door:
                            spawnObject(_doorPool, i, j);
                            break;
                        case MapMatrix.CellType.Key:
                            spawnObject(_keyPool, i, j);
                            break;
                        default:
                            break;
                    }
                }
            }
        }
        
        private void spawnObject(Pool pool, int row, int col, int size=-1)
        {
            MapMatrix.Position pos = new MapMatrix.Position(row, col);
            var cell = _mapMatrix.CellAt(pos);
            size = size == -1 ? _mapMatrix.CellSize(pos) : size;
            // if (!cell.TopLeft.Equals(pos) &&
            //     !cell.TopLeft.Equals(MapMatrix.Position.Empty()) && size == 2)
            if (size == 2 && !cell.TopLeft.Equals(pos))
                return;
            Vector3 coords = _start + new Vector2(row * _realSpacing, -col * _realSpacing);
            GameObject obj = pool.Get(coords, _scale);
            if (size == 2)
            {
                obj.transform.localScale *= 2;
                obj.transform.position += new Vector3(_realSpacing / 2, -_realSpacing / 2);
            }
        }


        public void MovePlayer(int dx, int dy)
        {
            _mapMatrix.MovePlayer(dx, dy);
            DisplayMapObjects(_mapMatrix);
        }
    }
}