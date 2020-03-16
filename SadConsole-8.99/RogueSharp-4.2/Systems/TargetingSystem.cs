using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework.Input;
using RogueSharp;
using RogueSharpSample2.Core;
using RogueSharpSample2.Interfaces;
using SadConsole;

namespace RogueSharpSample2.Systems
{
    public class TargetingSystem
    {
        private int _area;
        private int _currentTargetIndex;

        private Point _cursorPosition;
        private List<Point> _selectableTargets = new List<Point>();
        private SelectionType _selectionType;
        private ITargetable _targetable;

        public bool IsPlayerTargeting { get; private set; }

        public bool SelectMonster(ITargetable targetable)
        {
            Initialize();
            _selectionType = SelectionType.Target;
            var map = RogueGame.DungeonMap;
            _selectableTargets = map.GetMonsterLocationsInFieldOfView().ToList();
            _targetable = targetable;
            _cursorPosition = _selectableTargets.FirstOrDefault();
            if (_cursorPosition == Point.Zero)
            {
                StopTargeting();
                return false;
            }

            IsPlayerTargeting = true;
            return true;
        }

        public bool SelectArea(ITargetable targetable, int area = 0)
        {
            Initialize();
            _selectionType = SelectionType.Area;
            var player = RogueGame.Player;
            _cursorPosition = new Point {X = player.X, Y = player.Y};
            _targetable = targetable;
            _area = area;

            IsPlayerTargeting = true;
            return true;
        }

        public bool SelectLine(ITargetable targetable)
        {
            Initialize();
            _selectionType = SelectionType.Line;
            var player = RogueGame.Player;
            _cursorPosition = new Point {X = player.X, Y = player.Y};
            _targetable = targetable;

            IsPlayerTargeting = true;
            return true;
        }

        private void StopTargeting()
        {
            IsPlayerTargeting = false;
            Initialize();
        }

        private void Initialize()
        {
            _cursorPosition = Point.Zero;
            _selectableTargets = new List<Point>();
            _currentTargetIndex = 0;
            _area = 0;
            _targetable = null;
            _selectionType = SelectionType.None;
        }

        public bool HandleInput(InputState inputState)
        {
            if (_selectionType == SelectionType.Target)
            {
                HandleSelectableTargeting(inputState);
            }
            else if (_selectionType == SelectionType.Area)
            {
                HandleLocationTargeting(inputState);
            }
            else if (_selectionType == SelectionType.Line)
            {
                HandleLocationTargeting(inputState);
            }

            if (inputState.IsKeyPressed(Keys.Enter))
            {
                _targetable.SelectTarget(_cursorPosition);
                StopTargeting();
                return true;
            }

            return false;
        }

        private void HandleSelectableTargeting(InputState inputState)
        {
            if (inputState.IsKeyPressed(Keys.Right) || inputState.IsKeyPressed(Keys.Down))
            {
                _currentTargetIndex++;
                if (_currentTargetIndex >= _selectableTargets.Count)
                {
                    _currentTargetIndex = 0;
                }

                _cursorPosition = _selectableTargets[_currentTargetIndex];
            }
            else if (inputState.IsKeyPressed(Keys.Left) || inputState.IsKeyPressed(Keys.Up))
            {
                _currentTargetIndex--;
                if (_currentTargetIndex < 0)
                {
                    _currentTargetIndex = _selectableTargets.Count - 1;
                }

                _cursorPosition = _selectableTargets[_currentTargetIndex];
            }
        }

        private void HandleLocationTargeting(InputState inputState)
        {
            var x = _cursorPosition.X;
            var y = _cursorPosition.Y;
            var map = RogueGame.DungeonMap;

            if (inputState.IsKeyPressed(Keys.Right))
            {
                x++;
            }
            else if (inputState.IsKeyPressed(Keys.Left))
            {
                x--;
            }
            else if (inputState.IsKeyPressed(Keys.Up))
            {
                y--;
            }
            else if (inputState.IsKeyPressed(Keys.Down))
            {
                y++;
            }

            if (map.IsInFov(x, y))
            {
                _cursorPosition.X = x;
                _cursorPosition.Y = y;
            }
        }

        public void Draw(Console mapConsole)
        {
            if (IsPlayerTargeting)
            {
                var map = RogueGame.DungeonMap;
                var player = RogueGame.Player;
                if (_selectionType == SelectionType.Area)
                {
                    foreach (var cell in map.GetCellsInCircle(_cursorPosition.X, _cursorPosition.Y, _area))
                    {
                        mapConsole.SetBackground(cell.X, cell.Y, Swatch.DbSun);
                    }
                }
                else if (_selectionType == SelectionType.Line)
                {
                    foreach (var cell in map.GetCellsAlongLine(player.X, player.Y, _cursorPosition.X,
                        _cursorPosition.Y))
                    {
                        mapConsole.SetBackground(cell.X, cell.Y, Swatch.DbSun);
                    }
                }

                mapConsole.SetBackground(_cursorPosition.X, _cursorPosition.Y, Swatch.DbLight);
            }
        }

        private enum SelectionType
        {
            None = 0,
            Target = 1,
            Area = 2,
            Line = 3
        }
    }
}
