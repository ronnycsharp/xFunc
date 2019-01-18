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

			Assert.True (differentiator.Steps.Count > 0);
			Assert.True (differentiator.Steps.Values.ToArray() [0].Expression is Pow);
			Assert.True (differentiator.Steps.Values.ToArray() [1].Expression is Add);

			Assert.Equal (proc.Parse("8x"), differentiator.Steps.Values.ToArray () [0].Simplified);
		}
	}
}
