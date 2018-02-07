using System;
using System.Linq;
using xFunc.Maths.Analyzers;
namespace xFunc.Maths.Expressions {
    public class MultiCondition : DifferentParametersExpression {

        public MultiCondition() : base(-1) { }

        public MultiCondition(params Condition[] conditions) 
            : base (conditions, conditions.Length){
        }

        public MultiCondition(IExpression[] arguments, int countOfParams)
            : base(arguments, countOfParams) {

            if (arguments == null)
                throw new ArgumentNullException(nameof(arguments));

            if (arguments.Length != countOfParams)
                throw new ArgumentException();
        }

        #region Properties

        public override int MinParameters => 1;

        public override int MaxParameters => -1;

        public override ExpressionResultType[] ArgumentsTypes {
            get {
                var result = new ExpressionResultType[ParametersCount];
                if (ParametersCount > 0) {
                    return Enumerable.Repeat<ExpressionResultType>(ExpressionResultType.All, ParametersCount).ToArray();
                }
                return result;
            }
        }

        #endregion

        public override TResult Analyze<TResult>(IAnalyzer<TResult> analyzer) {
            return analyzer.Analyze(this);
        }

        public override IExpression Clone() {
            return new MultiCondition(CloneArguments(), ParametersCount);
        }

        public override object Execute(ExpressionParameters parameters) {
            foreach (var condition in this.Arguments.OfType<Condition>()) {
                if (condition.IsFulfilled(parameters)) {
                    return condition.Execute(parameters);
                }
            }
            return Double.NaN;
        }
    }
}
