using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text;
using System.IO;
using System;
using System.Security.Cryptography;

public class EncryptUtil {
	public static string AesEncrypt(string str, string key)
	{
		if (string.IsNullOrEmpty(str)) return null;
		Byte[] toEncryptArray = Encoding.UTF8.GetBytes(str);

		System.Security.Cryptography.RijndaelManaged rm = new System.Security.Cryptography.RijndaelManaged
		{
			Key = Encoding.UTF8.GetBytes(key),
			Mode = System.Security.Cryptography.CipherMode.ECB,
			Padding = System.Security.Cryptography.PaddingMode.PKCS7
		};

		System.Security.Cryptography.ICryptoTransform cTransform = rm.CreateEncryptor();
		Byte[] resultArray = cTransform.TransformFinalBlock(toEncryptArray, 0, toEncryptArray.Length);
		return Convert.ToBase64String(resultArray, 0, resultArray.Length);
	}

	public static string CreateToken(string contentString,string hashKey)
	{
		var encoding = new System.Text.ASCIIEncoding();
		byte[] keyByte = encoding.GetBytes(hashKey);
		byte[] messageBytes = encoding.GetBytes(contentString);
		using (var hmacsha256 = new HMACSHA256(keyByte))
		{
			StringBuilder sb = new StringBuilder ();
			byte[] hashmessage = hmacsha256.ComputeHash(messageBytes);
			return byteToHexStr(hashmessage).ToLower();
		}
	}

	public static string byteToHexStr(byte[] bytes)
	{
		string returnStr = "";
		if (bytes != null)
		{
			for (int i = 0; i < bytes.Length; i++)
			{
				returnStr += bytes[i].ToString("X2");
			}
		}
		return returnStr;
	}

	public static string unobfuscateString (string s)
	{
		if (s.Length != 32) {
			return null;
		} else {
			char[] key = new char[33];
			key [32] = (char) 0;

			for (int i = 0; i < 32; ++i) {
				char v = s [i];
				if (v <= 112) {
					if (v <= 94) {
						if (v <= 81) {
							if (v <= 54) {
								if (v <= 46) {
									if (v <= 41) {
										if (v <= 33) {
											if (v <= 32) {
												if (v == 32) {
													key [i] = (char)102;
												}
											} else {
												key [i] = (char)136;
												if (v == 33) {
													key [i] = (char)118;
												}
											}
										} else {
											key [i] = (char)162;
											if (v <= 37) {
												if (v <= 35) {
													if (v <= 34) {
														if (v == 34) {
															key [i] = (char)114;
														}
													} else {
														key [i] = (char)54;
														if (v == 35) {
															key [i] = (char)72;
														}
													}
												} else {
													key [i] = (char)218;
													if (v <= 36) {
														if (v == 36) {
															key [i] = (char)59;
														}
													} else {
														key [i] = (char)64;
														if (v == 37) {
															key [i] = (char)87;
														}
													}
												}
											} else {
												key [i] = (char)48;
												if (v <= 38) {
													if (v == 38) {
														key [i] = (char)86;
													}
												} else {
													key [i] = (char)145;
													if (v <= 40) {
														if (v <= 39) {
															if (v == 39) {
																key [i] = (char)109;
															}
														} else {
															key [i] = (char)140;
															if (v == 40) {
																key [i] = (char)73;
															}
														}
													} else {
														key [i] = (char)216;
														if (v == 41) {
															key [i] = (char)71;
														}
													}
												}
											}
										}
									} else {
										key [i] = (char)179;
										if (v <= 42) {
											if (v == 42) {
												key [i] = (char)43;
											}
										} else {
											key [i] = (char)227;
											if (v <= 44) {
												if (v <= 43) {
													if (v == 43) {
														key [i] = (char)70;
													}
												} else {
													key [i] = (char)2;
													if (v == 44) {
														key [i] = (char)96;
													}
												}
											} else {
												key [i] = (char)146;
												if (v <= 45) {
													if (v == 45) {
														key [i] = (char)90;
													}
												} else {
													key [i] = (char)219;
													if (v == 46) {
														key [i] = (char)122;
													}
												}
											}
										}
									}
								} else {
									key [i] = (char)115;
									if (v <= 53) {
										if (v <= 49) {
											if (v <= 48) {
												if (v <= 47) {
													if (v == 47) {
														key [i] = (char)107;
													}
												} else {
													key [i] = (char)67;
													if (v == 48) {
														key [i] = (char)69;
													}
												}
											} else {
												key [i] = (char)160;
												if (v == 49) {
													key [i] = (char)110;
												}
											}
										} else {
											key [i] = (char)13;
											if (v <= 52) {
												if (v <= 51) {
													if (v <= 50) {
														if (v == 50) {
															key [i] = (char)47;
														}
													} else {
														key [i] = (char)77;
														if (v == 51) {
															key [i] = (char)57;
														}
													}
												} else {
													key [i] = (char)185;
													if (v == 52) {
														key [i] = (char)45;
													}
												}
											} else {
												key [i] = (char)76;
												if (v == 53) {
													key [i] = (char)50;
												}
											}
										}
									} else {
										key [i] = (char)57;
										if (v == 54) {
											key [i] = (char)124;
										}
									}
								}
							} else {
								key [i] = (char)169;
								if (v <= 67) {
									if (v <= 64) {
										if (v <= 55) {
											if (v == 55) {
												key [i] = (char)37;
											}
										} else {
											key [i] = (char)41;
											if (v <= 56) {
												if (v == 56) {
													key [i] = (char)92;
												}
											} else {
												key [i] = (char)57;
												if (v <= 58) {
													if (v <= 57) {
														if (v == 57) {
															key [i] = (char)83;
														}
													} else {
														key [i] = (char)83;
														if (v == 58) {
															key [i] = (char)125;
														}
													}
												} else {
													key [i] = (char)84;
													if (v <= 61) {
														if (v <= 59) {
															if (v == 59) {
																key [i] = (char)84;
															}
														} else {
															key [i] = (char)213;
															if (v <= 60) {
																if (v == 60) {
																	key [i] = (char)116;
																}
															} else {
																key [i] = (char)210;
																if (v == 61) {
																	key [i] = (char)121;
																}
															}
														}
													} else {
														key [i] = (char)176;
														if (v <= 62) {
															if (v == 62) {
																key [i] = (char)100;
															}
														} else {
															key [i] = (char)95;
															if (v <= 63) {
																if (v == 63) {
																	key [i] = (char)108;
																}
															} else {
																key [i] = (char)85;
																if (v == 64) {
																	key [i] = (char)32;
																}
															}
														}
													}
												}
											}
										}
									} else {
										key [i] = (char)125;
										if (v <= 66) {
											if (v <= 65) {
												if (v == 65) {
													key [i] = (char)111;
												}
											} else {
												key [i] = (char)150;
												if (v == 66) {
													key [i] = (char)103;
												}
											}
										} else {
											key [i] = (char)184;
											if (v == 67) {
												key [i] = (char)91;
											}
										}
									}
								} else {
									key [i] = (char)26;
									if (v <= 77) {
										if (v <= 74) {
											if (v <= 70) {
												if (v <= 69) {
													if (v <= 68) {
														if (v == 68) {
															key [i] = (char)97;
														}
													} else {
														key [i] = (char)24;
														if (v == 69) {
															key [i] = (char)115;
														}
													}
												} else {
													key [i] = (char)142;
													if (v == 70) {
														key [i] = (char)89;
													}
												}
											} else {
												key [i] = (char)104;
												if (v <= 72) {
													if (v <= 71) {
														if (v == 71) {
															key [i] = (char)88;
														}
													} else {
														key [i] = (char)15;
														if (v == 72) {
															key [i] = (char)41;
														}
													}
												} else {
													key [i] = (char)38;
													if (v <= 73) {
														if (v == 73) {
															key [i] = (char)117;
														}
													} else {
														key [i] = (char)198;
														if (v == 74) {
															key [i] = (char)52;
														}
													}
												}
											}
										} else {
											key [i] = (char)213;
											if (v <= (char)76) {
												if (v <= 75) {
													if (v == 75) {
														key [i] = (char)62;
													}
												} else {
													key [i] = (char)224;
													if (v == 76) {
														key [i] = (char)85;
													}
												}
											} else {
												key [i] = (char)96;
												if (v == 77) {
													key [i] = (char)39;
												}
											}
										}
									} else {
										key [i] = (char)0;
										if (v <= 80) {
											if (v <= 78) {
												if (v == 78) {
													key [i] = (char)81;
												}
											} else {
												key [i] = (char)32;
												if (v <= 79) {
													if (v == 79) {
														key [i] = (char)63;
													}
												} else {
													key [i] = (char)109;
													if (v == 80) {
														key [i] = (char)35;
													}
												}
											}
										} else {
											key [i] = (char)247;
											if (v == 81) {
												key [i] = (char)79;
											}
										}
									}
								}
							}
						} else {
							key [i] = (char)141;
							if (v <= 87) {
								if (v <= 84) {
									if (v <= 82) {
										if (v == 82) {
											key [i] = (char)65;
										}
									} else {
										key [i] = (char)131;
										if (v <= 83) {
											if (v == 83) {
												key [i] = (char)123;
											}
										} else {
											key [i] = (char)219;
											if (v == 84) {
												key [i] = (char)58;
											}
										}
									}
								} else {
									key [i] = (char)76;
									if (v <= 86) {
										if (v <= 85) {
											if (v == 85) {
												key [i] = (char)51;
											}
										} else {
											key [i] = (char)123;
											if (v == 86) {
												key [i] = (char)34;
											}
										}
									} else {
										key [i] = (char)10;
										if (v == 87) {
											key [i] = (char)77;
										}
									}
								}
							} else {
								key [i] = (char)233;
								if (v <= 91) {
									if (v <= 89) {
										if (v <= 88) {
											if (v == 88) {
												key [i] = (char)66;
											}
										} else {
											key [i] = (char)152;
											if (v == 89) {
												key [i] = (char)105;
											}
										}
									} else {
										key [i] = (char)242;
										if (v <= 90) {
											if (v == 90) {
												key [i] =(char) 61;
											}
										} else {
											key [i] = (char)70;
											if (v == 91) {
												key [i] = (char)113;
											}
										}
									}
								} else {
									key [i] = (char)86;
									if (v <= 92) {
										if (v == 92) {
											key [i] = (char)80;
										}
									} else {
										key [i] = (char)166;
										if (v <= 93) {
											if (v == 93) {
												key [i] = (char)104;
											}
										} else {
											key [i] = (char)189;
											if (v == 94) {
												key [i] = (char)40;
											}
										}
									}
								}
							}
						}
					} else {
						key [i] = (char)223;
						if (v <= 99) {
							if (v <= 98) {
								if (v <= 97) {
									if (v <= 96) {
										if (v <= 95) {
											if (v == 95) {
												key [i] = (char)48;
											}
										} else {
											key [i] = (char)45;
											if (v == 96) {
												key [i] = (char)120;
											}
										}
									} else {
										key [i] = (char)180;
										if (v == 97) {
											key [i] = (char)99;
										}
									}
								} else {
									key [i] = (char)3;
									if (v == 98) {
										key [i] = (char)55;
									}
								}
							} else {
								key [i] = (char)62;
								if (v == 99) {
									key [i] = (char)106;
								}
							}
						} else {
							key [i] = (char)157;
							if (v <= 103) {
								if (v <= 100) {
									if (v == 100) {
										key [i] = (char)95;
									}
								} else {
									key [i] = (char)187;
									if (v <= 101) {
										if (v == 101) {
											key [i] = (char)74;
										}
									} else {
										key [i] = (char)33;
										if (v <= 102) {
											if (v == 102) {
												key [i] = (char)54;
											}
										} else {
											key [i] = (char)240;
											if (v == 103) {
												key [i] = (char)38;
											}
										}
									}
								}
							} else {
								key [i] = (char)167;
								if (v <= 107) {
									if (v <= 106) {
										if (v <= 105) {
											if (v <= 104) {
												if (v == 104) {
													key [i] = (char)76;
												}
											} else {
												key [i] = (char)240;
												if (v == 105) {
													key [i] = (char)82;
												}
											}
										} else {
											key [i] = (char)250;
											if (v == 106) {
												key [i] = (char)44;
											}
										}
									} else {
										key [i] = (char)26;
										if (v == 107) {
											key [i] = (char)119;
										}
									}
								} else {
									key [i] = (char)2;
									if (v <= 110) {
										if (v <= 108) {
											if (v == 108) {
												key [i] = (char)101;
											}
										} else {
											key [i] = (char)88;
											if (v <= 109) {
												if (v == 109) {
													key [i] = (char)98;
												}
											} else {
												key [i] = (char)116;
												if (v == 110) {
													key [i] = (char)78;
												}
											}
										}
									} else {
										key [i] = (char)42;
										if (v <= 111) {
											if (v == 111) {
												key [i] = (char)68;
											}
										} else {
											key [i] = (char)128;
											if (v == 112) {
												key [i] = (char)60;
											}
										}
									}
								}
							}
						}
					}
				} else {
					key [i] = (char)211;
					if (v <= 120) {
						if (v <= 115) {
							if (v <= 113) {
								if (v == 113) {
									key [i] = (char)126;
								}
							} else {
								key [i] = (char)7;
								if (v <= 114) {
									if (v == 114) {
										key [i] = (char)46;
									}
								} else {
									key [i] = (char)131;
									if (v == 115) {
										key [i] = (char)112;
									}
								}
							}
						} else {
							key [i] = (char)185;
							if (v <= 117) {
								if (v <= 116) {
									if (v == 116) {
										key [i] = (char)33;
									}
								} else {
									key [i] = (char)146;
									if (v == 117) {
										key [i] = (char)42;
									}
								}
							} else {
								key [i] = (char)89;
								if (v <= 119) {
									if (v <= 118) {
										if (v == 118) {
											key [i] = (char)93;
										}
									} else {
										key [i] = (char)213;
										if (v == 119) {
											key [i] = (char)94;
										}
									}
								} else {
									key [i] = (char)217;
									if (v == 120) {
										key [i] = (char)49;
									}
								}
							}
						}
					} else {
						key [i] = (char)22;
						if (v <= 122) {
							if (v <= 121) {
								if (v == 121) {
									key [i] = (char)64;
								}
							} else {
								key [i] = (char)108;
								if (v == 122) {
									key [i] = (char)36;
								}
							}
						} else {
							key [i] = (char)41;
							if (v <= 123) {
								if (v == 123) {
									key [i] = (char)67;
								}
							} else {
								key [i] = (char)161;
								if (v <= 124) {
									if (v == 124) {
										key [i] = (char)75;
									}
								} else {
									key [i] = (char)14;
									if (v <= 125) {
										if (v == 125) {
											key [i] = (char)56;
										}
									} else {
										key [i] = (char)204;
										if (v == 126) {
											key [i] = (char)53;
										}
									}
								}
							}
						}
					}
				}
			}
			return new string (key, 0, 32);
		}
	}

}
