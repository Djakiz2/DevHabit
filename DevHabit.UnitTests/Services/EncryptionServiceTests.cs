using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using DevHabit.Api.Services;
using DevHabit.Api.Settings;
using Microsoft.Extensions.Options;

namespace DevHabit.UnitTests.Services;
public sealed class EncryptionServiceTests
{
    private readonly EncryptionService _encryptionService;

    public EncryptionServiceTests()
    {
        IOptions<EncryptionOptions> options = Options.Create(new EncryptionOptions
        {
            Key = Convert.ToBase64String(RandomNumberGenerator.GetBytes(32))
        });
        _encryptionService = new EncryptionService(options);
    }

    [Fact]
    public void Decrypt_ShouldReturnPlainText_WhenDecryptingCorrectCiphertext()
    {
        // Arrange

        const string plainText = "sensitive data";
        string cipherText = _encryptionService.Encrypt(plainText);

        // Act

        string decryptionCipherText = _encryptionService.Decrypt(cipherText);



        // Assert

        Assert.Equal(plainText, decryptionCipherText);
    }


    [Fact]
    public void Encrypt_ShouldReturnDifferentCiphertext_WhenEncryptingText()
    {
        // Arrange
        const string plainText = "sensitive data";

        // Act
        string firstCipherText = _encryptionService.Encrypt(plainText);
        string secondCipherText = _encryptionService.Encrypt(plainText);

        // Assert
        Assert.NotEqual(firstCipherText, secondCipherText);
    }

    [Theory]
    [InlineData("")]
    [InlineData("  ")]
    [InlineData("not-base64")]
    [InlineData("aW52YWxpZC1jaXBoZXJ0ZXh0")] // too short, missing IV
    public void Decrypt_ShouldThrowInvalidOperationException_WhenCiphertextIsInvalid(string invalidCipherText)
    {
        // Act & Assert
        Assert.Throws<InvalidOperationException>(() => _encryptionService.Decrypt(invalidCipherText));
    }

    [Fact]
    public void Decrypt_ShouldThrowInvalidOperationException_WhenCiphertextIsCorrupted()
    {
        // Arrange
        const string plainText = "sensitive data";
        string cipherText = _encryptionService.Encrypt(plainText);
        string corruptedCiphertext = cipherText[..^10] + new string('0', 10); // Corrupt last 10 characters

        // Act and Assert
        Assert.Throws<InvalidOperationException>(() => _encryptionService.Decrypt(corruptedCiphertext));
    }

    [Fact]
    public void Encrypt_ShouldHandleLongText()
    {
        // Arrange
        string longText = new string('a', 10000);

        // Act
        string cipherText = _encryptionService.Encrypt(longText);
        string decryptedText = _encryptionService.Decrypt(cipherText);

        // Assert

        Assert.Equal(longText, decryptedText);
    }

    [Fact]
    public void Encrypt_ShouldHandleSpecialCharacters()
    {
        // Arrange
        const string specialChars = "!@#$%^&*()_+-=[]{}|;:'\",.<>?/~`";

        // Act

        string cipherText = _encryptionService.Encrypt(specialChars);
        string decryptedText = _encryptionService.Decrypt(cipherText);

        // Assert
        Assert.Equal(specialChars, decryptedText);
    }

}
