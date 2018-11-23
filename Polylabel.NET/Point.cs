namespace Polylabel.NET
{
    public struct Point
    {
        public double X;
        public double Y;

        public Point(double x, double y)
        {
            this.X = x;
            this.Y = y;
        }

        public static Point operator +(Point a, Point b)
        {
            return new Point(a.X + b.X, a.Y + b.Y);
        }

        public static Point operator -(Point a, Point b)
        {
            return new Point(a.X - b.X, a.Y - b.Y);
        }

        public static Point operator +(Point a, double b)
        {
            return new Point(a.X + b, a.Y + b);
        }

        public static Point operator -(Point a, double b)
        {
            return new Point(a.X - b, a.Y - b);
        }
    }
}
