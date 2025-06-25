using System.Collections;

namespace Core
{
    public class Pair<T, U>
    {
        public Pair (T first, U second)
        {
            this.First = first;
            this.Second = second;
        }
    
        public T First { get; set; }

        public U Second { get; set; }

        public override string ToString ()
        {
            return string.Format ("[Pair: First={0}, Second={1}]", First, Second);
        }
    }
}
