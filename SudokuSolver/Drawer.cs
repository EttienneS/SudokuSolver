using System;

namespace SudokuSolver
{
    public static class Drawer
    {
        public static void DrawGrid(Grid grid)
        {
            var count = 0;
            var output = "\n\t";
            foreach (var cell in grid.Cells)
            {
                var value = "_";
                if (cell.Value.HasValue)
                {
                    value = cell.Value.Value.ToString();
                }

                output += value + " ";
                count++;

                if (count == grid.Length)
                {
                    output += "\n\t";
                    count = 0;
                }
            }

            Console.WriteLine(output);
            Console.WriteLine($"\tSolved: {grid.Solved()}");
        }

        public static void DrawGrids(Grid baseGrid, Grid comp)
        {
            Console.ForegroundColor = ConsoleColor.White;

            var count = 0;
            Console.Write("\n\t");
            foreach (var cell in baseGrid.Cells)
            {
                var value = "_";
                if (cell.Value.HasValue)
                {
                    value = cell.Value.Value.ToString();
                }

                var compCell = comp.GetCell(cell.X, cell.Y);

                if (cell.Value.HasValue && !compCell.Value.HasValue)
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                }
                Console.Write(value + " ");
                Console.ForegroundColor = ConsoleColor.White;

                count++;

                if (count == baseGrid.Length)
                {
                    Console.WriteLine();
                    Console.Write("\t");
                    count = 0;
                }
            }
        }
    }
}