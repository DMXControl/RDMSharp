using System.Xml;

namespace RDMSharpTest.RDM
{
    public class UpdateManufacturerList
    {
        [SetUp]
        public void Setup()
        {
        }

        //[Test]  //Disabled for now
        public async Task UpdateManufacturerListMethode()
        {
            string website = string.Empty;
            for (int i = 0; i < 10; i++)
            {
                try
                {
                    using var client = new HttpClient();
                    website = await client.GetStringAsync("https://tsp.esta.org/tsp/working_groups/CP/mfctrIDs.php");
                }
                catch
                {
                    await Task.Delay(3000);
                }
                if (!string.IsNullOrWhiteSpace(website))
                    break;                
            }
            website = website.Substring(website.IndexOf("<table id=\'main_table\'"));
            website = website.Remove(website.IndexOf("</table>")) + "</table>";
            website = website.Replace("<p>", "");
            website = website.Replace("<br>", "");
            website = website.Replace("&nbsp;", "");
            website = website.Replace("&", "");
            website = website.Replace(";", "");
            website = website.Replace("Co.", "");
            website = website.Replace("Ltd.", "");
            website = website.Replace("Inc.", "");
            website = website.Replace("Ltd", "");
            website = website.Replace("GmbH", "");
            website = website.Replace(",", "");
            XmlReader reader = XmlReader.Create(new StringReader(website), new XmlReaderSettings() { DtdProcessing = DtdProcessing.Parse });
            Dictionary<string, string> dict = new Dictionary<string, string>();
            while (reader.ReadToFollowing("table"))
            {
                var id = reader.GetAttribute("id");
                if (string.Equals(id, "main_table"))
                {
                    reader.ReadToFollowing("tr");
                    reader.ReadToFollowing("tr");
                    do
                    {

                        reader.ReadToFollowing("td");
                        string uid = reader.ReadElementContentAsString();
                        reader.ReadToFollowing("td");
                        string shortName = reader.ReadElementContentAsString();
                        reader.ReadToFollowing("td");
                        string longName = reader.ReadElementContentAsString();
                        if (longName.EndsWith(" "))
                            longName = longName.Remove(longName.Length - 2);
                        if (longName.StartsWith(" "))
                            longName = longName.Substring(1);

                        shortName = shortName.Replace("  ", " ");
                        shortName = shortName.Replace(".", "_");
                        shortName = shortName.Replace("-", "_");
                        shortName = shortName.Replace(" ", "_");
                        longName = longName.Replace("  ", " ");
                        longName = longName.Replace(".", "_");
                        longName = longName.Replace("-", "_");
                        longName = longName.Replace(" ", "_");

                        dict[uid] = string.IsNullOrWhiteSpace(shortName) ? longName : shortName;
                    }
                    while (reader.ReadToFollowing("tr"));
                }
            }
        }
    }
}