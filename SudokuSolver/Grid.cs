using System;
using System.Collections.Generic;
using System.Linq;

namespace SudokuSolver
{
    public class Grid
    {
        public Grid(int size)
        {
            Size = size;
            Length = Size * Size;
            Domain = new int[Length];
            for (var i = 0; i < Length; i++)
            {
                Domain[i] = i + 1;
            }

            Cells = new List<Cell>();
            for (var y = 0; y < Length; y++)
            {
                for (var x = 0; x < Length; x++)
                {
                    Cells.Add(new Cell(x, y));
                }
            }

            Horizontals = new List<Cell[]>();
            Verticals = new List<Cell[]>();
            for (var x = 0; x < Length; x++)
            {
                Horizontals.Add(Cells.Where(c => c.X == x).ToArray());
            }

            for (var y = 0; y < Length; y++)
            {
                Verticals.Add(Cells.Where(c => c.Y == y).ToArray());
            }

            SubGrids = new List<Cell[]>();
            for (var x = 0; x < Length; x += Size)
            {
                for (var y = 0; y < Length; y += Size)
                {
                    SubGrids.Add(Cells.Where(c => c.X >= x && c.X < x + Size
                                              && c.Y >= y && c.Y < y + Size).ToArray());
                }
            }
        }

        public List<Cell> Cells { get; }
        public int[] Domain { get; }
        public List<Cell[]> Horizontals { get; }
        public int Length { get; }
        public int Size { get; }
        public List<Cell[]> SubGrids { get; }
        public List<Cell[]> Verticals { get; }

        public List<Cell> GetDomainCells(Cell cell)
        {
            var cells = new List<Cell>();
            cells.AddRange(Verticals.First(v => v.Contains(cell)));
            cells.AddRange(Horizontals.First(v => v.Contains(cell)));
            cells.AddRange(SubGrids.First(v => v.Contains(cell)));

            return cells.Distinct().ToList();
        }

        public List<Cell> GetUnsolvedCells()
        {
            return Cells.Where(c => !c.Value.HasValue).ToList();
        }

        public string Save()
        {
            var output = "";

            for (var y = 0; y < Length; y++)
            {
                for (var x = 0; x < Length; x++)
                {
                    var cell = GetCell(x, y);
                    output += (cell.Value.HasValue ? cell.Value.ToString() : " ") + ",";
                }
            }

            return output;
        }

        public bool SectionValid(Cell[] cells)
        {
            var domain = Domain.ToList();
            foreach (var cell in cells)
            {
                if (cell.Value.HasValue)
                {
                    if (domain.Contains(cell.Value.Value))
                    {
                        domain.Remove(cell.Value.Value);
                    }
                    else
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        public bool SectionSolved(Cell[] cells)
        {
            var domain = Domain.ToList();
            foreach (var cell in cells)
            {
                if (cell.Value.HasValue)
                {
                    if (domain.Contains(cell.Value.Value))
                    {
                        domain.Remove(cell.Value.Value);
                    }
                    else
                    {
                        throw new Exception("Invalid state!");
                    }
                }
                else
                {
                    return false;
                }
            }
            return true;
        }

        public void SetValue(int x, int y, int? value)
        {
            // this is not super performant, for huge grids rather index the cells and use x,y as keys
            GetCell(x, y).Value = value;
        }

        public Cell GetCell(int x, int y)
        {
            // this is not super performant, for huge grids rather index the cells and use x,y as keys
            return Cells.First(c => c.X == x && c.Y == y);
        }

        public bool Valid()
        {
            foreach (var section in GetSections())
            {
                if (!SectionValid(section))
                {
                    return false;
                }
            }

            return true;
        }

        public bool Solved()
        {
            foreach (var section in GetSections())
            {
                if (!SectionSolved(section))
                {
                    return false;
                }
            }

            return true;
        }

        private List<Cell[]> GetSections()
        {
            var sections = Horizontals.ToList();
            sections.AddRange(Verticals);
            sections.AddRange(SubGrids);
            return sections;
        }

        internal static Grid Load(string input)
        {
            //   X  0   1   2   3
            // Y ##################
            // 0 #  1   2   3   4
            // 1 #  4   3   2   1
            // 2 #  3   4   1   2
            // 3 #  2   1   4   3


            var size = (int)Math.Sqrt(Math.Sqrt(input.Length / 2));
            var grid = new Grid(size);

            var x = 0;
            var y = 0;
            foreach (var c in input.Split(','))
            {
                if (!string.IsNullOrWhiteSpace(c))
                {
                    grid.SetValue(x, y, int.Parse(c));
                }
                x++;
                if (x > grid.Length - 1)
                {
                    y++;
                    x = 0;
                }
            }

            return grid;
        }

       

        public Grid Copy()
        {
            return Load(Save());
        }

        internal bool Propagate()
        {
            // recursively infer every sure value
            var unsolved = GetUnsolvedCells();

            var updated = false;
            foreach (var cell in unsolved)
            {
                var options = GetOptionsForCell(cell);
                if (options.Count == 1)
                {
                    cell.Value = options[0];
                    updated = true;
                }
            }

            if (updated)
            {
                Propagate();
            }
            return updated;
        }

        public List<int> GetOptionsForCell(Cell cell)
        {
            var options = Domain.ToList();
            foreach (var domainCell in GetDomainCells(cell))
            {
                if (domainCell.Value.HasValue)
                {
                    options.Remove(domainCell.Value.Value);
                }
            }

            return options;
        }
    }
}