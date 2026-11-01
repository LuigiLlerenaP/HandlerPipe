using HandlerPipe.Abstractions;
using Xunit;

namespace HandlerPipe.Tests;

public class UnitTests
{
    [Fact]
    public void Value_Equals_Any_Unit_Instance()
    {
        var left = Unit.Value;
        var right = new Unit();

        Assert.True(left == right);
        Assert.Equal(left, right);
        Assert.Equal("()", left.ToString());
    }

    [Fact]
    public async Task Task_Returns_UnitValue()
    {
        var result = await Unit.Task;

        Assert.Equal(Unit.Value, result);
    }
}
