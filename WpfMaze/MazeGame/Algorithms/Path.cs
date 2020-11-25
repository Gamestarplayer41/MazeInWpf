﻿using System.Collections.Generic;
using WpfMaze.MazeGame.Space;

namespace WpfMaze.MazeGame.Algorithms
{
    public class Path
    {
        public List<Direction> Directions = new List<Direction>();

        public void RemoveFirstElement()
        {
            Directions.RemoveAt(0);
        }

        public void AddElement(Direction direction)
        {
            Directions.Add(direction);
        }

        public Direction RemoveLastElement()
        {
            Direction direction = Directions[Directions.Count - 1];
            Directions.RemoveAt(Directions.Count - 1);
            return direction;
        }
    }
}