// Imported by default
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

// JSON handling
using Newtonsoft.Json;

namespace ConsoleApplication
{
    [Serializable]
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

    public class Snake
    {
        public string playerName;
        public Coordinate head;
        public List<Coordinate> tail;
        public Snake(string name, Coordinate c)
        {
            playerName = name;
            head = c;
            tail = new List<Coordinate>();
        }
    }

    public class GameState
    {
        public List<Snake> snakes;
        public List<Coordinate> food;
        public bool finished = false;

        public GameState(string player1, string player2)
        {
            snakes = new List<Snake>();
            food = new List<Coordinate>();
            snakes.Add(new Snake(player1, new Coordinate(0, 0)));
            snakes.Add(new Snake(player2, new Coordinate(50, 0)));
        }

        public GameState(string JSON)
        {

        }

        public void update()
        {
            if (food.Count == 0)
            {
                spawnFood();
            }
        }

        public void spawnFood()
        {
            Random random = new Random();
            int x = random.Next(0, 100);
            int y = random.Next(0, 100);
            food.Add(new Coordinate(x, y));
        }



        public string ToJSON()
        {
            return JsonConvert.SerializeObject(this);
        }
    }

}
