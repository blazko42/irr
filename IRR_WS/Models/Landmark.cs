using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.IO;
using OpenCvSharp;
using System.IO.Compression;

namespace IRR_WS.Models
{
	public class Landmark
	{
		public int LDId { get; set; }
		public string LandmarkId { get; set; }
		public string LandmarkURL { get; set; }
		public byte[] LandmarkImage { get; set; }
		public int FeatureExtractor { get; set; }
		public string Name { get; set; }
		public string Location { get; set; }
		public string Information { get; set; }
		public DateTime Date { get; set; }
		public string LandmarkUploadedImageName { get; set; }
		public byte[] LandmarkUploadedIamgeContent { get; set; }

		public static async Task<List<Landmark>> RetrieveLandmarks(List<string> landmarksIdList)
		{
			try
			{
				string landmarksXML = SaveToXml(landmarksIdList);

				List<Landmark> landmarks = new List<Landmark>();

				using (SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString))
				{

					SqlCommand cmd = new SqlCommand();
					cmd.Connection = connection;
					cmd.CommandText = "IRR.RetrieveLandmark";
					cmd.CommandType = System.Data.CommandType.StoredProcedure;
					cmd.Parameters.AddWithValue("Landmarks", landmarksXML); ;
					connection.Open();
					SqlDataReader reader = cmd.ExecuteReader();
					while (reader.Read())
						landmarks.Add(new Landmark()
						{
							LDId = Convert.ToInt32(reader["LDId"]),
							LandmarkId = (string)reader["LandmarkId"],
							LandmarkURL = (string)reader["LandmarkURL"],
							LandmarkImage = (byte[])reader["LandmarkImage"],
							FeatureExtractor = Convert.ToInt32(reader["FeatureExtractor"]),
							Name = (string)(reader["Name"]),
							Location = (string)(reader["Location"]),
							Information = (string)(reader["Information"]),
							Date = (DateTime)reader["Date"]
						});

					return landmarks;
				}
			}
			catch (Exception ex)
			{

			}
			return null;
		}

		internal static void SaveLandmarkBinaryImage(string ldId, byte[] landmarkImage, string descriptorName, int datasetSize, string name, string location, string information)
		{
			try
			{
				using (SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString))
				{
					SqlCommand cmd = new SqlCommand();
					cmd.Connection = connection;
					cmd.CommandText = "IRR.SaveLandmarkBinaryImage";
					cmd.CommandType = System.Data.CommandType.StoredProcedure;
					cmd.Parameters.AddWithValue("LandmarkId", ldId);
					cmd.Parameters.AddWithValue("LandmarkImage", landmarkImage);
					cmd.Parameters.AddWithValue("FeatureExtractor", descriptorName);
					cmd.Parameters.AddWithValue("Name", name);
					cmd.Parameters.AddWithValue("Location", location);
					cmd.Parameters.AddWithValue("Information", information);
					cmd.Parameters.AddWithValue("DatasetSize", datasetSize);
					connection.Open();
					cmd.ExecuteNonQuery();
				}
			}
			catch (Exception ex)
			{
				throw ex;
			}

		}

		public static string SaveToXml(List<string> landmarksIdList)
		{

			StringBuilder builder = new StringBuilder();
			System.Xml.XmlWriterSettings settings = new System.Xml.XmlWriterSettings()
			{
				OmitXmlDeclaration = true,
				ConformanceLevel = System.Xml.ConformanceLevel.Fragment,
				Encoding = UTF8Encoding.UTF8
			};

			using (System.Xml.XmlWriter writer = System.Xml.XmlWriter.Create(builder, settings))
			{
				int rankInQuery = 1;
				writer.WriteStartElement("Landmarks");
				foreach (string landmarkId in landmarksIdList)
				{
					writer.WriteStartElement("Landmark");
					writer.WriteAttributeString("LandmarkId", landmarkId);
					writer.WriteAttributeString("LandmarkRank", Convert.ToString(rankInQuery));
					writer.WriteEndElement();
					rankInQuery++;
				}
				writer.WriteEndElement();
			}
			return builder.ToString();
		}

		public static List<string> GetSimilarImagesGUID(byte[] pictureContent, string descriptorName)
		{
			List<IndexImage.ImageIndex> indexedImages = IndexImage.LoadFromFile(descriptorName);

			Searcher searcher = new Searcher(indexedImages);

			List<string> similarImages = searcher.SearchDescriptor(pictureContent, descriptorName);

			return similarImages;
		}

		public static void FeatureMatch(byte[] pictureContent, string landmarkId, string descriptorName)
		{
			List<IndexImage.ImageIndex> indexedImages = IndexImage.LoadFromFile(descriptorName);

			Searcher searcher = new Searcher(indexedImages);

			searcher.DrawMatchesBetweenImages(pictureContent, landmarkId, descriptorName);

		}
	}
}


