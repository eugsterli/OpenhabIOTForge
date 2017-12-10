using Autodesk.Forge;
using Autodesk.Forge.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Configuration;
using System.Web.UI;
using System.Web.UI.WebControls;
using Microsoft.ServiceBus.Messaging;
using System.Threading;
using System.Threading.Tasks;
using System.Text;
using System.Data;
using System.Configuration;
using MySql.Data.MySqlClient;
using System.Web.Script.Serialization;

namespace forgeViewerTest
{
    public partial class _default : System.Web.UI.Page
    {
  
        
        public static string[] sensorvalue = new string[7];

        

        protected void Page_Load(object sender, EventArgs e)
        {


            ThreadStart childthreat = new ThreadStart(GetMySQLData);
            ThreadStart childthreat2 = new ThreadStart(RefreshJavascriptArray);

            Thread child = new Thread(childthreat);
            Thread child2 = new Thread(childthreat2);

            child.Start();
            child2.Start();






        }
      

        protected async void Button1_Click(object sender, EventArgs e)
        {
            

          
            // create a randomg bucket name (fixed prefix + randomg guid)
            string bucketKey = "forgeapp" + Guid.NewGuid().ToString("N").ToLower();

            // upload the file (to your server)
            string fileSavePath = Path.Combine(HttpContext.Current.Server.MapPath("~/App_Data"), bucketKey, FileUpload1.FileName);
            Directory.CreateDirectory(Path.GetDirectoryName(fileSavePath));
            FileUpload1.SaveAs(fileSavePath);

            // get a write enabled token
            TwoLeggedApi oauthApi = new TwoLeggedApi();
            dynamic bearer = await oauthApi.AuthenticateAsync(
                "fuWM1Dm2PtbSTvtnjUg0WmXgDB88WsEt",
                "vRR3VnJln5j1HDfT",
                "client_credentials",
                new Scope[] { Scope.BucketCreate, Scope.DataCreate, Scope.DataWrite, Scope.DataRead });

            // create the Forge bucket
            PostBucketsPayload postBucket = new PostBucketsPayload(bucketKey, null, PostBucketsPayload.PolicyKeyEnum.Transient /* erase after 24h*/ );
            BucketsApi bucketsApi = new BucketsApi();
            bucketsApi.Configuration.AccessToken = bearer.access_token;
            dynamic newBucket = await bucketsApi.CreateBucketAsync(postBucket);

            // upload file (a.k.a. Objects)
            ObjectsApi objectsApi = new ObjectsApi();
            oauthApi.Configuration.AccessToken = bearer.access_token;
            dynamic newObject;
            using (StreamReader fileStream = new StreamReader(fileSavePath))
            {
                newObject = await objectsApi.UploadObjectAsync(bucketKey, FileUpload1.FileName,
                    (int)fileStream.BaseStream.Length, fileStream.BaseStream,
                    "application/octet-stream");
            }

            // translate file
            string objectIdBase64 = ToBase64(newObject.objectId);
            List<JobPayloadItem> postTranslationOutput = new List<JobPayloadItem>()
                {
                    new JobPayloadItem(
                    JobPayloadItem.TypeEnum.Svf /* Viewer*/,
                    new List<JobPayloadItem.ViewsEnum>()
                    {
                       JobPayloadItem.ViewsEnum._3d,
                       JobPayloadItem.ViewsEnum._2d
                    })
                };

            JobPayload postTranslation = new JobPayload(
                new JobPayloadInput(objectIdBase64),
                new JobPayloadOutput(postTranslationOutput));
            DerivativesApi derivativeApi = new DerivativesApi();
            derivativeApi.Configuration.AccessToken = bearer.access_token;
            dynamic translation = await derivativeApi.TranslateAsync(postTranslation);

            // check if is ready
            int progress = 0;
            do
            {
                System.Threading.Thread.Sleep(1000); // wait 1 second
                try
                {
                    dynamic manifest = await derivativeApi.GetManifestAsync(objectIdBase64);
                    progress = (string.IsNullOrWhiteSpace(Regex.Match(manifest.progress, @"\d+").Value) ? 100 : Int32.Parse(Regex.Match(manifest.progress, @"\d+").Value));
                }
                catch (Exception ex)
                {

                }
            } while (progress < 100);

            // ready!!!!

            // register a client-side script to show this model
            //Page.ClientScript.RegisterStartupScript(this.GetType(), "ShowModel", string.Format("<script>showModel('{0}');</script>", objectIdBase64));
            Page.ClientScript.RegisterStartupScript(this.GetType(), "InitApplication", string.Format("<script>initApplication('{0}');</script>", objectIdBase64));
           

            // clean up
            Directory.Delete(Path.GetDirectoryName(fileSavePath), true);
        }

        public string ToBase64(string input)
        {
            var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(input);
            return System.Convert.ToBase64String(plainTextBytes);
        }

        public static void GetMySQLData()
        {
            while (true)
            {
                

                MySqlConnection connection = new MySqlConnection("Database=openhab;Data Source=localhost;User Id=root;Password=Sandro*93");
                connection.Open();

                MySqlCommand command = connection.CreateCommand();
                command.CommandText = "select distinct i11.value, i12.value, i13.value, i14.value,i11.value,i11.value,i11.value from item11 as i11, item12 as i12, item13 as i13, item14 as i14 where i11.time = (select max(time) from item11) and i12.time = (select max(time) from item12) and i13.time = (select max(time) from item13) and i14.time = (select max(time) from item14)";
                MySqlDataReader reader = command.ExecuteReader();

                Object[] values = new Object[reader.FieldCount];

                reader.Read();
                int fieldCount = reader.GetValues(values);


                for (int i = 0; i < fieldCount; i++)
                {
                    sensorvalue[i] = values[i].ToString();
                }

                connection.Close();
                Thread.Sleep(10000);

            }
        }

        public void RefreshJavascriptArray()
        {
            
            while (true)
            {

                ///give sensordate to javascript variable numbers
                //ScriptManager.RegisterClientScriptBlock(this, e.GetType(), "settingvariable", "var numbers='" + sensorvalue + "'", true);
                var serializer = new JavaScriptSerializer();
                var jsArray = string.Format("var jsArray = {0}", serializer.Serialize(sensorvalue));

                ClientScriptManager cs = Page.ClientScript;
                cs.RegisterClientScriptBlock(
                    this.GetType(),
                    "arrayDeclaration",
                    jsArray,
                    true
                );

                Thread.Sleep(9000);
            }
        }
    }
}