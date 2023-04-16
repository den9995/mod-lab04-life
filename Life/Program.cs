using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Threading;
using System.IO;

namespace cli_life
{
    public class Figure
    {
        public int m { get; set; }
        public int n { get; set; }
        public bool[,] table { get; set; } 
    }
    public class Config
    {
        public int width { get; set; }
        public int height { get; set; }
        public int cellSize { get; set; }
        public double density { get; set; }
    }

    public class Cell
    {
        public bool IsAlive;
        public readonly List<Cell> neighbors = new List<Cell>();
        private bool IsAliveNext;
        public void DetermineNextLiveState()
        {
            int liveNeighbors = neighbors.Where(x => x.IsAlive).Count();
            if (IsAlive)
                IsAliveNext = liveNeighbors == 2 || liveNeighbors == 3;
            else
                IsAliveNext = liveNeighbors == 3;
        }
        public void Advance()
        {
            IsAlive = IsAliveNext;
        }
    }
    public class Board
    {
        public readonly Cell[,] Cells;
        public readonly int CellSize;
        public double liveDensity;

        public int Columns { get { return Cells.GetLength(0); } }
        public int Rows { get { return Cells.GetLength(1); } }
        public int Width { get { return Columns * CellSize; } }
        public int Height { get { return Rows * CellSize; } }
        public double LiveDensity { get { return liveDensity; } }

        public Board(int width, int height, int cellSize, double liveDensity = .1)
        {
            CellSize = cellSize;

            Cells = new Cell[width / cellSize, height / cellSize];
            for (int x = 0; x < Columns; x++)
                for (int y = 0; y < Rows; y++)
                    Cells[x, y] = new Cell();

            ConnectNeighbors();
            Randomize(liveDensity);
        }

        readonly Random rand = new Random();
        public void Randomize(double liveDensity)
        {
            foreach (var cell in Cells)
                cell.IsAlive = rand.NextDouble() < liveDensity;
        }

        public void Advance()
        {
            foreach (var cell in Cells)
                cell.DetermineNextLiveState();
            foreach (var cell in Cells)
                cell.Advance();
        }
        private void ConnectNeighbors()
        {
            for (int x = 0; x < Columns; x++)
            {
                for (int y = 0; y < Rows; y++)
                {
                    int xL = (x > 0) ? x - 1 : Columns - 1;
                    int xR = (x < Columns - 1) ? x + 1 : 0;

                    int yT = (y > 0) ? y - 1 : Rows - 1;
                    int yB = (y < Rows - 1) ? y + 1 : 0;

                    Cells[x, y].neighbors.Add(Cells[xL, yT]);
                    Cells[x, y].neighbors.Add(Cells[x, yT]);
                    Cells[x, y].neighbors.Add(Cells[xR, yT]);
                    Cells[x, y].neighbors.Add(Cells[xL, y]);
                    Cells[x, y].neighbors.Add(Cells[xR, y]);
                    Cells[x, y].neighbors.Add(Cells[xL, yB]);
                    Cells[x, y].neighbors.Add(Cells[x, yB]);
                    Cells[x, y].neighbors.Add(Cells[xR, yB]);
                }
            }
        }
    }
    public class Program
    {
        static public Board board;
        static public void Reset()
        {
            string json = File.ReadAllText(@"./config.json");
            Config data = JsonSerializer.Deserialize<Config>(json);
            int width = data.width;
            int height = data.height;
            int cellSize = data.cellSize;
            double density = data.density;
            board = new Board(width: width, height: height, cellSize: cellSize,liveDensity: density);
        }
        static void Render()
        {
            for (int row = 0; row < board.Rows; row++)
            {
                for (int col = 0; col < board.Columns; col++)   
                {
                    var cell = board.Cells[col, row];
                    if (cell.IsAlive)
                    {
                        Console.Write('*');
                    }
                    else
                    {
                        Console.Write(' ');
                    }
                }
                Console.Write('\n');
            }
        }
        static void Main(string[] args)
        {
            Reset();
            if(new FileInfo("out.txt").Exists)
                load("out.txt");
            while(true)
            {
                Console.Clear();
                Render();
                save();
                Console.WriteLine("blocks:"+find("block"));
                Console.WriteLine("boxes:"+find("box"));
                Console.WriteLine("horizontal symetry:"+symmetry_horizontally());
                Console.WriteLine("vertical symetry:"+symmetry_vertically());
                board.Advance();
                Thread.Sleep(10);
            }
        }
        public static void save()
        {
            string fileName = "out.txt";
            string str = "";
            StreamWriter f = new StreamWriter(fileName, false);
            f.WriteLine(board.Columns.ToString());
            f.WriteLine(board.Rows.ToString());
            for (int row = 0; row < board.Rows; row++)
            {
                str = "";
                for (int col = 0; col < board.Columns; col++)
                {
                    if (board.Cells[col, row].IsAlive == true)
                    {
                        str = str + "*";
                    }
                    else
                    {
                        str = str + "_";
                    }
                }
                f.WriteLine(str);
            }
            f.Close();
        }
        public static void load(string str)
        {
            StreamReader f = new StreamReader(str);
            int width = Int32.Parse(f.ReadLine());
            int height = Int32.Parse(f.ReadLine());
            board = new Board(width: width, height: height, cellSize: 1, liveDensity: 0.5 );
            int j = 0;
                while (!f.EndOfStream)
                {
                    string s = f.ReadLine();
                    for (int i = 0; i < width; i++)
                    {
                        if (s[i] == '*')
                        board.Cells[i,j].IsAlive = true;
                        else
                        board.Cells[i, j].IsAlive = false;
                    }
                    j++;
                }
            f.Close();
        }
        public static bool symmetry_horizontally()
        {

            bool flag = true;
            for (int i=0; i<board.Columns; i++)
            {
                for (int j = 0; j < board.Rows/2; j++)
                {
                    if (board.Cells[i, j].IsAlive != board.Cells[i,board.Rows-1-j].IsAlive)
                        flag = false;   
                }
            }
            return flag;
        }
        public static bool symmetry_vertically()
        {
            bool flag = true;
            for (int i = 0; i < board.Rows; i++)
            {
                for (int j = 0; j < board.Columns / 2; j++)
                {
                    if (board.Cells[j, i].IsAlive != board.Cells[board.Columns - 1 -j,  i].IsAlive)
                        flag = false;
                }
            }
            return flag;
        }
        public static int find(string name)
        {
            StreamReader f = new StreamReader(name+".txt");
            int width = Int32.Parse(f.ReadLine());
            int height = Int32.Parse(f.ReadLine());
            bool [,] table = new bool[width,height];
            int j = 0;
            while (!f.EndOfStream)
            {
                string s = f.ReadLine();
                for (int i = 0; i < width; i++)
                {
                    if (s[i] == '*')
                    table[i,j] = true;
                    else
                    table[i,j] = false;
                }
                j++;
            }
            f.Close();
            
            j = 0;
            int rowIndexStart = 0;
            int rowIndexFinish = width;
            int colIndexStart = 0;
            int colIndexFinish = height;
            bool flag = true;
            int sum = 0;
            while (rowIndexFinish < board.Columns)
            {
                while (colIndexFinish < board.Rows)
                {
                    int i = 0;
                    j = 0;
                    for (int row = rowIndexStart; row < rowIndexFinish; row++)
                    {
                        for (int col = colIndexStart; col < colIndexFinish; col++)
                        {
                            if (board.Cells[row, col].IsAlive != table[i,j])
                            {
                                flag = false;
                                break;
                            }
                            j++;
                        }
                        j = 0;
                        i++;
                    }
                    i = 0;
                    colIndexStart++;
                    colIndexFinish++;
                    if (flag) sum++;
                    flag = true;
                }
                colIndexFinish = height;
                colIndexStart = 0;
                rowIndexStart++;
                rowIndexFinish++;
            }
            return sum;
        }
    }
}
