using System;
using System.Threading;
using System.Threading.Tasks;
using AI_DEMO.Application.Common.Exceptions;
using AI_DEMO.Application.Features.UserManagement.Commands;
using AI_DEMO.Domain.Entities;
using AI_DEMO.Domain.Enums;
using AI_DEMO.Domain.Interfaces;
using FluentAssertions;
using Moq;
using Xunit;

/// <summary>
/// CreateUserCommandHandler 的單元測試類別。
/// </summary>
public class CreateUserCommandHandlerTests
{
    private readonly Mock<IUserRepository> _mockUserRepository;
    private readonly Mock<IPasswordHasher> _mockPasswordHasher;

    /// <summary>
    /// 初始化測試類別。
    /// </summary>
    public CreateUserCommandHandlerTests()
    {
        _mockUserRepository = new Mock<IUserRepository>();
        _mockPasswordHasher = new Mock<IPasswordHasher>();
    }

    /// <summary>
    /// 測試當命令有效時應建立使用者。
    /// </summary>
    [Fact]
    public async Task Should_CreateUser_When_CommandIsValid()
    {
        // Arrange
        _mockUserRepository
            .Setup(r => r.ExistsAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(false); // 假設使用者名稱和電子郵件不存在

        _mockPasswordHasher
            .Setup(h => h.HashPassword(It.IsAny<string>()))
            .Returns("hashedPassword"); // 模擬密碼雜湊

        _mockUserRepository
            .Setup(r => r.AddAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask); // 模擬新增使用者成功

        var handler = new CreateUserCommandHandler(_mockUserRepository.Object, _mockPasswordHasher.Object);
        var command = new CreateUserCommand("testuser", "test@example.com", "password123", UserRole.User);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeEmpty(); // 應返回非空 GUID

        _mockUserRepository.Verify(r => r.ExistsAsync("testuser", "test@example.com", It.IsAny<CancellationToken>()), Times.Once);
        _mockPasswordHasher.Verify(h => h.HashPassword("password123"), Times.Once);
        _mockUserRepository.Verify(r => r.AddAsync(It.Is<User>(u => u.Username == "testuser" && u.Email == "test@example.com"), It.IsAny<CancellationToken>()), Times.Once);
    }

    /// <summary>
    /// 測試當使用者名稱或電子郵件已存在時應拋出 DuplicateUserException。
    /// </summary>
    [Fact]
    public async Task Should_ThrowDuplicateUserException_When_UsernameOrEmailExists()
    {
        // Arrange
        _mockUserRepository
            .Setup(r => r.ExistsAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true); // 假設使用者名稱和電子郵件已存在

        var handler = new CreateUserCommandHandler(_mockUserRepository.Object, _mockPasswordHasher.Object);
        var command = new CreateUserCommand("existinguser", "existing@example.com", "password123", UserRole.User);

        // Act & Assert
        await Assert.ThrowsAsync<DuplicateUserException>(() => handler.Handle(command, CancellationToken.None));

        _mockUserRepository.Verify(r => r.ExistsAsync("existinguser", "existing@example.com", It.IsAny<CancellationToken>()), Times.Once);
        _mockPasswordHasher.Verify(h => h.HashPassword(It.IsAny<string>()), Times.Never); // 不應呼叫雜湊
        _mockUserRepository.Verify(r => r.AddAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()), Times.Never); // 不應呼叫新增
    }

    /// <summary>
    /// 測試應正確呼叫密碼雜湊功能。
    /// </summary>
    [Fact]
    public async Task Should_HashPassword_When_CreatingUser()
    {
        // Arrange
        _mockUserRepository
            .Setup(r => r.ExistsAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        _mockPasswordHasher
            .Setup(h => h.HashPassword(It.IsAny<string>()))
            .Returns("hashedPassword");

        _mockUserRepository
            .Setup(r => r.AddAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        var handler = new CreateUserCommandHandler(_mockUserRepository.Object, _mockPasswordHasher.Object);
        var command = new CreateUserCommand("testuser", "test@example.com", "password123", UserRole.User);

        // Act
        await handler.Handle(command, CancellationToken.None);

        // Assert
        _mockPasswordHasher.Verify(h => h.HashPassword("password123"), Times.Once); // 應正確呼叫雜湊一次
    }
}