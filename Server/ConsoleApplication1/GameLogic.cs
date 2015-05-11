using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApplication
{
    public class Coordinate
    {
        public int x;
        public int y;
        public Coordinate(int x_coordinate, int y_coordinate)
        {
            x = x_coordinate;
            y = y_coordinate;
        }
    }

    class Snake
    {
        string playerName;
        Coordinate head;
        List<Coordinate> tail;
        bool alive = true;
        public Snake(string name, Coordinate c)
        {
            playerName = name;
            head = c;
            tail = new List<Coordinate>();
        }
    }

    public class GameState
    {
        List<Snake> snakes;
        List<Coordinate> food;

        public GameState()
        {
        }

        public GameState(string JSON)
        {
        }
    }

    class GameLogic
    {
    }
    
}
