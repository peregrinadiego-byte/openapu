using OpenAPU.Api.Security;

namespace OpenAPU.Api.Tests;

public sealed class AdminAccessOptionsTests
{
    [Fact]
    public void Empty_key_disables_admin_protection()
    {
        var options = new AdminAccessOptions("");

        Assert.False(options.Enabled);
    }

    [Fact]
    public void Configured_key_enables_admin_protection()
    {
        var options = new AdminAccessOptions(
            "a-long-administrative-key");

        Assert.True(options.Enabled);
    }
}
