using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net.Http;
using Newtonsoft.Json.Linq;
using System.Globalization;

namespace rabet
{
    public partial class Form1 : Form
    {

        private static readonly string apiUrl = "http://80.210.21.35/getlastStatusWithoutSomeGroups.php"; // Replace with your API URL
        string connectionString = "Server=AZADEGAN-RABET;Database=dideban_azadegan;User Id=sa;Password=azg#^(963;";

        public Form1()
        {
            InitializeComponent();
        }


        private static async Task<JArray> FetchDataFromApi()
        {
            using (HttpClient client = new HttpClient())
            {
                HttpResponseMessage response = await client.GetAsync(apiUrl);
                response.EnsureSuccessStatusCode();

                string responseData = await response.Content.ReadAsStringAsync();
                return JArray.Parse(responseData); // Assuming the API returns a JSON array
            }
        }


        private  void SaveDataToLastStatus(JArray data)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                   connection.Open();

                    foreach (var item in data)
                    {
                        string sqlQuery = "INSERT INTO last_status ( deviceid, name, fixtime, latitude, longitude, distance) VALUES ( @deviceid, @name, @fixtime, @latitude, @longitude, @distance)";
                        using (SqlCommand command = new SqlCommand(sqlQuery, connection))
                        {
                            CultureInfo culture = new CultureInfo("en-EN");
                            DateTime fixtime =DateTime.Parse (item["fixtime"].ToString(),culture);

                            int deviceid = int.Parse(item["deviceid"].ToString());
                            command.Parameters.AddWithValue("@deviceid", deviceid);

                            String name = item["name"].ToString();
                            command.Parameters.AddWithValue("@name",name);

                            command.Parameters.AddWithValue("@fixtime", fixtime);

                            double latitude = double.Parse(item["latitude"].ToString(), culture);
                            command.Parameters.AddWithValue("@latitude", latitude);

                            double longitude = double.Parse(item["longitude"].ToString(), culture);
                            command.Parameters.AddWithValue("@longitude", longitude);

                            String at = item["attributes"].ToString();
                            int index_distance = at.IndexOf("totalDistance");
                            if (index_distance > -1)
                            {
                                at = at.Substring(index_distance);
                                int index_colon = at.IndexOf(":");
                                int index_comma = at.IndexOf(",");
                                if(index_comma < 0)
                                {
                                    index_comma = at.IndexOf("}");
                                }
                                if(index_comma < 0)
                                {
                                    textBox1.Text = textBox1.Text + $"Fetch total distance of {name} with device id {deviceid} error" + DateTime.Now.ToString() + Environment.NewLine;
                                }
                                String distance = at.Substring(index_colon + 1, index_comma - index_colon - 1);
                                command.Parameters.AddWithValue("@distance",double.Parse(distance,culture));
                            }

                            
                            int result = command.ExecuteNonQuery();
                            if (result < 0)
                            {
                                textBox1.Text = textBox1.Text + $"Error inserting data into Database! deviceid: {deviceid} name: {name}" + DateTime.Now.ToString() + Environment.NewLine;

                            }
                        }
                    }
                    textBox1.Text = textBox1.Text + "Data inserted successfully to last_status!" + DateTime.Now.ToString() + Environment.NewLine;
                }
            }catch (Exception ex)
            {
                // Handle any errors that may have occurred
                textBox1.Text = textBox1.Text + "Exception: " + ex.Message + DateTime.Now.ToString() + Environment.NewLine;
            }
        }

        private void SaveDataToStatus(JArray data)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    foreach (var item in data)
                    {
                        string sqlQuery = "INSERT INTO status ( deviceid, name, fixtime, latitude, longitude, distance) VALUES ( @deviceid, @name, @fixtime, @latitude, @longitude, @distance)";
                        using (SqlCommand command = new SqlCommand(sqlQuery, connection))
                        {
                            CultureInfo culture = new CultureInfo("en-EN");
                            DateTime fixtime = DateTime.Parse(item["fixtime"].ToString(), culture);

                            int deviceid = int.Parse(item["deviceid"].ToString());
                            command.Parameters.AddWithValue("@deviceid", deviceid);

                            String name = item["name"].ToString();
                            command.Parameters.AddWithValue("@name", name);

                            command.Parameters.AddWithValue("@fixtime", fixtime);

                            double latitude = double.Parse(item["latitude"].ToString(), culture);
                            command.Parameters.AddWithValue("@latitude", latitude);

                            double longitude = double.Parse(item["longitude"].ToString(), culture);
                            command.Parameters.AddWithValue("@longitude", longitude);

                            String at = item["attributes"].ToString();
                            int index_distance = at.IndexOf("totalDistance");
                            if (index_distance > -1)
                            {
                                at = at.Substring(index_distance);
                                int index_colon = at.IndexOf(":");
                                int index_comma = at.IndexOf(",");
                                if (index_comma < 0)
                                {
                                    index_comma = at.IndexOf("}");
                                }
                                if (index_comma < 0)
                                {
                                    textBox1.Text = textBox1.Text + $"Fetch total distance of {name} with device id {deviceid} error" + DateTime.Now.ToString() + Environment.NewLine;
                                }
                                String distance = at.Substring(index_colon + 1, index_comma - index_colon - 1);
                                command.Parameters.AddWithValue("@distance", double.Parse(distance, culture));
                            }


                            int result = command.ExecuteNonQuery();
                            if (result < 0)
                            {
                                textBox1.Text = textBox1.Text + $"Error inserting data into Database! deviceid: {deviceid} name: {name}" + DateTime.Now.ToString() + Environment.NewLine;

                            }
                        }
                    }
                    textBox1.Text = textBox1.Text + "Data inserted successfully to status!" + DateTime.Now.ToString() + Environment.NewLine;
                }
            }
            catch (Exception ex)
            {
                // Handle any errors that may have occurred
                textBox1.Text = textBox1.Text + "Exception: " + ex.Message + DateTime.Now.ToString() + Environment.NewLine;
            }
        }

        private void truncateLastStatus() {

            string tableName = "last_status";
            string query = $"TRUNCATE TABLE {tableName}";
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    // Open the connection to the database
                    connection.Open();

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        // Execute the truncate command
                        command.ExecuteNonQuery();
                        textBox1.Text = textBox1.Text + $"Table {tableName} truncated successfully." + Environment.NewLine;
                    }
                }
            }
            catch (Exception ex)
            {
                textBox1.Text = textBox1.Text + $"An error occurred: {ex.Message}" + Environment.NewLine;

            }
        }

        private async void Form1_Load(object sender, EventArgs e)
        {
            var data = await FetchDataFromApi();
            SaveDataToStatus(data);
            truncateLastStatus();
            SaveDataToLastStatus(data);
        }
    }
}
