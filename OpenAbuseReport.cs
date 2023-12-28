/*
 * Copyright 2023 Christopher Strachan of GridPlay Productions
 * As Is meaning no support for modifing this code
 * If compiling for your grid do NOT claim to be the first
 * that goes to canadiangrid.ca that I own
 * This is my very first attempt at c# outside of the Unity engine
 */
using log4net;
using Mono.Addins;
using Nini.Config;
using OpenMetaverse;
using OpenSim.Framework;
using OpenSim.Region.Framework.Interfaces;
using OpenSim.Region.Framework.Scenes;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Net;
using OSDMap = OpenMetaverse.StructuredData.OSDMap;
using System.IO;
using OpenMetaverse.StructuredData;

[assembly: Addin("OpenAbuseReport", "1.0")]
[assembly: AddinDependency("OpenSim.Region.Framework", OpenSim.VersionInfo.VersionNumber)]

namespace OpenSim.Modules.OpenAbuseReport
{
    [Extension(Path = "/OpenSim/RegionModules", NodeName = "RegionModule", Id = "OpenAbuseReports")]
    public class OpenAbuseReport : ISharedRegionModule
    {
        private static readonly ILog m_log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        public string Name
        {
            get { return "OpenAbuseReport"; }
        }

        public Type ReplaceableInterface
        {
            get { return null; }
        }
        private List<Scene> m_SceneList = new List<Scene>();
        private Scene m_scene;
        public bool m_reportsEnabled = false;
        public bool m_debugEnabled = false;
        public string m_reportURI = String.Empty;
        public void Initialise(IConfigSource config)
        {
            IConfig abuseConfig = config.Configs["Abuse Reports"];
            if (abuseConfig == null)
            {
                return;
            }
            else
            {
                m_reportsEnabled = abuseConfig.GetBoolean("Enabled", false);
                if (!m_reportsEnabled)
                {
                    return;
                }
                m_log.InfoFormat("[Abuse Reports]: Initializing {0}", this.Name);
                m_reportURI = abuseConfig.GetString("ReportURI", String.Empty);
                m_debugEnabled = abuseConfig.GetBoolean("DebugEnabled", false);
            }
        }
        public void AddRegion(Scene scene)
        {
            if (!m_reportsEnabled)
                return;
            m_scene = scene;
            m_scene.EventManager.OnNewClient += handleNewClient;
        }

        private void handleNewClient(IClientAPI client)
        {
            client.OnUserReport += HandleNewUserReport;
        }
        public void onClientClosed(IClientAPI client)
        {
            client.OnUserReport -= HandleNewUserReport;
        }

        public void Close()
        {
            if (!m_reportsEnabled)
                return;

            if (m_debugEnabled) m_log.Debug("[Abuse Report]: Shutting down Auctions module.");
        }

        public void PostInitialise()
        {
            // nah
        }

        public void RegionLoaded(Scene scene)
        {
            if (!m_reportsEnabled)
                return;

            lock (m_SceneList)
            {
                m_SceneList.Add(scene);
            }
        }

        public void RemoveRegion(Scene scene)
        {
            if (!m_reportsEnabled)
                return;

            lock (m_SceneList)
            {
                m_SceneList.Remove(scene);
            }
            m_scene.EventManager.OnNewClient -= handleNewClient;
        }
        public void HandleNewUserReport(IClientAPI client, string regionName, UUID abuserID, byte catagory, byte checkflags, string details, UUID objectID, Vector3 postion, byte reportType, UUID screenshotID, string Summary, UUID reporter)
        {
            NewUserReport(client, regionName, abuserID, catagory, checkflags, details, objectID, postion, reportType, screenshotID, Summary, reporter);
        }
        public IClientAPI NewUserReport(IClientAPI client, string regionName, UUID abuserID, byte catagory, byte checkflags, string details, UUID objectID, Vector3 postion, byte reportType, UUID screenshotID, string Summary, UUID reporter)
        {
            if (!m_reportsEnabled)
                return null;

            if (m_reportURI == String.Empty) 
                return null;

            m_log.InfoFormat("[Abuse Report]: Processing report from region {0}", regionName);

            OSDMap map = new OSDMap();

            map.Add("region", regionName);
            map.Add("abuser", abuserID);
            map.Add("cat", catagory);
            map.Add("check", checkflags);
            map.Add("details", details);
            map.Add("object", objectID);
            map.Add("pos", postion);
            map.Add("type", reportType);
            map.Add("img", screenshotID);
            map.Add("summary", Summary);
            map.Add("reporter", reporter);

            var httpWebRequest = (HttpWebRequest)WebRequest.Create(m_reportURI);
            httpWebRequest.ContentType = "application/json";
            httpWebRequest.Method = "POST";
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;

            using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
            {
                string json = OSDParser.SerializeJsonString(map);
                streamWriter.Write(json);
                streamWriter.Flush();
                streamWriter.Close();
            }

            var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
            using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
            {
                var result = streamReader.ReadToEnd();
                if (m_debugEnabled) m_log.InfoFormat("[Abuse Report]: SUCCESS with result {0}", result);
            }
            return null;
        }
    }
}
