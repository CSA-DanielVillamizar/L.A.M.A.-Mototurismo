using Xunit;
using Lama.Infrastructure.Services;

namespace Lama.UnitTests.Services;

/// <summary>
/// Tests unitarios para TenantContext
/// Verifica que el contexto de tenant mantiene y proporciona el tenant correcto
/// </summary>
public class TenantContextTests
{
    [Fact]
    public void NewTenantContext_ShouldHaveDefaultTenant()
    {
        // Arrange & Act
        var tenantContext = new TenantContext();

        // Assert
        Assert.Equal(TenantContext.DefaultTenantId, tenantContext.CurrentTenantId);
        Assert.Equal(TenantContext.DefaultTenantName, tenantContext.CurrentTenantName);
        Assert.True(tenantContext.IsDefaultTenant);
    }

    [Fact]
    public void SetCustomTenantId_ShouldUpdateCurrentTenantId()
    {
        // Arrange
        var customTenantId = Guid.NewGuid();
        var tenantContext = new TenantContext();

        // Act
        tenantContext.CurrentTenantId = customTenantId;

        // Assert
        Assert.Equal(customTenantId, tenantContext.CurrentTenantId);
        Assert.False(tenantContext.IsDefaultTenant);
    }

    [Fact]
    public void SetCustomTenantName_ShouldUpdateCurrentTenantName()
    {
        // Arrange
        var customName = "CUSTOM_TENANT";
        var tenantContext = new TenantContext();

        // Act
        tenantContext.CurrentTenantName = customName;

        // Assert
        Assert.Equal(customName, tenantContext.CurrentTenantName);
    }

    [Fact]
    public void ResetToDefault_ShouldRestoreBothIdAndName()
    {
        // Arrange
        var tenantContext = new TenantContext();
        tenantContext.CurrentTenantId = Guid.NewGuid();
        tenantContext.CurrentTenantName = "CUSTOM_TENANT";

        // Act
        tenantContext.ResetToDefault();

        // Assert
        Assert.Equal(TenantContext.DefaultTenantId, tenantContext.CurrentTenantId);
        Assert.Equal(TenantContext.DefaultTenantName, tenantContext.CurrentTenantName);
        Assert.True(tenantContext.IsDefaultTenant);
    }

    [Fact]
    public void DefaultTenantIdConstant_ShouldBeCorrectGuid()
    {
        // Arrange & Act
        var expectedGuid = Guid.Parse("00000000-0000-0000-0000-000000000001");

        // Assert
        Assert.Equal(expectedGuid, TenantContext.DefaultTenantId);
    }

    [Fact]
    public void DefaultTenantNameConstant_ShouldBeCorrectName()
    {
        // Arrange & Act & Assert
        Assert.Equal("LAMA_DEFAULT", TenantContext.DefaultTenantName);
    }

    [Fact]
    public void MultipleInstances_ShouldBeIndependent()
    {
        // Arrange
        var tenantContext1 = new TenantContext();
        var tenantContext2 = new TenantContext();
        var customTenant = Guid.NewGuid();

        // Act
        tenantContext1.CurrentTenantId = customTenant;

        // Assert - tenantContext2 no debe ser afectado
        Assert.Equal(customTenant, tenantContext1.CurrentTenantId);
        Assert.Equal(TenantContext.DefaultTenantId, tenantContext2.CurrentTenantId);
    }
}
