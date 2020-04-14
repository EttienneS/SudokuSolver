namespace SudokuSolver
{
    public class Cell
    {
        private int? _value;
        public int? Value
        {
            get
            {
                return _value;
            }
            set
            {
                _value = value;
                Solver.Counter++;
            }
        }
       
        public int X { get; set; }
        public int Y { get; set; }

        public Cell(int x, int y)
        {
            X = x;
            Y = y;
        }

        public override string ToString()
        {
            return $"{X}:{Y}={Value}";
        }
    }
}
