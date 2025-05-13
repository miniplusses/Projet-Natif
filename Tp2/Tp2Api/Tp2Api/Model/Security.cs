using System.Security.Cryptography;
using System.Text;

namespace Tp2Api.Model; 

public static class Security {
    
    public static string EncryptString(string plainText, string key) {
        using (Aes aesAlg = Aes.Create()) {
            aesAlg.Key = Encoding.UTF8.GetBytes(key);
            aesAlg.GenerateIV();

            ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);

            using (MemoryStream msEncrypt = new MemoryStream()) {
                using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write)) {
                    using (StreamWriter swEncrypt = new StreamWriter(csEncrypt)) {
                        swEncrypt.Write(plainText);
                    }
                }

                byte[] encrypted = msEncrypt.ToArray();
                byte[] result = new byte[aesAlg.IV.Length + encrypted.Length];
                Buffer.BlockCopy(aesAlg.IV, 0, result, 0, aesAlg.IV.Length);
                Buffer.BlockCopy(encrypted, 0, result, aesAlg.IV.Length, encrypted.Length);
                
                return BitConverter.ToString(result).Replace("-", "");
            }
        }
    }

    public static string DecryptString(string cipherText, string key) {
        using (Aes aesAlg = Aes.Create()) {
            aesAlg.Key = Encoding.UTF8.GetBytes(key);
            
            byte[] iv = new byte[aesAlg.BlockSize / 8];
            Buffer.BlockCopy(StringToByteArray(cipherText), 0, iv, 0, iv.Length);
            aesAlg.IV = iv;

            ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);

            using (MemoryStream msDecrypt = new MemoryStream()) {
                using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Write)) {
                    using (StreamWriter swDecrypt = new StreamWriter(csDecrypt)) {
                        byte[] cipherBytes = StringToByteArray(cipherText.Substring(32));
                        csDecrypt.Write(cipherBytes, 0, cipherBytes.Length);
                    }
                }

                return Encoding.UTF8.GetString(msDecrypt.ToArray());
            }
        }
    }
    
    public static string GenerateRandomString(int length) {
        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
        Random random = new Random();
        StringBuilder stringBuilder = new StringBuilder(length);

        for (int i = 0; i < length; i++) {
            int index = random.Next(chars.Length);
            stringBuilder.Append(chars[index]);
        }

        return stringBuilder.ToString();
    }

    static byte[] StringToByteArray(string hex) {
        int numberChars = hex.Length;
        byte[] bytes = new byte[numberChars / 2];
        for (int i = 0; i < numberChars; i += 2) {
            bytes[i / 2] = Convert.ToByte(hex.Substring(i, 2), 16);
        }
        return bytes;
    }
}