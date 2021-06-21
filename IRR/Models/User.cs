using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace IRR.Models
{
	public class User
	{
		public string UserName { get; set; }
		public string Password { get; set; }
		public string LoginError { get; set; }

		public bool Login()
		{
			try
			{
				LoginError = null;
				using (SqlConnection connection = new SqlConnection(this.GetConnectionString()))
				{
					connection.Open();
				}
				HttpContext.Current.Session.Add("User", this);
				return true;
			}
			catch (Exception ex)
			{
				LoginError = ex.Message;
				return false;
			}
		}

		public static User GetSessionUser()
		{
			if (HttpContext.Current.Session != null)
				return (User)HttpContext.Current.Session["User"];
			return null;
		}

		public string GetConnectionString()
		{
			SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder(ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString);
			builder.UserID = UserName;
			builder.Password = Password;
			return builder.ToString();
		}

		public static string GetAnonymousConnectionString()
		{
			return ConfigurationManager.ConnectionStrings["AnonymousConnection"].ConnectionString;
		}

		public static void Logout()
		{
			HttpContext.Current.Session.Clear();
			HttpContext.Current.Session.Abandon();
		}

		public bool CreateAccount()
		{
			using (SqlConnection connection = new SqlConnection(GetAnonymousConnectionString()))
			{
				SqlCommand cmd = new SqlCommand();
				cmd.Connection = connection;
				cmd.CommandText = "IRR.CreateUser";
				cmd.CommandType = System.Data.CommandType.StoredProcedure;
				connection.Open();
				SqlCommandBuilder.DeriveParameters(cmd);
				cmd.Parameters["@UserName"].Value = UserName;
				cmd.Parameters["@Password"].Value = Password;
				cmd.ExecuteNonQuery();
				return true;
			}
		}

		public bool IsAdmin()
		{
			using (SqlConnection connection = new SqlConnection(GetSessionUser().GetConnectionString()))
			{
				SqlCommand cmd = new SqlCommand();
				cmd.Connection = connection;
				cmd.CommandText = "IRR.IsAdmin";
				cmd.CommandType = System.Data.CommandType.StoredProcedure;
				connection.Open();
				SqlCommandBuilder.DeriveParameters(cmd);
				cmd.ExecuteNonQuery();
				return (bool)cmd.Parameters["@IsAdmin"].Value;
			}
		}
	}
}