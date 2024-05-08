using System;

namespace XamarinFormsDemoApplication
{

    class Profiler
    {
        private readonly string name;
        private readonly int id;
        private int total;
        private int start;
        private int last;

        public Profiler() : this(null, 0, true)
        {

        }

        public Profiler(int id) : this(null, id, true)
        {

        }
        
        public Profiler(string name, int id, bool start)
        {
            this.name = name;
            this.id = id;
            if (start)
            {
                this.start = Environment.TickCount;
            }
        }
        public void Start()
        {
            if (start != 0)
            {
                throw new InvalidOperationException("Profiler.Start() called many times");
            }
            last = 0;
            start = Environment.TickCount;
        }
        public int Finish()
        {
            if (start == 0)
            {
                throw new InvalidOperationException("Profiler.Finish() called without Start()");
            }
            last = Environment.TickCount - start;
            total += last;
            return last;
        }

        public string Name { get => name; }
        public int Id { get => id; }
        public bool HasId { get => id != 0; }
        public int Total { get => total; }
        public int Last { get => last; }

        public override string ToString()
        {
            return total.ToString();
        }
    }
}