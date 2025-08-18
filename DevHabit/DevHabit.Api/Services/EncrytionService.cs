using System.Security.Cryptography;
using DevHabit.Api.Settings;
using Microsoft.Extensions.Options;

namespace DevHabit.Api.Services;

public sealed class EncrytionService(IOptions<EncryptionOptions> options)
{
    private readonly byte[] _masterKey = Convert.FromBase64String(options.Value.Key);
    private const int IvSize = 16;
    public string Encrypt(string plainText)
    {
        try
        {
            using var aes = Aes.Create();
            aes.Mode = CipherMode.CBC;
            aes.Padding = PaddingMode.PKCS7;
            aes.Key = _masterKey;
            aes.IV = RandomNumberGenerator.GetBytes(IvSize);

            using var memoStream = new MemoryStream();
            memoStream.Write(aes.IV, 0, IvSize);
            using (ICryptoTransform encryptor = aes.CreateEncryptor())
            using (var cryptoStream = new CryptoStream(memoStream, encryptor, CryptoStreamMode.Write))
            using (var streamWriter = new StreamWriter(cryptoStream))
            {
                streamWriter.Write(plainText);
            }
            return Convert.ToBase64String(memoStream.ToArray());
        }
        catch (CryptographicException ex)
        {

            throw new InvalidOperationException("Encryption failed.", ex);
        }

            
    }

    public string Decrypt(string cipherText) {
        try
        {
            byte[] cipherData = Convert.FromBase64String(cipherText);

            if (cipherData.Length < IvSize)
            {
                throw new InvalidOperationException("Invalid cipher text format.");
            }

            // Extract the IV and cipher text data from the cipher data
            var iv = new byte[IvSize];
            var encryptedData = new byte[cipherData.Length - IvSize];

            Buffer.BlockCopy(cipherData, 0, iv, 0, IvSize);
            Buffer.BlockCopy(cipherData, IvSize, encryptedData, 0, encryptedData.Length);

            using var aes = Aes.Create();
            aes.Mode = CipherMode.CBC;
            aes.Padding = PaddingMode.PKCS7;
            aes.Key = _masterKey;
            aes.IV = iv;

            using var memoryStream = new MemoryStream(encryptedData);
            using var decryptor = aes.CreateDecryptor();
            using var cryptoStream = new CryptoStream(memoryStream, decryptor, CryptoStreamMode.Read);
            using var streamReader = new StreamReader(cryptoStream);

            return streamReader.ReadToEnd();
        }
        catch (CryptographicException ex)
        {
            throw new InvalidOperationException("Decryption failed", ex);
        }
        catch (FormatException ex)
        {
            throw new InvalidOperationException("Invalid cipher text format", ex);
        }
    }

}
