using Xunit;
using AppCore;
using AppCore.ValueObjects;
using Assert = Xunit.Assert;

namespace UnitTests;

public class EmailAddressTests
{
    [Fact]
    public void Should_Parse_Email_Correctly()
    {
        var email = new EmailAddress("test@example.com");

        Assert.Equal("test", email.User);
        Assert.Equal("example.com", email.Domain);
    }

    [Fact]
    public void Should_Throw_For_Invalid_Email()
    {
        Assert.Throws<ArgumentException>(() => 
            new EmailAddress("wrong-email"));
    }

    [Fact]
    public void Should_Format_Email()
    {
        var email = new EmailAddress("john@domain.com");

        Assert.Equal("john@domain.com", email.ToString());
    }
}