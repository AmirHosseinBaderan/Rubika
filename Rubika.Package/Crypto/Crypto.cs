namespace Rubika.Package.Crypto;

internal static class CryptoEx
{
    private static readonly byte[] IV = { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };

    private static AesCryptoServiceProvider _aesCrypto;

    private static ICryptoTransform _cryptoTransform;

    public static byte[] CreateAndSetKey(this string auth)
    {
        string str = auth[16..][..8] + auth[..8] + auth[24..] + auth[8..][..8];
        StringBuilder sb = new(str);


        for (int i = 0; i < sb.Length; i++)
        {
            if (sb[i] >= '0' && sb[i] <= '9')
                sb[i] = (char)((((str[i] - '0') + 5) % 10) + 48);

            if (sb[i] >= 'a' && sb[i] <= 'z')
                sb[i] = (char)((((str[i] - 'a') + 9) % 26) + 97);
        }

        byte[] key = ASCII.GetBytes(sb.ToString());
        _aesCrypto = new()
        {
            BlockSize = 128,
            KeySize = 256,
            Key = key,
            IV = IV,
            Padding = PaddingMode.PKCS7,
            Mode = CipherMode.CBC
        };

        return key;
    }

    public static byte[] GetBytes(this string str) => UTF8.GetBytes(str);

    public static string Encrypt(this string data)
    {
        byte[] f = data.GetBytes();
        _cryptoTransform = _aesCrypto.CreateEncryptor(_aesCrypto.Key, _aesCrypto.IV);
        byte[] transform = _cryptoTransform.TransformFinalBlock(f, 0, f.Length);
        return ToBase64String(transform);
    }

    public static string Decrypt(this string data)
    {
        data = data.Replace("\\n", "\n");
        _cryptoTransform = _aesCrypto.CreateDecryptor(_aesCrypto.Key, _aesCrypto.IV);
        byte[] dataBytes = FromBase64String(data);
        byte[] transform = _cryptoTransform.TransformFinalBlock(dataBytes, 0, dataBytes.Length);
        return UTF8.GetString(transform);
    }
}
