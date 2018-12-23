using System;

// This is a simple replacment, 
// cause there is'nt such a class available in Xamarin iOS.
namespace System.Diagnostics.CodeAnalysis {
	public class ExcludeFromCodeCoverageAttribute : Attribute {
		public ExcludeFromCodeCoverageAttribute () {
		}
	}
}
