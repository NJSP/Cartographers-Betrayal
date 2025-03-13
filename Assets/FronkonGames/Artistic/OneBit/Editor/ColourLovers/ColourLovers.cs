///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// Copyright (c) Fronkon Games @FronkonGames <fronkongames@gmail.com>. All rights reserved.
//
// THIS FILE CAN NOT BE HOSTED IN PUBLIC REPOSITORIES.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES
// OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR
// IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
using System;
using System.Text.RegularExpressions;
using System.Net;
using System.Xml;
using UnityEngine;

namespace FronkonGames.Artistic.OneBit
{
  /// <summary> Five color palette. </summary>
  [Serializable]
  public struct Palette
  {
    public string tittle;

    public string author;

    public Color[] colors;
  }

  /// <summary> Color palettes search using colourlovers.com. </summary>
  public class ColourLovers
  {
    private static readonly string urlRandom = "http://www.colourlovers.com/api/palettes/random?format=xml";
    private static readonly string urlTop = "http://www.colourlovers.com/api/palettes/top";

    /// <summary> Find palettes. This function is synchronous, it may take some time to return a value. </summary>
    /// <param name="page">Page to search, 0 to get the first 5 palettes.</param>
    /// <param name="search">Search term.</param>
    /// <param name="totalResults">Results obtained.</param>
    /// <returns>Array of palettes.</returns>
    public static Palette[] Search(int page, string search, out int totalResults)
    {
      totalResults = 0;
      Palette[] colorEntries = new Palette[5];

      if (string.IsNullOrEmpty(search) == false)
      {
        search = search.Replace(",", " ").Replace("&", "");
        search = Regex.Replace(search, @"\s*\s\s*", "+");
      }

      string url = string.Format("{0}?keywords={1}&resultOffset={2}&numResults=5&orderCol=score&sortBy=DESC", urlTop, search, page * 5);
      HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
      request.Method = "GET";
      request.ContentType = "application/json";
      request.UserAgent = "Mozilla / 5.0";

      using (var response = request.GetResponse())
      {
        using (var stream = response.GetResponseStream())
        {
          XmlDocument doc = new();
          doc.Load(stream);

          XmlNode xmlPalettes = doc.SelectSingleNode("palettes");
          if (xmlPalettes != null)
          {
            if (xmlPalettes.Attributes["totalResults"] != null)
              totalResults = int.Parse(xmlPalettes.Attributes["totalResults"].Value);

            XmlNodeList xmllist = xmlPalettes.SelectNodes("palette");
            for (int i = 0; i < xmllist.Count && i < 5; ++i)
            {
              colorEntries[i].tittle = xmllist[i].SelectSingleNode("title").InnerText;
              colorEntries[i].author = xmllist[i].SelectSingleNode("userName").InnerText;

              colorEntries[i].colors = new Color[5];
              for (int j = 0; j < 5; ++j)
                colorEntries[i].colors[j] = new Color(0.0f, 0.0f, 0.0f, 1.0f);

              XmlNodeList nodeColor = xmllist[i].SelectNodes("colors/hex");
              for (int j = 0; j < nodeColor.Count && j < 5; ++j)
              {
                colorEntries[i].colors[j] = HexToColor(nodeColor[j].InnerText);
                colorEntries[i].colors[j].a = 1.0f;
              }
            }
          }
        }
      }

      return colorEntries;
    }

    /// <summary> Gets a random palette. This function is synchronous, it may take some time to return a value. </summary>
    /// <returns>Palette.</returns>
    public static Palette Random()
    {
      Palette colorEntry = new Palette();

      HttpWebRequest request = (HttpWebRequest)WebRequest.Create(urlRandom);
      request.ContentType = "application/json";
      request.UserAgent = "Mozilla / 5.0";

      using (var response = request.GetResponse())
      {
        using (var stream = response.GetResponseStream())
        {
          XmlDocument doc = new XmlDocument();
          doc.Load(stream);

          XmlNode nodeTitle = doc.SelectSingleNode("palettes/palette/title");
          colorEntry.tittle = nodeTitle.InnerText;

          XmlNode nodeAuthor = doc.SelectSingleNode("palettes/palette/userName");
          colorEntry.author = nodeAuthor.InnerText;

          colorEntry.colors = new Color[5];
          for (int i = 0; i < 5; ++i)
            colorEntry.colors[i] = new Color(0.0f, 0.0f, 0.0f, 1.0f);

          XmlNodeList nodeColor = doc.SelectNodes("palettes/palette/colors/hex");
          for (int i = 0; i < nodeColor.Count && i < 5; ++i)
          {
            colorEntry.colors[i] = HexToColor(nodeColor[i].InnerText);
            colorEntry.colors[i].a = 1.0f;
          }
        }
      }

      return colorEntry;
    }

    private static Color HexToColor(string hex)
    {
      int rgb = Convert.ToInt32(hex, 16);
      float r = (rgb & 0xff0000) >> 16;
      float g = (rgb & 0xff00) >> 8;
      float b = (rgb & 0xff);

      return new Color(r / 256.0f, g / 256.0f, b / 256.0f);
    }
  }
}