using System;
using System.Collections.Generic;
using System.Linq;

namespace SudokuSolver
{
    public static class Solver
    {
        public static int Counter { get; set; }

        public static bool Solve(Grid grid)
        {
            Counter = 0;
            if (SolveBrute(grid))
            {
                Steps.Reverse();

                // draw steps:
                //var previous = grid;
                //foreach (var step in Steps)
                //{
                // Drawer.DrawGrids(step, previous);
                //    previous = step;
                //}

                Console.WriteLine();
                Drawer.DrawGrid(Steps.Last());
                Console.WriteLine($"\tOperations: {Counter}");
                Console.WriteLine($"\tSteps: {Steps.Count}");
                return true;
            }

            Drawer.DrawGrid(grid);
            Console.Write("Unable to solve!");
            return false;
        }

        public static bool SolveBrute(Grid grid)
        {
            var workGrid = grid.Copy();

            for (var y = 0; y < workGrid.Length; y++)
            {
                for (var x = 0; x < workGrid.Length; x++)
                {
                    var cell = workGrid.GetCell(x, y);
                    if (!cell.Value.HasValue)
                    {
                        cell.Value = 1;
                        while (!workGrid.Valid())
                        {
                            cell.Value++;
                            if (cell.Value > workGrid.Length)
                            {
                                return false;
                            }
                        }

                        if (SolveBrute(workGrid))
                        {
                            Steps.Add(workGrid);
                            return true;
                        }
                        else
                        {
                            cell.Value++;
                            while (!workGrid.Valid())
                            {
                                cell.Value++;
                                if (cell.Value > workGrid.Length)
                                {
                                    return false;
                                }
                            }
                        }
                    }
                }
            }

            return workGrid.Solved();
        }

        public static List<Grid> Steps = new List<Grid>();

        // not working :(
        private static bool SolveRecurse(Grid grid, int level = 0)
        {
            level++;
            grid.Propagate();
            foreach (var cell in grid.GetUnsolvedCells().OrderBy(c => grid.GetOptionsForCell(c).Count))
            {
                Console.WriteLine($"{level}: {cell}");
                foreach (var option in grid.GetOptionsForCell(cell))
                {
                    var workGrid = grid.Copy();
                    workGrid.SetValue(cell.X, cell.Y, option);
                    workGrid.Propagate();
                    //Drawer.DrawGrids(workGrid, grid);

                    if (workGrid.Solved())
                    {
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.WriteLine("Solved");
                        //Drawer.DrawGrid(workGrid);
                        return true;
                    }
                    if (SolveRecurse(workGrid, level))
                    {
                        return true;
                    }
                }
            }

            Console.WriteLine($"Break after {level}");
            return false;
        }

    }

}