using OpenCvSharp;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading.Tasks;
using System.Web;

namespace IRR_WS.Models
{
	public class IndexImage
	{
		[Serializable]
		public class ImageIndex
		{
			public string ImageId { get; set; }
			public string ImagePath { get; set; }
			public string ImageName { get; set; }
			public Descriptor Descriptor { get; set; }

		}

		private static void SaveToFile(List<ImageIndex> data, string descriptorName)
		{
			string datasetName;
			if (descriptorName == "SIFT")
				datasetName = "SIFTDataset.bin";
			else if (descriptorName == "SURF")
				datasetName = "SURFDataset.bin";
			else
				datasetName = "GeneralDataset.bin";

			Stream ms = File.Open(@"D:\Others\Fac\Disertatie\googleDataset\" + datasetName, FileMode.Create, FileAccess.Write, FileShare.Read);
			//Format the object as Binary
			BinaryFormatter formatter = new BinaryFormatter();
			//It serialize the employee object
			formatter.Serialize(ms, data);
			ms.Flush();
			ms.Close();
			ms.Dispose();
		}
		public static List<ImageIndex> LoadFromFile(string descriptorName)
		{
			string datasetName;
			if (descriptorName == "SIFT")
				datasetName = "SIFTDataset.bin";
			else if (descriptorName == "SURF")
				datasetName = "SURFDataset.bin";
			else
				datasetName = "GeneralDataset.bin";

			BinaryFormatter formatter = new BinaryFormatter();
			//Reading the file from the server
			FileStream fs = File.Open(@"D:\Others\Fac\Disertatie\googleDataset\" + datasetName, FileMode.Open);
			object obj = formatter.Deserialize(fs);
			List<ImageIndex> result = (List<ImageIndex>)obj;
			fs.Flush();
			fs.Close();
			fs.Dispose();
			return result;
		}

		public static async Task<bool> ComputeDataset(string descriptorName)
		{
			if (descriptorName == "SIFT")
			{
				ComputeSIFTDataset();
				return true;
			}
			else if (descriptorName == "SURF")
			{
				ComputeSURFDataset();
				return true;
			}
			else return false;

		}

		private static void ComputeSIFTDataset()
		{
			try
			{
				Descriptor siftDescriptor = new Descriptor();
				List<ImageIndex> imgIndexes = new List<ImageIndex>();

				DirectoryInfo datasetDirectory = new DirectoryInfo(@"D:\Others\Fac\Disertatie\googleDataset\images");
				DirectoryInfo infoDirectory = new DirectoryInfo(@"D:\Others\Fac\Disertatie\googleDataset\images_info");

				string name, location, information;
				name = location = information = "Lorem ipsum dolor sit amet.";

				foreach (FileInfo image in datasetDirectory.GetFiles())
				{
					foreach (FileInfo imageInfo in infoDirectory.GetFiles())
					{
						if (Path.GetFileNameWithoutExtension(imageInfo.Name) == Path.GetFileNameWithoutExtension(image.Name))
						{
							name = File.ReadLines(imageInfo.FullName).Take(1).First();
							location = File.ReadLines(imageInfo.FullName).Skip(1).Take(1).First();
							information = File.ReadLines(imageInfo.FullName).Skip(2).Take(1).First();
						}

					}

					Mat datasetImage = Cv2.ImRead(image.FullName);

					siftDescriptor = siftDescriptor.ComputeSIFTKeypointsAndDescriptors(datasetImage);
					string imageGUID = Guid.NewGuid().ToString("N");
					imgIndexes.Add(new ImageIndex() { ImageId = imageGUID, Descriptor = siftDescriptor, ImagePath = image.FullName, ImageName = image.Name });

					Landmark.SaveLandmarkBinaryImage(imageGUID, File.ReadAllBytes(image.FullName), "SIFT", datasetDirectory.GetFiles().Length, name, location, information);
				}

				SaveToFile(imgIndexes, "SIFT");
			}
			catch (Exception ex)
			{
				throw ex;
			}
		}

		private static void ComputeSURFDataset()
		{
			try
			{
				Descriptor surfDescriptor = new Descriptor();
				List<ImageIndex> imgIndexes = new List<ImageIndex>();

				DirectoryInfo datasetDirectory = new DirectoryInfo(@"D:\Others\Fac\Disertatie\googleDataset\images");
				DirectoryInfo infoDirectory = new DirectoryInfo(@"D:\Others\Fac\Disertatie\googleDataset\images_info");

				string name, location, information;
				name = location = information = "Lorem ipsum dolor sit amet.";

				foreach (FileInfo image in datasetDirectory.GetFiles())
				{
					foreach (FileInfo imageInfo in infoDirectory.GetFiles())
					{
						if (Path.GetFileNameWithoutExtension(imageInfo.Name) == Path.GetFileNameWithoutExtension(image.Name))
						{
							name = File.ReadLines(imageInfo.FullName).Take(1).First();
							location = File.ReadLines(imageInfo.FullName).Skip(1).Take(1).First();
							information = File.ReadLines(imageInfo.FullName).Skip(2).Take(1).First();
						}

					}

					Mat datasetImage = Cv2.ImRead(image.FullName);

					surfDescriptor = surfDescriptor.ComputeSURFKeypointsAndDescriptors(datasetImage);
					string imageGUID = Guid.NewGuid().ToString("N");
					imgIndexes.Add(new ImageIndex() { ImageId = imageGUID, Descriptor = surfDescriptor, ImagePath = image.FullName, ImageName = image.Name });

					Landmark.SaveLandmarkBinaryImage(imageGUID, File.ReadAllBytes(image.FullName), "SURF", datasetDirectory.GetFiles().Length, name, location, information);
				}

				SaveToFile(imgIndexes, "SURF");
			}
			catch (Exception ex)
			{
				throw ex;
			}
		}

		public static async Task<bool> ClearDatasetForActiveFeatureExtractor()
		{
			try
			{
				using (SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString))
				{
					SqlCommand cmd = new SqlCommand();
					cmd.Connection = connection;
					cmd.CommandText = "IRR.ClearDatasetForActiveFeatureExtractor";
					cmd.CommandType = System.Data.CommandType.StoredProcedure;
					connection.Open();
					cmd.ExecuteNonQuery();

					return true;
				}
			}
			catch (Exception ex)
			{
				throw ex;
			}
		}

	}
}