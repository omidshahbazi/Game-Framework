// Copyright 2015-2017 Zorvan Game Studio. All Rights Reserved.
using System.Collections.Generic;
using System.IO;

namespace Zorvan.Framework.Common.Utilities
{
	public static class FileHelper
	{
		public static uint GetHash(string FilePath)
		{
			return CRC32.CalculateHash(File.ReadAllBytes(FilePath));
		}

		public static bool AreFilesSame(string FilePathA, string FilePathB)
		{
			return GetHash(FilePathA) == GetHash(FilePathB);
		}

		//private static string[] FilterFiles(string[] Paths, string SearchPatterns)
		//{
		//	List<string> list = new List<string>();

		//	string[] searchPatterns = SearchPatterns.Split('|');

		//	for (int i = 0; i < Paths.Length; ++i)
		//		for (int j = 0; j < searchPatterns.Length; ++j)
		//			list.AddRange(Directory.GetFiles(Paths[i], searchPatterns[j], SearchOption.AllDirectories));

		//	string[] files = list.ToArray();

		//	for (int i = 0; i < files.Length; ++i)
		//	{
		//		string filePath = files[i];

		//		for (int j = 0; j < Paths.Length; ++j)
		//			filePath = filePath.Replace(Paths[j] + "\\", "");

		//		files[i] = filePath.Replace("\\", "/").Remove(filePath.LastIndexOf('.'));
		//	}

		//	return files;
		//}
	}
}