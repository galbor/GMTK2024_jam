using System;
using System.Collections.Generic;

namespace DefaultNamespace
{
    public class MapMatrix
    {
        private Cell[,] _matrix;
        private Position _playerPosition;
        
        public enum CellType
        {
            Air,
            Wall,
            Door,
            Key,
            Portal,
            Player,
            Rock,
        }

        public struct Cell
        {
            public CellType Type;
            public int LeftX; //default -1
            public int TopY; //default -1
        }

        public struct Position
        {
            public int x;
            public int y;
        }
        
        public MapMatrix(int width, int height)
        {
            _matrix = new Cell[width, height];
            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    _matrix[i, j] = new Cell
                    {
                        Type = CellType.Air,
                        LeftX = -1,
                        TopY = -1
                    };
                }
            }
        }
        
        public Cell[,] Matrix
        {
            get => _matrix;
            private set => _matrix = value;
        }
        
        // public MapMatrix Clone()
        // {
        //     var clone = new MapMatrix(_matrix.GetLength(0), _matrix.GetLength(1));
        //     for (int i = 0; i < _matrix.GetLength(0); i++)
        //     {
        //         for (int j = 0; j < _matrix.GetLength(1); j++)
        //         {
        //             clone._matrix[i, j] = _matrix[i, j];
        //         }
        //     }
        //     return clone;
        // }
        
        public int CellSize(int x, int y)
        {
            Cell cell = _matrix[x, y];
            if (cell.Type == CellType.Wall)
            {
                return 1;
            }
            if (cell.LeftX == -1 || cell.TopY == -1)
            {
                return 1;
            }
            if (x == _matrix.GetLength(0) - 1 || y == _matrix.GetLength(1) - 1)
            {
                return 1;
            }
            if (cell.LeftX != x || cell.TopY != y)
            {
                return 2;
            }

            return _matrix[x + 1, y + 1].TopY == y ? 2 : 1;
        }



        /**
         * @param x, y - coordinates of the cell
         * @param dx, dy - direction of the move
         */
        public bool CanMove(int x, int y, int dx, int dy)
        {
            return CanMove(x, y, dx, dy, CellSize(x, y));
        }

        public bool CanMove(int x, int y, int dx, int dy, int force)
        {
            Cell cell = _matrix[x, y];
            if (cell.Type != CellType.Player && cell.Type != CellType.Rock)
            {
                return false;
            }
            x = cell.LeftX == -1 ? x : cell.LeftX;
            y = cell.TopY == -1 ? y : cell.TopY;
            cell = _matrix[x, y];
            int size = CellSize(x, y);
            
            if (cell.Type == CellType.Rock && force < size)
            {
                return false;
            }

            int newX = x + dx;
            int newY = y + dy;
            
            if (size == 1 && cell.Type == CellType.Player)
            {
                switch (_matrix[newX, newY].Type)
                {
                    case CellType.Air:
                        return true;
                    case CellType.Wall:
                        return false;
                    case CellType.Rock:
                        return CanMove(newX, newY, dx, dy, force);
                    case CellType.Door:
                        // TODO if player has key
                        return false;
                    case CellType.Key:
                        return true;
                    case CellType.Portal:
                        return true;
                    default:
                        return false;
                }
            }

            if (size == 1 && cell.Type == CellType.Rock)
            {
                switch (_matrix[newX, newY].Type)
                {
                    case CellType.Air:
                        return true;
                    case CellType.Wall:
                        return false;
                    case CellType.Rock:
                        return CanMove(newX, newY, dx, dy, force - size);
                    case CellType.Door:
                        return false;
                    case CellType.Key:
                        return false;
                    case CellType.Portal:
                        return false;
                    default:
                        return false;
                }   
            }
            //redundant
            if (size == 1) return false;
            newX = dx == 1 ? newX + 1 : newX;
            newY = dy == 1 ? newY + 1 : newY;
            int newX2 = dx == 0 ? newX + 1 : newX;
            int newY2 = dy == 0 ? newY + 1 : newY;
            if (cell.Type == CellType.Player)
            {
                return CanMove(newX, newY, dx, dy, force) && CanMove(newX2, newY2, dx, dy, force);
                // TODO verify that we aren't moving 2 big rocks
            }
            if (cell.Type == CellType.Rock)
            {
                return CanMove(newX, newY, dx, dy, force - size) && CanMove(newX2, newY2, dx, dy, force - size);
            }

            return false;
        }
        
        public void Move(int x, int y, int dx, int dy)
        {
            Cell cell = _matrix[x, y];
            x = cell.LeftX == -1 ? x : cell.LeftX;
            y = cell.TopY == -1 ? y : cell.TopY;
            cell = _matrix[x, y];
            int size = CellSize(x, y);

            int newX = x + dx;
            int newY = y + dy;
            
            if (cell.Type == CellType.Player)
                _playerPosition = new Position
                {
                    x = newX,
                    y = newY
                };
            
            // TODO what if size = 2?

            if (_matrix[newX, newY].Type == CellType.Rock)
            {
                Move(newX, newY, dx, dy);
            }
            
            _matrix[newX, newY].Type = cell.Type;
            _matrix[newX, newY].LeftX = -1;
            _matrix[newX, newY].TopY = -1;
            
            _matrix[x, y].Type = CellType.Air;
            _matrix[x, y].LeftX = -1;
            _matrix[x, y].TopY = -1;
        }

        public void MovePlayer(int dx, int dy)
        {
            if (!CanMove(_playerPosition.x, _playerPosition.y, dx, dy)) return;
            Move(_playerPosition.x, _playerPosition.y, dx, dy);
        }



        public MapMatrix(string path)
        {
            string[] lines = System.IO.File.ReadAllLines(path);
            int width = lines[0].Length;
            int height = lines.Length;
            _matrix = new Cell[width, height];
            
            for (int i = 0; i < lines.Length; i++)
            {
                string cells = lines[i];
                for (int j = 0; j < cells.Length; j++)
                {
                    _matrix[j,i].LeftX = -1;
                    _matrix[j,i].TopY = -1;
                    switch (cells[j])
                    {
                        case ' ':
                            _matrix[j, i].Type = CellType.Air;
                            break;
                        case 'X':
                            _matrix[j, i].Type = CellType.Wall;
                            break;
                        case 'D':
                            _matrix[j, i].Type = CellType.Door;
                            break;
                        case 'K':
                            _matrix[j, i].Type = CellType.Key;
                            break;
                        case '@':
                            _matrix[j, i].Type = CellType.Portal;
                            break;
                        case 'R':
                        case 'r':
                            _matrix[j, i].Type = CellType.Rock;
                            break;
                        case 'P':
                        case 'p':
                            _matrix[j, i].Type = CellType.Player;
                            //TODO if player is big
                            _playerPosition = new Position
                            {
                                x = j,
                                y = i
                            };
                            break;
                        default:
                            throw new Exception($"Invalid character in map file '{cells[i]}'");
                    }
                }
            }
        }
    }
}