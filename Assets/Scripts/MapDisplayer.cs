using System;
using UnityEngine;

namespace DefaultNamespace
{
    public class MapDisplayer : Singleton<MapDisplayer>
    {
        [SerializeField] private string _mapPath;
        [SerializeField] private float _spacing = 0.5f;
        [SerializeField] private Vector2 _start;
        
        [SerializeField] private Pool _playerPool;
        [SerializeField] private Pool _rockPool;
        [SerializeField] private Pool _wallPool;
        
        
        private MapMatrix _mapMatrix;
        private void Start()
        {
            _mapMatrix = new MapMatrix("./Assets/Maps/" + _mapPath);
            DisplayMap(_mapMatrix);
        }
        
        private void DisplayMap(MapMatrix mapMatrix)
        {
            var matrix = mapMatrix.Matrix;
            ReturnAllToPools();
            for (int i = 0; i < matrix.GetLength(0); i++)
            {
                for (int j = 0; j < matrix.GetLength(1); j++)
                {
                    var cell = matrix[i, j];
                    Vector3 position = _start - new Vector2(-i * _spacing, j * _spacing);
                    switch (cell.Type)
                    {
                        case MapMatrix.CellType.Wall:
                            _wallPool.Get(position, _wallPool.transform);
                            break;
                        case MapMatrix.CellType.Player:
                            _playerPool.Get(position, _playerPool.transform);
                            break;
                        case MapMatrix.CellType.Rock:
                            _rockPool.Get(position, _rockPool.transform);
                            break;
                        default:
                            break;
                    }
                }
            }
        }

        private void ReturnAllToPools()
        {
            for (int i = 0; i<_wallPool.transform.childCount; i++)
            {
                _wallPool.Return(_wallPool.transform.GetChild(i).gameObject);
            }
            for (int i = 0; i<_playerPool.transform.childCount; i++)
            {
                _playerPool.Return(_playerPool.transform.GetChild(i).gameObject);
            }
            for (int i = 0; i<_rockPool.transform.childCount; i++)
            {
                _rockPool.Return(_rockPool.transform.GetChild(i).gameObject);
            }
        }

        public void MovePlayer(int dx, int dy)
        {
            _mapMatrix.MovePlayer(dx, dy);
            DisplayMap(_mapMatrix);
        }
    }
}