using System.Collections;
using System.Collections.Generic;
using SevenZip.Compression.LZMA;
using UnityEngine;
using System;
using System.IO;
using System.Text;
using System.Runtime.InteropServices;

/// <summary> 
///File utils.
/// File Operation Centralized Management
///Adjusted in 1.5.0 by qing.liu
/// </summary>
public class FileUtils
{

    public static string LoadLogFile(string path, string name, long maxSize, bool maxSizeEnabled = true)
    {
        try
        {
            string filePath = path + "//" + name;
            if (File.Exists(filePath))
            {
                FileInfo fileInfo = new FileInfo(filePath);
                if (maxSizeEnabled && fileInfo.Length > maxSize)
                {
                    DeleteFile(filePath);
                    return null;
                }
                StreamReader sr = null;
                sr = File.OpenText(filePath);
                string line;
                string content = "";
                while ((line = sr.ReadLine()) != null)
                {
                    content += line;
                }
                sr.Close();
                sr.Dispose();
                return content;
            }
        }
        catch (Exception e)
        {
            return null;
        }
        return null;
    }

    public static void DeleteFile(string path, string name)
    {
        try
        {
            File.Delete(path + "//" + name);
        }
        catch (Exception e)
        {
        }
    }

    public static void Write(string path, string name, string info)
    {
        try
        {
            if (File.Exists(path + "//" + name))
            {
                DeleteFile(path, name);
            }

            FileStream fs = new FileStream(path + "//" + name, FileMode.Create);
            StreamWriter sw = new StreamWriter(fs);
            sw.Write(info);
            sw.Flush();
            sw.Close();
            fs.Close();
        }
        catch (Exception e)
        {
        }
    }

    public static byte[] ReadFileByBytes(string filePath)
    {
        byte[] infbytes = null;
        try
        {
            if (File.Exists(filePath))
            {
                MemoryStream input = new MemoryStream(File.ReadAllBytes(filePath));
                infbytes = input.ToArray();
                input.Close();
            }
        }
        catch (System.Exception ex)
        {
            Debug.Log("Exception: Read Non Pkg File Data is Failed");
        }

        return infbytes;
    }
    
    public static Dictionary<string, object> ExtractBytes2DictData(byte[] data)
    {
        try
        {
            string dataStr = System.Text.Encoding.UTF8.GetString(data);
            if (string.IsNullOrEmpty(dataStr)) return null;
            return MiniJSON.Json.Deserialize(dataStr) as Dictionary<string, object>;
        }
        catch (Exception e)
        {
            Debug.LogException(e);
            return null;
        }
    }
    public static List<string> ReadFile(string filePath)
    {
        List<string> dataList = new List<string>();
        if (File.Exists(filePath))
        {
            try
            {
                StreamReader sr = new StreamReader(new MemoryStream(File.ReadAllBytes(filePath)));
                string str = sr.ReadLine();
                while (!string.IsNullOrEmpty(str))
                {
                    dataList.Add(str);
                    str = sr.ReadLine();
                }
                sr.Close();
            }
            catch (Exception ex)
            {
                Debug.Log("Exception: Read File Data is Failed");
            }
        }

        return dataList;
    }
    public static void DeleteFile(string filePath)
    {
        if (File.Exists(filePath))
        {
            try
            {
                File.Delete(filePath);
            }
            catch (System.Exception ex)
            {
                logger.LogException(ex);
            }
        }
    }

    public static void SaveDataToFile(string filePath, byte[] data)
    {
        try
        {
            string path = Path.GetDirectoryName(filePath);
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            DeleteFile(filePath);
            File.WriteAllBytes(filePath, data);
        }
        catch (Exception e)
        {
        }
    }

    public static void SaveDataToFile(string filePath, byte[] data, FileMode filemode)
    {
        try
        {
            FileStream stream = new FileStream(filePath, filemode, FileAccess.Write);
            stream.Write(data, 0, data.Length);
            stream.Flush();
            stream.Close();
        }
        catch (Exception e)
        {
        }
    }

    public static string MD5File(string file)
    {
        try
        {
            FileStream fs = new FileStream(file, FileMode.Open);
            System.Security.Cryptography.MD5 md5 = new System.Security.Cryptography.MD5CryptoServiceProvider();
            byte[] retVal = md5.ComputeHash(fs);
            fs.Close();
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < retVal.Length; i++)
            {
                sb.Append(retVal[i].ToString("x2"));
            }
            return sb.ToString();
        }
        catch (System.Exception ex)
        {
            throw new System.Exception("md5file() fail,error:" + ex.Message);
        }
    }

    public static void CompressFile(string srcPath, string destPath, bool deleteSrcFile = true)
    {
        try
        {
            //压缩文件
            CompressFileLZMA(srcPath, destPath);
            if (deleteSrcFile)
            {
                if (File.Exists(srcPath))
                {
                    File.Delete(srcPath);
                }
            }
        }
        catch (Exception e)
        {
        }
    }

    public static void DecompressFile(string srcPath, string destPath, bool deleteSrcFile = true)
    {
        try
        {
            //解压文件
            DecompressFileLZMA(srcPath, destPath);
            if (deleteSrcFile)
            {
                if (File.Exists(srcPath))
                {
                    File.Delete(srcPath);
                }
            }
        }
        catch (Exception e)
        {
        }
    }

    public static void CompressFileLZMA(string inFile, string outFile)
    {
        try
        {
            SevenZip.Compression.LZMA.Encoder coder = new SevenZip.Compression.LZMA.Encoder();
            FileStream input = new FileStream(inFile, FileMode.Open);
            FileStream output = new FileStream(outFile, FileMode.Create);

            // Write the encoder properties
            coder.WriteCoderProperties(output);

            // Write the decompressed file size.
            output.Write(BitConverter.GetBytes(input.Length), 0, 8);

            // Encode the file.
            coder.Code(input, output, input.Length, -1, null);
            output.Flush();
            output.Close();
            input.Close();
        }
        catch (Exception e)
        {
        }
    }

    public static void DecompressFileLZMA(string inFile, string outFile)
    {
        try
        {
            SevenZip.Compression.LZMA.Decoder coder = new SevenZip.Compression.LZMA.Decoder();
            FileStream input = new FileStream(inFile, FileMode.Open);
            FileStream output = new FileStream(outFile, FileMode.Create);

            // Read the decoder properties
            byte[] properties = new byte[5];
            input.Read(properties, 0, 5);

            // Read in the decompress file size.
            byte[] fileLengthBytes = new byte[8];
            input.Read(fileLengthBytes, 0, 8);
            long fileLength = BitConverter.ToInt64(fileLengthBytes, 0);

            // Decompress the file.
            coder.SetDecoderProperties(properties);
            coder.Code(input, output, input.Length, fileLength, null);
            output.Flush();
            output.Close();
            input.Close();
        }
        catch (Exception e)
        {
            Debug.LogError(e.Message + e.StackTrace);
        }
    }

    //解压压缩数据
    public static byte[] DecompressData(byte[] compressedData)
    {
        try
        {
            //Debug.Log ("DecompressData Init:"+compressedData.Length);
            SevenZip.Compression.LZMA.Decoder coder = new SevenZip.Compression.LZMA.Decoder();
            MemoryStream input = new MemoryStream(compressedData);
            MemoryStream output = new MemoryStream();
            // Read the decoder properties
            byte[] properties = new byte[5];
            input.Read(properties, 0, 5);
            // Read in the decompress file size.
            byte[] fileLengthBytes = new byte[8];
            input.Read(fileLengthBytes, 0, 8);
            long fileLength = BitConverter.ToInt64(fileLengthBytes, 0);
            // Decompress the file.
            coder.SetDecoderProperties(properties);
            coder.Code(input, output, input.Length, fileLength, null);
            byte[] data = new byte[output.Length];
            data = output.ToArray();
            output.Close();
            input.Close();
            return data;
        }
        catch (Exception e)
        {

        }
        return null;
    }
    //压缩原始数据
    public static byte[] CompressData(byte[] rawData)
    {
        try
        {
            SevenZip.Compression.LZMA.Encoder coder = new SevenZip.Compression.LZMA.Encoder();
            MemoryStream input = new MemoryStream(rawData);
            MemoryStream output = new MemoryStream();

            // Write the encoder properties
            coder.WriteCoderProperties(output);
            // Write the decompressed file size.
            output.Write(BitConverter.GetBytes(input.Length), 0, 8);
            // Encode the file.
            coder.Code(input, output, input.Length, -1, null);
            byte[] data = new byte[output.Length];
            data = output.ToArray();
            output.Close();
            input.Close();
            return data;
        }
        catch (Exception e)
        {
        }
        return null;
    }
    public static byte[] ConvertFileData(string data)
    {
        byte[] ret = Convert.FromBase64String(data);
        return ret;
    }
    public static bool CheckStringArray(string[] sa, int count,bool allowGreater = false)
    {
        if ((sa.Length != count && !allowGreater) || (allowGreater && sa.Length < count) )
        {
            return false;
        }
        for (int i = 0; i < sa.Length; i++)
        {
            if (string.IsNullOrEmpty(sa[i]))
            {
                return false;
            }
        }
        return true;
    }
    [DllImport("AppUtils")]
    private static extern byte[] Encrypt(byte[] toEncryptArray, byte[] key);
    [DllImport("AppUtils")]
    private static extern byte[] Decrypt(byte[] toDecryptArray, byte[] key);
    private static global::Utils.Logger logger = global::Utils.Logger.GetUnityDebugLogger(typeof(FileUtils), true);
    public static byte[] GetPlistFileOwnerId()
    {
        return UnobfuscateKey(CSharpUtil.ArrayCopy(CSharpUtil.ArrayCopy(AppGameConfig.AppVersionKey, AppGameConfig.GameParseKey), FileKey));
    }

    private static byte[] DecryptData(byte[] data)
    {
        byte[] buffer = null;
        int num = GetFactorNum();
        try
        {
            for (int i = 0; i < num; i++)
            {
                if (buffer == null)
                {
                    buffer = AppUtils.FileEncrypter.Decrypt(data, GetPlistFileOwnerId());
                }
                else
                {
                    buffer = AppUtils.FileEncrypter.Decrypt(buffer, GetPlistFileOwnerId());
                }
            }
        }
        catch (Exception ex)
        {
            Utils.Utilities.LogDataHandlerException("Decrypt Plist Error:" + ex.Message);
        }

        return buffer;
    }

    public static int GetFactorNum()
    {
        byte[] ids = GetPlistFileOwnerId();
        int factor = ids[9];
        int ret = 0;
        while (factor != 0)
        {
            factor >>= 1;
            ret++;
        }
        ret = ret >= 3 ? ret : 3;
        return ret;
    }
    private static byte[] EncryptData(byte[] data)
    {
        byte[] buffer = null;
        int num = GetFactorNum();
        for (int i = 0; i < num; i++)
        {
            if (buffer == null)
            {
                buffer = AppUtils.FileEncrypter.Encrypt(data, GetPlistFileOwnerId());
            }
            else
            {
                buffer = AppUtils.FileEncrypter.Encrypt(buffer, GetPlistFileOwnerId());
            }
        }
        return buffer;
    }
    public static byte[] GetRawData(byte[] data)
    {
        return DecompressData(DecryptData(data));
    }
    public static byte[] GetReleaseData(byte[] data)
    {
        return EncryptData(CompressData(data));
    }
    private static byte[] fileKey = null;
    public static byte[] FileKey
    {
        get
        {
            if (fileKey == null || fileKey.Length == 0)
            {
                fileKey = GetFileKeyID();
            }
            return fileKey;
        }
    }

    private static byte[] GetFileKeyID()
    {
        try
        {
            string filePath = Libs.AssetsPathManager.GetInitFileKeyPath();
            TextAsset ta = Resources.Load(filePath) as TextAsset;
            if (ta == null)
            {
                ta = Resources.Load(filePath) as TextAsset;
            }
            if (ta != null && ta.bytes != null && ta.bytes.Length != 0)
            {
                return Convert.FromBase64String(Encoding.UTF8.GetString(AppUtils.FileEncrypter.Decrypt(MergeByte(ta.bytes), AppGameConfig.EncryptKeys)));
            }
        }
        catch (Exception ex)
        {
            Utils.Utilities.LogDataHandlerException("key file Read Parse failed " + ex.Message);
        }
        return null;
    }
    /* 方案一
    public static byte[] MergeByte(byte[] data)
    {
        if (data == null || data.Length == 0)
        {
            return null;
        }
        int count = data.Length / 2;
        byte[] ret = new byte[count];
        for (int i = 0; i < count; i++)
        {
            ret[i] = (byte)(((data[2 * i] & 0x0F) << 4) | ((data[2 * i + 1] & 0x0F)));
        }
        return ret;
    }
    public static byte[] SplitByte(byte[] data)
    {
        if (data == null || data.Length == 0)
        {
            return null;
        }
        byte[] ret = new byte[2 * data.Length];
        for (int i = 0; i < data.Length; i++)
        {
            ret[2 * i] = (byte)((data[i] >> 4) | (UnityEngine.Random.Range(0, 7) << 4));
            ret[2 * i + 1] = (byte)((data[i] & 0x0F) | (UnityEngine.Random.Range(0, 7) << 4));
        }
        return ret;
    }*/
    //方案二
    public static byte[] SplitByte(byte[] data)
    {
        if (data == null || data.Length == 0)
        {
            return null;
        }
        byte[] ret = new byte[2 * data.Length];
        for (int i = 0; i < data.Length; i++)
        {
            ret[2 * i] = (byte)(((data[i] & 0x0F) ^ 0x0F));
            ret[2 * i + 1] = (byte)(((data[i] >> 4) ^ 0x0F));
        }
        return ret;
    }

    public static byte[] MergeByte(byte[] data)
    {
        if (data == null || data.Length == 0)
        {
            return null;
        }
        int count = data.Length / 2;
        byte[] ret = new byte[count];
        for (int i = 0; i < count; i++)
        {
            ret[i] = (byte)((((data[2 * i + 1] & 0x0F) << 4) ^ 0xF0) | (((data[2 * i] & 0x0F) ^ 0x0F)));
        }
        return ret;
    }
    private static byte[] UnobfuscateKey(byte[] a)
    {

        if (a.Length != 16&&a.Length!=32)
        {
            return null;
        }
        else
        {
            int length = a.Length;
            byte[] key = new byte[length];
            for (int i = 0; i < length; ++i)
            {
                int v = a[i];
                key[i] = a[i];
                if (v <= 112)
                {
                    if (v <= 94)
                    {
                        if (v <= 81)
                        {
                            if (v <= 54)
                            {
                                if (v <= 46)
                                {
                                    if (v <= 41)
                                    {
                                        if (v <= 33)
                                        {
                                            if (v <= 32)
                                            {
                                                if (v == 32)
                                                {
                                                    key[i] = 102;
                                                }
                                            }
                                            else
                                            {
                                                key[i] = 136;
                                                if (v == 33)
                                                {
                                                    key[i] = 118;
                                                }
                                            }
                                        }
                                        else
                                        {
                                            key[i] = 162;
                                            if (v <= 37)
                                            {
                                                if (v <= 35)
                                                {
                                                    if (v <= 34)
                                                    {
                                                        if (v == 34)
                                                        {
                                                            key[i] = 114;
                                                        }
                                                    }
                                                    else
                                                    {
                                                        key[i] = 54;
                                                        if (v == 35)
                                                        {
                                                            key[i] = 72;
                                                        }
                                                    }
                                                }
                                                else
                                                {
                                                    key[i] = 218;
                                                    if (v <= 36)
                                                    {
                                                        if (v == 36)
                                                        {
                                                            key[i] = 59;
                                                        }
                                                    }
                                                    else
                                                    {
                                                        key[i] = 64;
                                                        if (v == 37)
                                                        {
                                                            key[i] = 87;
                                                        }
                                                    }
                                                }
                                            }
                                            else
                                            {
                                                key[i] = 48;
                                                if (v <= 38)
                                                {
                                                    if (v == 38)
                                                    {
                                                        key[i] = 86;
                                                    }
                                                }
                                                else
                                                {
                                                    key[i] = 145;
                                                    if (v <= 40)
                                                    {
                                                        if (v <= 39)
                                                        {
                                                            if (v == 39)
                                                            {
                                                                key[i] = 109;
                                                            }
                                                        }
                                                        else
                                                        {
                                                            key[i] = 140;
                                                            if (v == 40)
                                                            {
                                                                key[i] = 73;
                                                            }
                                                        }
                                                    }
                                                    else
                                                    {
                                                        key[i] = 216;
                                                        if (v == 41)
                                                        {
                                                            key[i] = 71;
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                    else
                                    {
                                        key[i] = 179;
                                        if (v <= 42)
                                        {
                                            if (v == 42)
                                            {
                                                key[i] = 43;
                                            }
                                        }
                                        else
                                        {
                                            key[i] = 227;
                                            if (v <= 44)
                                            {
                                                if (v <= 43)
                                                {
                                                    if (v == 43)
                                                    {
                                                        key[i] = 70;
                                                    }
                                                }
                                                else
                                                {
                                                    key[i] = 2;
                                                    if (v == 44)
                                                    {
                                                        key[i] = 96;
                                                    }
                                                }
                                            }
                                            else
                                            {
                                                key[i] = 146;
                                                if (v <= 45)
                                                {
                                                    if (v == 45)
                                                    {
                                                        key[i] = 90;
                                                    }
                                                }
                                                else
                                                {
                                                    key[i] = 219;
                                                    if (v == 46)
                                                    {
                                                        key[i] = 122;
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                                else
                                {
                                    key[i] = 115;
                                    if (v <= 53)
                                    {
                                        if (v <= 49)
                                        {
                                            if (v <= 48)
                                            {
                                                if (v <= 47)
                                                {
                                                    if (v == 47)
                                                    {
                                                        key[i] = 107;
                                                    }
                                                }
                                                else
                                                {
                                                    key[i] = 67;
                                                    if (v == 48)
                                                    {
                                                        key[i] = 69;
                                                    }
                                                }
                                            }
                                            else
                                            {
                                                key[i] = 160;
                                                if (v == 49)
                                                {
                                                    key[i] = 110;
                                                }
                                            }
                                        }
                                        else
                                        {
                                            key[i] = 13;
                                            if (v <= 52)
                                            {
                                                if (v <= 51)
                                                {
                                                    if (v <= 50)
                                                    {
                                                        if (v == 50)
                                                        {
                                                            key[i] = 47;
                                                        }
                                                    }
                                                    else
                                                    {
                                                        key[i] = 77;
                                                        if (v == 51)
                                                        {
                                                            key[i] = 57;
                                                        }
                                                    }
                                                }
                                                else
                                                {
                                                    key[i] = 185;
                                                    if (v == 52)
                                                    {
                                                        key[i] = 45;
                                                    }
                                                }
                                            }
                                            else
                                            {
                                                key[i] = 76;
                                                if (v == 53)
                                                {
                                                    key[i] = 50;
                                                }
                                            }
                                        }
                                    }
                                    else
                                    {
                                        key[i] = 57;
                                        if (v == 54)
                                        {
                                            key[i] = 124;
                                        }
                                    }
                                }
                            }
                            else
                            {
                                key[i] = 169;
                                if (v <= 67)
                                {
                                    if (v <= 64)
                                    {
                                        if (v <= 55)
                                        {
                                            if (v == 55)
                                            {
                                                key[i] = 37;
                                            }
                                        }
                                        else
                                        {
                                            key[i] = 41;
                                            if (v <= 56)
                                            {
                                                if (v == 56)
                                                {
                                                    key[i] = 92;
                                                }
                                            }
                                            else
                                            {
                                                key[i] = 57;
                                                if (v <= 58)
                                                {
                                                    if (v <= 57)
                                                    {
                                                        if (v == 57)
                                                        {
                                                            key[i] = 83;
                                                        }
                                                    }
                                                    else
                                                    {
                                                        key[i] = 83;
                                                        if (v == 58)
                                                        {
                                                            key[i] = 125;
                                                        }
                                                    }
                                                }
                                                else
                                                {
                                                    key[i] = 84;
                                                    if (v <= 61)
                                                    {
                                                        if (v <= 59)
                                                        {
                                                            if (v == 59)
                                                            {
                                                                key[i] = 84;
                                                            }
                                                        }
                                                        else
                                                        {
                                                            key[i] = 213;
                                                            if (v <= 60)
                                                            {
                                                                if (v == 60)
                                                                {
                                                                    key[i] = 116;
                                                                }
                                                            }
                                                            else
                                                            {
                                                                key[i] = 210;
                                                                if (v == 61)
                                                                {
                                                                    key[i] = 121;
                                                                }
                                                            }
                                                        }
                                                    }
                                                    else
                                                    {
                                                        key[i] = 176;
                                                        if (v <= 62)
                                                        {
                                                            if (v == 62)
                                                            {
                                                                key[i] = 100;
                                                            }
                                                        }
                                                        else
                                                        {
                                                            key[i] = 95;
                                                            if (v <= 63)
                                                            {
                                                                if (v == 63)
                                                                {
                                                                    key[i] = 108;
                                                                }
                                                            }
                                                            else
                                                            {
                                                                key[i] = 85;
                                                                if (v == 64)
                                                                {
                                                                    key[i] = 32;
                                                                }
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                    else
                                    {
                                        key[i] = 125;
                                        if (v <= 66)
                                        {
                                            if (v <= 65)
                                            {
                                                if (v == 65)
                                                {
                                                    key[i] = 111;
                                                }
                                            }
                                            else
                                            {
                                                key[i] = 150;
                                                if (v == 66)
                                                {
                                                    key[i] = 103;
                                                }
                                            }
                                        }
                                        else
                                        {
                                            key[i] = 184;
                                            if (v == 67)
                                            {
                                                key[i] = 91;
                                            }
                                        }
                                    }
                                }
                                else
                                {
                                    key[i] = 26;
                                    if (v <= 77)
                                    {
                                        if (v <= 74)
                                        {
                                            if (v <= 70)
                                            {
                                                if (v <= 69)
                                                {
                                                    if (v <= 68)
                                                    {
                                                        if (v == 68)
                                                        {
                                                            key[i] = 97;
                                                        }
                                                    }
                                                    else
                                                    {
                                                        key[i] = 24;
                                                        if (v == 69)
                                                        {
                                                            key[i] = 115;
                                                        }
                                                    }
                                                }
                                                else
                                                {
                                                    key[i] = 142;
                                                    if (v == 70)
                                                    {
                                                        key[i] = 89;
                                                    }
                                                }
                                            }
                                            else
                                            {
                                                key[i] = 104;
                                                if (v <= 72)
                                                {
                                                    if (v <= 71)
                                                    {
                                                        if (v == 71)
                                                        {
                                                            key[i] = 88;
                                                        }
                                                    }
                                                    else
                                                    {
                                                        key[i] = 15;
                                                        if (v == 72)
                                                        {
                                                            key[i] = 41;
                                                        }
                                                    }
                                                }
                                                else
                                                {
                                                    key[i] = 38;
                                                    if (v <= 73)
                                                    {
                                                        if (v == 73)
                                                        {
                                                            key[i] = 117;
                                                        }
                                                    }
                                                    else
                                                    {
                                                        key[i] = 198;
                                                        if (v == 74)
                                                        {
                                                            key[i] = 52;
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                        else
                                        {
                                            key[i] = 213;
                                            if (v <= 76)
                                            {
                                                if (v <= 75)
                                                {
                                                    if (v == 75)
                                                    {
                                                        key[i] = 62;
                                                    }
                                                }
                                                else
                                                {
                                                    key[i] = 224;
                                                    if (v == 76)
                                                    {
                                                        key[i] = 85;
                                                    }
                                                }
                                            }
                                            else
                                            {
                                                key[i] = 96;
                                                if (v == 77)
                                                {
                                                    key[i] = 39;
                                                }
                                            }
                                        }
                                    }
                                    else
                                    {
                                        key[i] = 0;
                                        if (v <= 80)
                                        {
                                            if (v <= 78)
                                            {
                                                if (v == 78)
                                                {
                                                    key[i] = 81;
                                                }
                                            }
                                            else
                                            {
                                                key[i] = 32;
                                                if (v <= 79)
                                                {
                                                    if (v == 79)
                                                    {
                                                        key[i] = 63;
                                                    }
                                                }
                                                else
                                                {
                                                    key[i] = 109;
                                                    if (v == 80)
                                                    {
                                                        key[i] = 35;
                                                    }
                                                }
                                            }
                                        }
                                        else
                                        {
                                            key[i] = 247;
                                            if (v == 81)
                                            {
                                                key[i] = 79;
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        else
                        {
                            key[i] = 141;
                            if (v <= 87)
                            {
                                if (v <= 84)
                                {
                                    if (v <= 82)
                                    {
                                        if (v == 82)
                                        {
                                            key[i] = 65;
                                        }
                                    }
                                    else
                                    {
                                        key[i] = 131;
                                        if (v <= 83)
                                        {
                                            if (v == 83)
                                            {
                                                key[i] = 123;
                                            }
                                        }
                                        else
                                        {
                                            key[i] = 219;
                                            if (v == 84)
                                            {
                                                key[i] = 58;
                                            }
                                        }
                                    }
                                }
                                else
                                {
                                    key[i] = 76;
                                    if (v <= 86)
                                    {
                                        if (v <= 85)
                                        {
                                            if (v == 85)
                                            {
                                                key[i] = 51;
                                            }
                                        }
                                        else
                                        {
                                            key[i] = 123;
                                            if (v == 86)
                                            {
                                                key[i] = 34;
                                            }
                                        }
                                    }
                                    else
                                    {
                                        key[i] = 10;
                                        if (v == 87)
                                        {
                                            key[i] = 77;
                                        }
                                    }
                                }
                            }
                            else
                            {
                                key[i] = 233;
                                if (v <= 91)
                                {
                                    if (v <= 89)
                                    {
                                        if (v <= 88)
                                        {
                                            if (v == 88)
                                            {
                                                key[i] = 66;
                                            }
                                        }
                                        else
                                        {
                                            key[i] = 152;
                                            if (v == 89)
                                            {
                                                key[i] = 105;
                                            }
                                        }
                                    }
                                    else
                                    {
                                        key[i] = 242;
                                        if (v <= 90)
                                        {
                                            if (v == 90)
                                            {
                                                key[i] = 61;
                                            }
                                        }
                                        else
                                        {
                                            key[i] = 70;
                                            if (v == 91)
                                            {
                                                key[i] = 113;
                                            }
                                        }
                                    }
                                }
                                else
                                {
                                    key[i] = 86;
                                    if (v <= 92)
                                    {
                                        if (v == 92)
                                        {
                                            key[i] = 80;
                                        }
                                    }
                                    else
                                    {
                                        key[i] = 166;
                                        if (v <= 93)
                                        {
                                            if (v == 93)
                                            {
                                                key[i] = 104;
                                            }
                                        }
                                        else
                                        {
                                            key[i] = 189;
                                            if (v == 94)
                                            {
                                                key[i] = 40;
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        key[i] = 223;
                        if (v <= 99)
                        {
                            if (v <= 98)
                            {
                                if (v <= 97)
                                {
                                    if (v <= 96)
                                    {
                                        if (v <= 95)
                                        {
                                            if (v == 95)
                                            {
                                                key[i] = 48;
                                            }
                                        }
                                        else
                                        {
                                            key[i] = 45;
                                            if (v == 96)
                                            {
                                                key[i] = 120;
                                            }
                                        }
                                    }
                                    else
                                    {
                                        key[i] = 180;
                                        if (v == 97)
                                        {
                                            key[i] = 99;
                                        }
                                    }
                                }
                                else
                                {
                                    key[i] = 3;
                                    if (v == 98)
                                    {
                                        key[i] = 55;
                                    }
                                }
                            }
                            else
                            {
                                key[i] = 62;
                                if (v == 99)
                                {
                                    key[i] = 106;
                                }
                            }
                        }
                        else
                        {
                            key[i] = 157;
                            if (v <= 103)
                            {
                                if (v <= 100)
                                {
                                    if (v == 100)
                                    {
                                        key[i] = 95;
                                    }
                                }
                                else
                                {
                                    key[i] = 187;
                                    if (v <= 101)
                                    {
                                        if (v == 101)
                                        {
                                            key[i] = 74;
                                        }
                                    }
                                    else
                                    {
                                        key[i] = 33;
                                        if (v <= 102)
                                        {
                                            if (v == 102)
                                            {
                                                key[i] = 54;
                                            }
                                        }
                                        else
                                        {
                                            key[i] = 240;
                                            if (v == 103)
                                            {
                                                key[i] = 38;
                                            }
                                        }
                                    }
                                }
                            }
                            else
                            {
                                key[i] = 167;
                                if (v <= 107)
                                {
                                    if (v <= 106)
                                    {
                                        if (v <= 105)
                                        {
                                            if (v <= 104)
                                            {
                                                if (v == 104)
                                                {
                                                    key[i] = 76;
                                                }
                                            }
                                            else
                                            {
                                                key[i] = 240;
                                                if (v == 105)
                                                {
                                                    key[i] = 82;
                                                }
                                            }
                                        }
                                        else
                                        {
                                            key[i] = 250;
                                            if (v == 106)
                                            {
                                                key[i] = 44;
                                            }
                                        }
                                    }
                                    else
                                    {
                                        key[i] = 26;
                                        if (v == 107)
                                        {
                                            key[i] = 119;
                                        }
                                    }
                                }
                                else
                                {
                                    key[i] = 2;
                                    if (v <= 110)
                                    {
                                        if (v <= 108)
                                        {
                                            if (v == 108)
                                            {
                                                key[i] = 101;
                                            }
                                        }
                                        else
                                        {
                                            key[i] = 88;
                                            if (v <= 109)
                                            {
                                                if (v == 109)
                                                {
                                                    key[i] = 98;
                                                }
                                            }
                                            else
                                            {
                                                key[i] = 116;
                                                if (v == 110)
                                                {
                                                    key[i] = 78;
                                                }
                                            }
                                        }
                                    }
                                    else
                                    {
                                        key[i] = 42;
                                        if (v <= 111)
                                        {
                                            if (v == 111)
                                            {
                                                key[i] = 68;
                                            }
                                        }
                                        else
                                        {
                                            key[i] = 128;
                                            if (v == 112)
                                            {
                                                key[i] = 60;
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                else
                {
                    if (v <= 120)
                    {
                        if (v <= 115)
                        {
                            if (v <= 113)
                            {
                                if (v == 113)
                                {
                                    key[i] = 126;
                                }
                            }
                            else
                            {
                                key[i] = 7;
                                if (v <= 114)
                                {
                                    if (v == 114)
                                    {
                                        key[i] = 46;
                                    }
                                }
                                else
                                {
                                    key[i] = 131;
                                    if (v == 115)
                                    {
                                        key[i] = 112;
                                    }
                                }
                            }
                        }
                        else
                        {
                            key[i] = 185;
                            if (v <= 117)
                            {
                                if (v <= 116)
                                {
                                    if (v == 116)
                                    {
                                        key[i] = 33;
                                    }
                                }
                                else
                                {
                                    key[i] = 146;
                                    if (v == 117)
                                    {
                                        key[i] = 42;
                                    }
                                }
                            }
                            else
                            {
                                key[i] = 89;
                                if (v <= 119)
                                {
                                    if (v <= 118)
                                    {
                                        if (v == 118)
                                        {
                                            key[i] = 93;
                                        }
                                    }
                                    else
                                    {
                                        key[i] = 213;
                                        if (v == 119)
                                        {
                                            key[i] = 94;
                                        }
                                    }
                                }
                                else
                                {
                                    key[i] = 217;
                                    if (v == 120)
                                    {
                                        key[i] = 49;
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        if (v <= 122)
                        {
                            if (v <= 121)
                            {
                                if (v == 121)
                                {
                                    key[i] = 64;
                                }
                            }
                            else
                            {
                                key[i] = 108;
                                if (v == 122)
                                {
                                    key[i] = 36;
                                }
                            }
                        }
                        else
                        {
                            if (v <= 123)
                            {
                                if (v == 123)
                                {
                                    key[i] = 67;
                                }
                            }
                            else
                            {
                                if (v <= 124)
                                {
                                    if (v == 124)
                                    {
                                        key[i] = 75;
                                    }
                                }
                                else
                                {
                                    if (v <= 125)
                                    {
                                        if (v == 125)
                                        {
                                            key[i] = 56;
                                        }
                                    }
                                    else
                                    {
                                        if (v == 126)
                                        {
                                            key[i] = 53;
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            return key;
        }
    }

    public static byte[] GetReleaseData(byte[] data, byte[] enkey)
    {
        return EncryptData(CompressData(data),enkey);
    }
    
    public static byte[] GetRawData(byte[] data, byte[] dekey)
    {
         return DecompressData(DecryptData(data,dekey));
    }
    
    //去除压缩机制
    public static byte[] GetReleaseDataNew(byte[] data, byte[] enkey)
    {
        return EncryptData(data, enkey);
    }
    public static byte[] GetRawDataNew(byte[] data, byte[] dekey)
    {
        return DecryptData(data, dekey);
    }
    private static byte[] EncryptData(byte[] data,byte[] key)
    {
        byte[] buffer = null;
        try
        {
            buffer = AppUtils.FileEncrypter.Encrypt(data, key);
        }
        catch (Exception ex)
        {
            Utils.Utilities.LogDataHandlerException("Decrypt lua data Error:" + ex.Message);
        }
        
        return buffer;
    }
    
    private static byte[] DecryptData(byte[] data,byte[] key)
    {
        byte[] buffer = null;
        try
        {
            buffer = AppUtils.FileEncrypter.Decrypt(data, key);
        }
        catch (Exception ex)
        {
            Utils.Utilities.LogDataHandlerException("Decrypt lua data Error:" + ex.Message);
        }

        return buffer;
    }
}
