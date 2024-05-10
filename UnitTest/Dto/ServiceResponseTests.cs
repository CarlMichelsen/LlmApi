using Domain.Dto;

namespace UnitTest.Dto;

public class ServiceResponseTests
{
    [Fact]
    public void StringServiceResponseShouldBeOk()
    {
        // Arrange
        // Act
        var res = new ServiceResponse<string>("stfu");
        
        // Assert
        Assert.NotNull(res.Data);
        Assert.True(res.Ok);
    }

    [Fact]
    public void ServiceResponseNotOkWithError()
    {
        // Arrange
        var res = new ServiceResponse<object>(new Dictionary<string, int>());
        
        // Act
        res.Errors.Add("Not the answer to life, the universe and everything.");

        // Assert
        Assert.NotNull(res.Data);
        Assert.False(res.Ok);
    }

    [Fact]
    public void ErrorsShouldAllBePresent()
    {
        // Arrange
        var errorList = new List<string>
        {
            "not fun",
            "meme error",
        };

        // Act
        var res = new ServiceResponse<string>(errorList.ToArray());
        
        // Assert
        Assert.Null(res.Data);
        Assert.False(res.Ok);
        Assert.Equal(errorList, res.Errors);
    }

    [Fact]
    public void ShouldNeverBeOkWithNullData()
    {
        // Arrange
        // Act
        object? data = null;
        var res = new ServiceResponse<object?>(data);
        
        // Assert
        Assert.Null(res.Data);
        Assert.False(res.Ok);
        Assert.Empty(res.Errors);
    }
}
