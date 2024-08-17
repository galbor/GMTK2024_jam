using System;
using UnityEngine;

namespace DefaultNamespace
{
    public class MapDisplayer : Singleton<MapDisplayer>
    {
        [SerializeField] private string _mapPath;
        [SerializeField] private float _spacing = 0.5f;
        [SerializeField] private float _scale = 1f;
        private Vector2 _start;
        private float _realSpacing;
        
        [SerializeField] private Pool _playerPool;
        [SerializeField] private Pool _rockPool;
        [SerializeField] private Pool _wallPool;
        
        
        private MapMatrix _mapMatrix;
        private void Start()
        {
            _realSpacing = _spacing * _scale;
            ResetMap();
        }

        public void ResetMap()
        {
            _mapMatrix = new MapMatrix("./Assets/Maps/" + _mapPath);
            _start = new Vector2(-_mapMatrix.Matrix.GetLength(0) * _realSpacing / 2, _mapMatrix.Matrix.GetLength(1) * _realSpacing / 2);
            DisplayBackground(_mapMatrix);
            DisplayMapObjects(_mapMatrix);
        }

        private void DisplayBackground(MapMatrix mapMatrix)
        {
            ReturnAllToPool(_wallPool);
            var matrix = mapMatrix.Matrix;
            //same for airpool
            for (int i = 0; i < matrix.GetLength(0); i++)
            {
                for (int j = 0; j < matrix.GetLength(1); j++)
                {
                    Vector3 position = _start + new Vector2(i * _realSpacing, -j * _realSpacing);
                    switch (matrix[i, j].Type)
                    {
                        case MapMatrix.CellType.Wall:
                            _wallPool.Get(position, _scale);
                            break;
                        default:
                            //airpool.get
                            break;
                    }
                }
            }
        }
        
        private void DisplayMapObjects(MapMatrix mapMatrix)
        {
            GameObject obj = this.gameObject;
            var matrix = mapMatrix.Matrix;
            ReturnAllToPool(_playerPool);
            ReturnAllToPool(_rockPool);
            for (int i = 0; i <matrix.GetLength(0) ; i++)
            {
                for (int j = 0; j < matrix.GetLength(1); j++)
                {
                    var cell = matrix[i, j];
                    Vector3 position = _start + new Vector2(i * _realSpacing, -j * _realSpacing);
                    if (!cell.TopLeft.Equals(new MapMatrix.Position(i,j)) && !cell.TopLeft.Equals(MapMatrix.Position.Empty()))
                        continue;
                    switch (cell.Type)
                    {
                        case MapMatrix.CellType.Player:
                            obj = _playerPool.Get(position, _scale);
                            break;
                        case MapMatrix.CellType.Rock:
                            obj = _rockPool.Get(position, _scale);
                            break;
                        default:
                            break;
                    }
                    if (_mapMatrix.CellSize(new MapMatrix.Position(i, j)) == 2)
                    {
                        obj.transform.localScale *= 2;
                        obj.transform.position += new Vector3(_realSpacing / 2, -_realSpacing / 2);
                    }
                }
            }
        }

        private void ReturnAllToPools()
        {
            ReturnAllToPool(_rockPool);
            ReturnAllToPool(_wallPool);
            ReturnAllToPool(_playerPool);
        }

        private void ReturnAllToPool(Pool pool)
        {
            for (int i = 0; i<pool.transform.childCount; i++)
            {
                pool.Return(pool.transform.GetChild(i).gameObject);
            }
        }

        public void MovePlayer(int dx, int dy)
        {
            _mapMatrix.MovePlayer(dx, dy);
            DisplayMapObjects(_mapMatrix);
        }
    }
}