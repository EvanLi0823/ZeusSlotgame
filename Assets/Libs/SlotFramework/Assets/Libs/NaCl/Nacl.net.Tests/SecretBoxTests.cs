using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Security.Cryptography;
using System.Text;
using nacl;

namespace nacl
{
	public class SecretBoxTests
	{
		public void Easy ()
		{
			byte[] cipher = new byte[32 + 16];
			byte[] decoded = new byte[32];
			byte[] message = new byte[32];

			byte[] nonce = new byte[24];
			byte[] key = new byte[32];

			SecretBox secretBox = new SecretBox (key);

			secretBox.EasyBox (cipher, message, nonce);

			secretBox.EaseOpen (decoded, cipher, nonce);
		}

		public void SecretBox2 (byte[] firstkey)
		{
			byte[] nonce = {
				0x4a, 0x59, 0x3d, 0xaf, 0x29, 0xc8, 0x97, 0xba, 0x87, 0x1a, 0x94, 0x98,
				0xe6, 0xb2, 0xf4, 0xf8, 0x7b, 0x2e, 0xd4, 0xb6, 0x67, 0x0, 0x3a, 0xb9
			};
      
			SecretBox box = new SecretBox (firstkey);

			string plaintext = "a";
			byte[] m = System.Text.Encoding.UTF8.GetBytes (plaintext);
			byte[] c = new byte[m.Length + 16];
			box.EasyBox (c, m, nonce);
//			box.Open
			for (int i = 0; i < c.Length; ++i) {
				UnityEngine.Debug.Log (",0x" + c [i].ToString ("X"));
			}

			box.EaseOpen (m, c, nonce);
			UnityEngine.Debug.Log ("bbbbb ....  " + System.Text.Encoding.UTF8.GetString (m));
		}


		public static void LogBytes(byte[] bytes){
			String temp = "";
			for (int i = 0; i < bytes.Length; ++i) {
				temp += ("0x" + bytes [i].ToString ("X"));
				if (i < bytes.Length - 1) {
					temp +=",";
				}
			}
			UnityEngine.Debug.Log (temp);
		}

			
	}

}
