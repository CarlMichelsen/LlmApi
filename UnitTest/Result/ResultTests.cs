using Domain.Abstraction;
using Domain.Exception;

namespace UnitTest.Result;

public class ResultTests
{
    [Fact]
    public void ErrorResultNotOk()
    {
        // Arrange
        var exception = new Exception("I'm an exception!");

        // Act
        Result<int> result = exception;

        // Assert
        Assert.NotNull(result.Error);
        Assert.True(result.IsError);
        Assert.False(result.IsSuccess);
        var ex = Assert.Throws<UnhandledResultException>(() => result.Unwrap());
        Assert.Same(exception, ex.InnerException);
    }

    [Fact]
    public void SuccessResultOk()
    {
        // Arrange
        var integer = 42;

        // Act
        Result<int> result = integer;

        // Assert
        Assert.Null(result.Error);
        Assert.False(result.IsError);
        Assert.True(result.IsSuccess);
        Assert.Equal(integer, result.Unwrap());
    }
}
