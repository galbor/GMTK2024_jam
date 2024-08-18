using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

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
            End
        }

        public bool MayEnter(CellType origin, CellType target)
        {
            if (origin == CellType.Player)
            {
                if (target == CellType.Air || target == CellType.Key)
                    return true;
                return false;
            }
            return origin == CellType.Rock && target == CellType.Air;
        }

        public struct Cell
        {
            public CellType Type;
            public Position TopLeft;

            public Cell(CellType type)
            {
                Type = type;
                TopLeft = Position.Empty();
            }
            public Cell(CellType type, Position topLeft)
            {
                Type = type;
                TopLeft = topLeft;
            }
        }

        public struct Position
        {
            public int x;
            public int y;

            public Position(int x, int y)
            {
                this.x = x;
                this.y = y;
            }
            public Position Shift(int dx, int dy)
            {
                return new Position { x = x + dx, y = y + dy };
            }

            public bool IsMinus1()
            {
                return x == -1 || y == -1;
            }

            public static Position Empty()
            {
                return new Position { x = -1, y = -1 };
            }

            bool Equals(Position pos)
            {
                return pos.x == this.x && pos.y == this.y;
            }
        }

        public ref Cell CellAt(Position pos)
        {
            return ref _matrix[pos.x, pos.y];
        }

        public Position Origin(Position pos)
        {
            var cell = CellAt(pos);
            if (cell.TopLeft.IsMinus1())
                return pos;
            return cell.TopLeft;
        }
        
        public MapMatrix(int width, int height)
        {
            _matrix = new Cell[width, height];
            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    _matrix[i, j] = new Cell(CellType.Air);
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
        
        public int CellSize(Position pos)
        {
            Cell cell = CellAt(Origin(pos));
            if (cell.Type == CellType.Wall)
            {
                return 1;
            }
            if (cell.TopLeft.IsMinus1())
            {
                return 1;
            }
            if (pos.x == _matrix.GetLength(0) - 1 || pos.y == _matrix.GetLength(1) - 1)
            {
                return 1;
            }

            return CellAt(pos.Shift(1, 1)).TopLeft.y == pos.y &&
                   CellAt(pos.Shift(1, 1)).TopLeft.x == pos.x ? 2 : 1;
        }



        /**
         * @param x, y - coordinates of the cell
         * @param dx, dy - direction of the move
         */
        public bool CanMove(Position pos, int dx, int dy)
        {
            return CanMove(pos, dx, dy, CellSize(pos));
        }

        public bool CanMove(Position pos, int dx, int dy, int force)
        {
            pos = Origin(pos);
            Cell cell = CellAt(pos);
            CellType moverCellType = cell.Type;
            if (moverCellType == CellType.Air)
            {
                return true;
            }
            if (moverCellType != CellType.Player && moverCellType != CellType.Rock)
            {
                return false;
            }
            int size = CellSize(pos);
            
            if (moverCellType == CellType.Rock && force < size)
            {
                return false;
            }

            Position newPos = pos.Shift(dx, dy);
            if (size == 1)
            {
                if (CellAt(newPos).Type == CellType.Portal)
                {
                    return CanTeleport(moverCellType, Origin(newPos).Shift(2*dx, 2*dy), 2);
                }
                else if (moverCellType == CellType.Player)
                {
                    if (CellAt(newPos).Type == CellType.Rock)
                    {
                        return CanMove(newPos, dx, dy, force);
                    }
                    return MayEnter(moverCellType, CellAt(newPos).Type);
                }

                else if (moverCellType == CellType.Rock)
                {
                    if (CellAt(newPos).Type == CellType.Rock)
                    {
                        return CanMove(newPos, dx, dy, force - size);
                    }
                    return MayEnter(moverCellType, CellAt(newPos).Type);
                }
                else
                {
                    //something wrong if gets here
                    Debug.Log("Someone is trying to move something that is not a player or a rock.");
                    return false;
                }
            }

            var (edgePos1, edgePos2) = EdgePositions(pos, dx, dy);
            
            //needs both edges to be portal if one of them is portal
            //in which case teleports to one of the locations
            if (CellAt(edgePos1).Type == CellType.Portal || CellAt(edgePos2).Type == CellType.Portal)
            {
                if (CellAt(edgePos1).Type != CellType.Portal || CellAt(edgePos2).Type != CellType.Portal)
                    return false;
                //edge positions of portal
                (edgePos1, edgePos2) = EdgePositions(Origin(edgePos1), dx, dy);
                return CanTeleport(moverCellType, edgePos1, 1) || CanTeleport(moverCellType, edgePos2, 1);
            }
            if (moverCellType == CellType.Player)
            {
                return CanMove(edgePos1, dx, dy, force) && CanMove(edgePos2, dx, dy, force);
                // TODO verify that we aren't moving 2 big rocks
            }
            if (moverCellType == CellType.Rock)
            {
                return CanMove(edgePos1, dx, dy, force - size) && CanMove(edgePos2, dx, dy, force - size);
            }

            return false;
        }

        private (Position edgePos1, Position edgePos2) EdgePositions(Position TopLeft, int dx, int dy)
        {
            dx = dx > 0 ? 2 : dx;
            dy = dy > 0 ? 2 : dy;
            int newX = TopLeft.x + dx;
            int newY = TopLeft.y + dy;
            Position edgePos1 = new Position {x = newX, y = newY};
            int newX2 = dx == 0 ? newX + 1 : newX;
            int newY2 = dy == 0 ? newY + 1 : newY;
            Position edgePos2 = new Position {x = newX2, y = newY2};
            return (edgePos1, edgePos2);
        }

        private bool CanTeleport(CellType moverCellType, Position newPos, int size)
        {
            if (moverCellType != CellType.Player && moverCellType != CellType.Rock)
            {
                return false;
            }
            for (int i = 0; i < size; i++)
            {
                for (int j = 0; j < size; j++)
                {
                    if (!MayEnter(moverCellType, CellAt(newPos.Shift(i, j)).Type))
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        private void Teleport(Position pos,Position portalPos , int dx, int dy)
        {
            pos = Origin(pos);
            portalPos = Origin(portalPos);
            CellType moverCellType = CellAt(pos).Type;
            int newSize = 3- CellSize(pos); //2->1, 1->2
            Position[] newPos = new Position[2];
            if (newSize == 1)
            {
                (newPos[0], newPos[1]) = EdgePositions(portalPos, dx, dy);
                for (int i = 0; i < newPos.Length; i++)
                {
                    if (!MayEnter(moverCellType, CellAt(newPos[i]).Type))
                        continue;
                    ref Cell newCell = ref CellAt(newPos[i]);
                    newCell.Type = CellAt(pos).Type;
                    newCell.TopLeft = newPos[i];
                    EraseSquare(pos);
                    SetPlayerPosition(newPos[i]);
                    return;
                }
            }
            //else newSize == 2
            dx *= 2;
            dy *= 2;
            newPos[0] = portalPos.Shift(dx, dy);
            for (int i = 0; i < 2; i++)
            {
                for (int j = 0; j < 2; j++)
                {
                    ref Cell newCell = ref CellAt(newPos[0].Shift(i,j));
                    newCell.Type = moverCellType;
                    newCell.TopLeft = newPos[0];
                }
            }
            SetPlayerPosition(newPos[0]);

            ref Cell originalCell = ref CellAt(pos);
            originalCell.Type = CellType.Air;
            return;
        }

        private void SetPlayerPosition(Position pos)
        {
            if (CellAt(pos).Type == CellType.Player)
            {
                _playerPosition = pos;
            }
        }

        public void Move(Position pos, int dx, int dy)
        {
            if (CellAt(pos).Type == CellType.Air) return;
            pos = Origin(pos);
            ref Cell cell = ref CellAt(pos);
            int size = CellSize(pos);

            Position newPos = pos.Shift(dx, dy);

            if (size == 1)
            {
                if (CellAt(newPos).Type == CellType.Portal)
                {
                    Teleport(pos, newPos, dx, dy);
                    return;
                }

                if (CellAt(newPos).Type == CellType.Rock)
                {
                    Move(newPos, dx, dy);
                }

                ref Cell newCell = ref CellAt(newPos);
                newCell.Type = cell.Type;
                newCell.TopLeft = Position.Empty();
                SetPlayerPosition(newPos);
                

                cell.Type = CellType.Air;
                cell.TopLeft = Position.Empty();
                return;
            }

            var (edgePos1, edgePos2) = EdgePositions(pos, dx, dy);
            if (CellAt(edgePos1).Type == CellType.Portal)
            {
                Teleport(pos, edgePos1, dx, dy);
                return;
            }
            
            
            if (CellAt(edgePos1).Type == CellType.Rock)
                Move(edgePos1, dx, dy);
            if (CellAt(edgePos2).Type == CellType.Rock)
                Move(edgePos2, dx, dy);

            CellType type = cell.Type;
            EraseSquare(pos);
            for (int i = 0; i < 2; i++)
            {
                for (int j = 0; j < 2; j++)
                {
                    ref Cell newCell = ref CellAt(newPos.Shift(i,j));
                    newCell.Type = type;
                    newCell.TopLeft = newPos;
                }
            }
            SetPlayerPosition(newPos);
        }

        //erase 2x2 square
        private void EraseSquare(Position TopLeft)
        {
            for (int i = 0; i < 2; i++)
            {
                for (int j = 0; j < 2; j++)
                {
                    ref Cell cell = ref CellAt(TopLeft.Shift(i, j));
                    cell.Type = CellType.Air;
                    cell.TopLeft = Position.Empty();
                }
            }
        }

        public void MovePlayer(int dx, int dy)
        {
            if (!CanMove(_playerPosition, dx, dy)) return;
            Move(_playerPosition, dx, dy);
        }



        public MapMatrix(string path)
        {
            string[] lines = System.IO.File.ReadAllLines(path);
            int width = lines[0].Length;
            int height = lines.Length;
            _matrix = new Cell[width, height];
            
            for (int j = lines.Length-1; j >= 0; j--)
            {
                string cells = lines[j];
                for (int i = cells.Length-1; i >=0; i--)
                {
                    switch (cells[i])
                    {
                        case ' ':
                            _matrix[i, j] = new Cell(CellType.Air);
                            break;
                        case 'X':
                            _matrix[i, j] = new Cell(CellType.Wall);
                            break;
                        case 'D':
                            _matrix[i, j] = new Cell(CellType.Door);
                            break;
                        case 'K':
                            _matrix[i, j] = new Cell(CellType.Key);
                            break;
                        case '#':
                            _matrix[i, j] = new Cell(CellType.Portal);
                            SetTopLeft2x2(i,j);
                            break;
                        case '@':
                            _matrix[i, j] = new Cell(CellType.Portal);
                            break;
                        case 'R':
                            _matrix[i, j] = new Cell( CellType.Rock);
                            SetTopLeft2x2(i,j);
                            break;
                        case 'r':
                            _matrix[i, j] = new Cell( CellType.Rock);
                            break;
                        case 'P':
                            _matrix[i, j] = new Cell(CellType.Player);
                            SetTopLeft2x2(i,j);
                            _playerPosition = _matrix[i, j].TopLeft;
                            break;
                        case 'p':
                            _matrix[i, j] = new Cell( CellType.Player);
                            break;
                        default:
                            throw new Exception($"Invalid character in map file '{cells[j]}'");
                    }
                }
            }

            // Print();
        }

        private void SetTopLeft2x2(int row, int col)
        {
            _matrix[row, col].TopLeft = new Position(row, col);
            for (int a = 0; a < 2; a++)
            {
                for (int b = 0; b < 2; b++)
                {
                    _matrix[row + a, col + b].TopLeft = _matrix[row, col].TopLeft;
                }
            }
        }

        private void Print()
        {
            Debug.Log("Printing map matrix:");
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i< _matrix.GetLength(1); i++)
            {
                for (int j = 0; j < _matrix.GetLength(0); j++)
                {
                    sb.Append(_matrix[j, i].Type switch
                    {
                        CellType.Air => ' ',
                        CellType.Wall => 'X',
                        CellType.Door => 'D',
                        CellType.Key => 'K',
                        CellType.Portal => '@',
                        CellType.Player => 'P',
                        CellType.Rock => 'R',
                        _ => throw new Exception("Invalid cell type")
                    });
                }

                sb.Append("\n");
            }
            Debug.Log(sb.ToString());
        }
    }
}