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

    public class PlayerSnake
    {
        public string playerName;
        public Coordinate head;
        public List<Coordinate> tail;
        public string direction;
        public PlayerSnake(string name, Coordinate c, string d)
        {
            playerName = name;
            head = c;
            tail = new List<Coordinate>();
            direction = d;
        }
    }

    public class GameState
    {
        public List<PlayerSnake> snakes;
        public List<Coordinate> food;
        public bool finished = false;
        public string currentPlayer;

        public GameState(string player1, string player2)
        {
            snakes = new List<PlayerSnake>();
            food = new List<Coordinate>();
            food.Add(new Coordinate(0, 0));
            snakes.Add(new PlayerSnake(player1, new Coordinate(-20, 0), "r"));
            snakes.Add(new PlayerSnake(player2, new Coordinate(20, 0), "l"));
        }

        public GameState()
        {
        }

        public void update(string JSON)
        {
            Console.WriteLine(JSON);
            GameState newgame = JsonConvert.DeserializeObject<GameState>(JSON);
            PlayerSnake SnakeToChange = new PlayerSnake("", new Coordinate(0,0), "");
            for (int i = 0; i < newgame.snakes.Count; i++)
            {
                if (newgame.snakes[i].playerName == newgame.currentPlayer)
                {
                    SnakeToChange = newgame.snakes[i];
                }
            }
            for (int i = 0; i < snakes.Count; i++)
            {
                if (snakes[i].playerName == SnakeToChange.playerName)
                {
                    snakes[i].head = SnakeToChange.head;
                    snakes[i].tail = SnakeToChange.tail;
                }
            }
            if (newgame.finished)
            {
                finished = true;
            }
        }

        public void mainLoop()
        {
            if (!finished)
            {
                if (food.Count == 0)
                {
                    spawnFood();
                }
            }
        }


        public void updateSnake(string player, string snakeInfo)
        {
            for (int i = 0; i < snakes.Count; i++)
            {
                if (snakes[i].playerName == player)
                {
                    // update the snake stuff
                }
            }
        }

        public void spawnFood()
        {
            Random random = new Random();
            int x = random.Next(-34, 34);
            int y = random.Next(-24, 24);
            food.Add(new Coordinate(x, y));
        }



        public string ToJSON()
        {
            return JsonConvert.SerializeObject(this);
        }
    }

}
