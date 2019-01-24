using xFunc.Maths;
using xFunc.Maths.Analyzers;
using xFunc.Maths.Expressions;
using Xunit;
using System.Linq;
using System;

namespace xFunc.Tests
{
	public class DerivationStepTest
	{
		[Fact]
		public void Testing_of_derivation_steps () {
			var differentiator 	= new Differentiator ();
			var proc 			= new Processor ();
			var exp 			= proc.Parse ("(x+x)^2");

			var derivative 		= exp.Analyze (differentiator);

			Assert.True (differentiator.RootStep != null);
			Assert.True (differentiator.RootStep.Expression is Pow pow);
			Assert.Equal (proc.Parse("8x"), differentiator.RootStep.SimplifiedDerivative);


		}
	}
}
