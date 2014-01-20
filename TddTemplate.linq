void Main()
{
	var tests = new SimpleClassTests();
	tests.RunTests();
}
	
class ClassUnderTest
{
}

class SimpleClassTests : UnitTestBase
{
	[Test]
	public void TestSomething()
	{
		var instance = new ClassUnderTest();
		Assert.IsTrue(instance != null);
	}
}	
	
// test framework initially based on http://www.youtube.com/watch?feature=player_detailpage&list=PL3D3F4B7C71FF6AA0&v=hayjhjIKSwA
[AttributeUsage(AttributeTargets.Method)]
class SetupAttribute : Attribute
{
}

[AttributeUsage(AttributeTargets.Method)]
class TestAttribute : Attribute
{
}

[AttributeUsage(AttributeTargets.Method)]
class TeardownAttribute : Attribute
{
}

class Assert
{
	public bool Passed { get; set; }
	public string Message { get; private set; }
	
	public Assert()
	{
		Reset();
	}
	
	public void Reset()
	{
		Passed = true;
		Message = string.Empty;
	}
	
	public void AreEqual(double expected, double actual)
	{
		if(!expected.Equals(actual))
		{
			Passed = false;
			Message = string.Format("Expected {0}, but was {1}.", expected, actual);
		}
	}
	
	public void AreEqual(bool expected, bool actual)
	{
		if(!expected.Equals(actual))
		{
			Passed = false;
			Message = string.Format("Expected {0}, but was {1}.", expected, actual);
		}
	}
	
	public void IsTrue(bool actual)
	{
		AreEqual(true, actual);
	}
	
	public void IsFalse(bool actual)
	{
		AreEqual(false, actual);
	}
	
	public void WriteResults(string methodName)
	{
		if(Passed)
			Console.WriteLine("Success: {0}", methodName);
		else
			Console.WriteLine("Failed: {0} - {1}", methodName, Message);
	}
}


abstract class UnitTestBase
{
	protected Assert Assert {get; private set;}
	
	public UnitTestBase()
	{
		Assert = new Assert();
	}
	
	public void RunTests()
	{
		// run Setup methods
		var methods = this.GetType().GetMethods();
		foreach (var method in methods.Where(m => m.IsDefined(typeof(SetupAttribute), false)))
		{
			this.GetType().InvokeMember(method.Name, BindingFlags.InvokeMethod, null, this, null);
		}
		
		// run Test methods
		foreach (var method in methods.Where(m => m.IsDefined(typeof(TestAttribute), false)))
		{
			// clear results
			Assert.Reset();
			
			// run the test
			this.GetType().InvokeMember(method.Name, BindingFlags.InvokeMethod, null, this, null);
			
			// report results
			Assert.WriteResults(method.Name);
		}
		
		// run Teardown methods
		foreach (var method in methods.Where(m => m.IsDefined(typeof(TeardownAttribute), false)))
		{
			this.GetType().InvokeMember(method.Name, BindingFlags.InvokeMethod, null, this, null);
		}
	}
}
