using System;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace IRR.Models
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

		public static List<Landmark> GetLandmarks()
		{
			List<Landmark> landmarks = new List<Landmark>();

			using (SqlConnection connection = new SqlConnection(User.GetSessionUser().GetConnectionString()))
			{
				SqlCommand cmd = new SqlCommand();
				cmd.Connection = connection;
				cmd.CommandText = "IRR.GetLandmarks";
				cmd.CommandType = System.Data.CommandType.StoredProcedure;
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

		internal static Landmark GetLandmarkEntry(string landmarkId)
		{
			using (SqlConnection connection = new SqlConnection(User.GetSessionUser().GetConnectionString()))
			{
				SqlCommand cmd = new SqlCommand();
				cmd.Connection = connection;
				cmd.CommandText = "IRR.GetLandmarks";
				cmd.CommandType = System.Data.CommandType.StoredProcedure;
				cmd.Parameters.AddWithValue("LandmarkId", landmarkId);
				connection.Open();
				SqlDataReader reader = cmd.ExecuteReader();
				while (reader.Read())
					return new Landmark()
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
					};
				return null;
			}
		}

		internal static void DeleteLandmark(int? ldId)
		{
			using (SqlConnection connection = new SqlConnection(User.GetSessionUser().GetConnectionString()))
			{
				SqlCommand cmd = new SqlCommand();
				cmd.Connection = connection;
				cmd.CommandText = "IRR.DeleteLandmark";
				cmd.CommandType = System.Data.CommandType.StoredProcedure;
				cmd.Parameters.AddWithValue("LDId", ldId);
				connection.Open();
				cmd.ExecuteNonQuery();
			}
		}

		internal bool SaveLandmark()
		{
			using (SqlConnection connection = new SqlConnection(User.GetSessionUser().GetConnectionString()))
			{
				SqlCommand cmd = new SqlCommand();
				cmd.Connection = connection;
				cmd.CommandText = "IRR.SaveLandmark";
				cmd.CommandType = System.Data.CommandType.StoredProcedure;
				cmd.Parameters.AddWithValue("LDId", LDId);
				cmd.Parameters.AddWithValue("LandmarkId", LandmarkId);
				cmd.Parameters.AddWithValue("LandmarkURL", LandmarkURL);
				cmd.Parameters.AddWithValue("Name", Name);
				cmd.Parameters.AddWithValue("Location", Location);
				cmd.Parameters.AddWithValue("Information", Information);
				connection.Open();
				cmd.ExecuteNonQuery();

				return true;
			}
		}
	}
}