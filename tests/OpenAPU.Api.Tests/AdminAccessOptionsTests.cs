using OpenAPU.Api.Security;

namespace OpenAPU.Api.Tests;

public sealed class AdminAccessOptionsTests
{
    [Fact]
    public void Empty_key_disables_admin_protection()
    {
        var options = AdminAccessOptions.Create("");

        Assert.False(options.Enabled);
        Assert.Null(options.Key);
    }

    [Fact]
    public void Configured_key_enables_admin_protection()
    {
        var options = AdminAccessOptions.Create(
            "a-long-administrative-key");

        Assert.True(options.Enabled);
    }

    [Fact]
    public void Short_key_is_rejected()
    {
        var exception = Assert.Throws<InvalidOperationException>(
            () => AdminAccessOptions.Create("too-short"));

        Assert.Contains(
            AdminAccessOptions.MinimumKeyLength.ToString(),
            exception.Message);
    }

    [Fact]
    public void Minimum_length_key_is_accepted()
    {
        var key = new string(
            'x',
            AdminAccessOptions.MinimumKeyLength);

        var options = AdminAccessOptions.Create(key);

        Assert.True(options.Enabled);
        Assert.Equal(key, options.Key);
    }
}
