namespace xFunc.Maths {
    internal struct Vector2 {
        public Vector2(double x, double y) {
            this.X = x;
            this.Y = y;
        }

        public double X;
        public double Y;

        public override bool Equals(object obj) {
            var v = (Vector2)obj;
            return v.X == this.X && v.Y == this.Y;
        }

        public override int GetHashCode() {
            return ("Vector2(x:" + this.X + " y:" + this.Y + ")").GetHashCode();
        }

        public override string ToString() {
            return "X: " + this.X.ToString() + ", Y: " + this.Y.ToString();
        }

    }
}
