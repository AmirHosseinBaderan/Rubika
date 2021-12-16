using Rubika.Package.Bot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rubika.Package.Crypto;

internal static class CryptoEx
{
    private static readonly byte[] IV = { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };

    private static AesCryptoServiceProvider _aesCrypto;

    private static ICryptoTransform _cryptoTransform;

    private static List<Message> _messages = new();

    public static byte[] CreateAndSetKey(this string auth)
    {
        string str = auth[16..][..8] + auth[..8] + auth[24..] + auth[8..][..8];
        char[] sb = str.ToCharArray();


        for (int i = 0; i < sb.Length; i++)
        {
            if (sb[i] >= '0' && sb[i] <= '9')
            {
                sb[i] = (char)((((str[i] - '0') + 5) % 10) + 48);
            }
            if (sb[i] >= 'a' && sb[i] <= 'z')
            {
                sb[i] = (char)((((str[i] - 'a') + 9) % 26) + 97);
            }
        }

        byte[] key = ASCIIEncoding.ASCII.GetBytes(new String(sb));
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

    public static byte[] GetBytes(this string str) => Encoding.UTF8.GetBytes(str);

    public static string Crypto(this string data, bool get)
    {
        if (get)
        {
            data = data.Replace("\\n", "\n");
            _cryptoTransform = _aesCrypto.CreateDecryptor(_aesCrypto.Key, _aesCrypto.IV);
            return Encoding.UTF8.GetString(_cryptoTransform.TransformFinalBlock(Convert.FromBase64String(data), 0, Convert.FromBase64String(data).Length));
        }
        else
        {
            byte[] f = data.GetBytes();
            _cryptoTransform = _aesCrypto.CreateEncryptor(_aesCrypto.Key, _aesCrypto.IV);
            return Convert.ToBase64String(_cryptoTransform.TransformFinalBlock(f, 0, f.Length));
        }
    }

}
