using UnityEngine;
using System.Collections;
using System.Text;

/// <summary>
/// 
/// Add a feature to the Texture class which allows you to detect the case when you have attempted to download a bogus WWW Texture.
///
/// by Matt "Trip" Maker, Monstrous Company :: http://monstro.us
/// 
/// TODO could also use the filesystem cache to keep the example error image between runs.
///
/// from http://unifycommunity.com/wiki/index.php?title=TextureBogusExtensions
/// 
/// </summary>
public static class TextureBogusExtension
{
    private static byte[] questionMarkPNG = new byte[] { 137, 80, 78, 71, 13, 10, 26, 10, 0, 0, 0, 13, 73, 72, 68, 82, 0, 0, 0, 8, 0, 0, 0, 8, 8, 2, 0, 0, 0, 75, 109, 41, 220, 0, 0, 0, 65, 73, 68, 65, 84, 8, 29, 85, 142, 81, 10, 0, 48, 8, 66, 107, 236, 254, 87, 110, 106, 35, 172, 143, 74, 243, 65, 89, 85, 129, 202, 100, 239, 146, 115, 184, 183, 11, 109, 33, 29, 126, 114, 141, 75, 213, 65, 44, 131, 70, 24, 97, 46, 50, 34, 72, 25, 39, 181, 9, 251, 205, 14, 10, 78, 123, 43, 35, 17, 17, 228, 109, 164, 219, 0, 0, 0, 0, 73, 69, 78, 68, 174, 66, 96, 130, };
    /// <summary>
    /// The easy way: compare to a saved version of the question mark image, expressed here as an array of bytes.
    /// </summary>
    /// <param name="tex">
    /// A <see cref="Texture"/>
    /// </param>
    /// <returns>
    /// A <see cref="System.Boolean"/>
    /// </returns>
    public static bool isBogus(this Texture2D tex)
    {
        if (tex == null)
            return true;

        return isBogus(tex.EncodeToPNG());
    }

    public static bool isBogus(byte[] png)
    {
        if (png == null)
            return true;

        return Equivalent(png, questionMarkPNG);
    }

    /// <summary>
    /// Compare two byte arrays.
    /// </summary>
    public static bool Equivalent(byte[] bytes1, byte[] bytes2)
    {
        if (bytes1.Length != bytes2.Length)
            return false;
        for (int i = 0; i < bytes1.Length; i++)
            if (!bytes1[i].Equals(bytes2[i]))
                return false;
        return true;
    }
}