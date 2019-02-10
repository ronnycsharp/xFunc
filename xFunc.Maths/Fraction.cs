using System;
using xFunc.Maths.Expressions;

// https://stackoverflow.com/questions/5360196/how-can-you-add-two-fractions

namespace xFunc.Maths {
    public struct Fraction {
        public Fraction (int numerator, int denominator) {
            Numerator   = numerator;
            Denominator = denominator;
        }

        public Fraction(int number) {
            Numerator      = number;
            Denominator    = 1;
        }

        #region Properties

        public int Numerator    { get; set; }

        public int Denominator  { get; set; }

        #endregion

        #region Operators 

        public static Fraction operator + (Fraction left, Fraction right)
        {
            var left2 = left.InTermsOf (right);
            var right2 = right.InTermsOf (left);

            return new Fraction (left2.Numerator + right2.Numerator, left2.Denominator);
        }

        public static Fraction operator - (Fraction left, Fraction right)
        {
            var left2 = left.InTermsOf (right);
            var right2 = right.InTermsOf (left);

            return new Fraction (left2.Numerator - right2.Numerator, left2.Denominator);
        }

        public static Fraction operator * (Fraction left, Fraction right)
        {
            return new Fraction (left.Numerator * right.Numerator, left.Denominator * right.Denominator);
        }

        public static Fraction operator / (Fraction left, Fraction right)
        {
            return new Fraction (left.Numerator * right.Denominator, left.Denominator * right.Numerator);
        }

        /*
        public static implicit operator Fraction (string value)
        {
            var tokens = value.Split ('/');
            int num;
            int den;
            if (tokens.Length == 1 && int.TryParse (tokens [0], out num)) {
                return new Fraction (num, 1);
            } else if (tokens.Length == 2 && int.TryParse (tokens [0], out num) && int.TryParse (tokens [1], out den)) {
                return new Fraction (num, den);
            }
            throw new Exception ("Invalid fraction format");
        }*/

        public static implicit operator Fraction (Div div) {
            throw new NotImplementedException ();
        }

        #endregion

        public Fraction Simplify () {
            int gcd = GCD ();
            return new Fraction (Numerator / gcd, Denominator / gcd);
        }

        public Fraction InTermsOf (Fraction other) {
            return Denominator == other.Denominator ? this :
                new Fraction (Numerator * other.Denominator, Denominator * other.Denominator);
        }

        private int GCD () {
            int a = Numerator;
            int b = Denominator;
            while (b != 0) {
                int t = b;
                b = a % b;
                a = t;
            }
            return a;
        }

        public Fraction Reciprocal () {
            return new Fraction (Denominator, Numerator);
        }

        public override string ToString () {
            return string.Format ("{0}/{1}", Numerator, Denominator);
        }

        public static bool TryConvert (IExpression exp, out Fraction fraction) {
            if (exp is Number number && number.Value == (int)number.Value) {
                fraction = new Fraction { 
                        Numerator = (int)number.Value, 
                        Denominator = 1 
                    };
                return true;
            } else if (exp is Div div && div.Left is Number num && div.Right is Number denom
                            && num.Value == (int)num.Value
                            && denom.Value == (int)denom.Value) {

                fraction = new Fraction {
                    Numerator   = (int)num.Value,
                    Denominator = (int)denom.Value
                };
                return true;
            }
            fraction = new Fraction ();
            return false;
        }

        public IExpression ToExpression () {
            if (this.Numerator == 0)
                return new Number (0);
            else {
                if (this.Denominator == 1)
                    return new Number (this.Numerator);

                if ((this.Numerator < 0) ^ (this.Denominator < 0)) {
                    // the numerator and denominator should be visualized without a negative value,
                    // instead the div-expression should be a child of a unary minus expression.

                    return new UnaryMinus (
                        new Div (
                            new Number (Math.Abs(this.Numerator)),
                            new Number (Math.Abs(this.Denominator))));
                }
                return new Div (
                    new Number (this.Numerator), 
                    new Number (this.Denominator));

            }
        }
    }
}
