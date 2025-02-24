using Newtonsoft.Json.Linq;
using REPS.Enums;
using REPS.Interfaces;
using REPS.Models;
using REPS.Nodes;
using REPS.Structs;
using System;
using System.Collections.Generic;
using System.IO.Pipelines;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace REPS {
    public class JSONInterpreter {
        string path;
        string fileName;
        private List<INode> nodes;
        public string NotificationBroker;
        public string NotificationTopic;
        public string ReportBroker;
        public UpdateRate updateRate;

        public JSONInterpreter(string path, string fileName) {
            this.path = path;
            this.fileName = fileName;
            nodes = new List<INode>();
        }

        private void SetConfigValues(JObject config) {
            NotificationBroker = config.GetValue("BrokerInfo").Value<string>("NotificationBroker");
            NotificationTopic = config.GetValue("BrokerInfo").Value<string>("NotificationTopic");
            ReportBroker = config.GetValue("BrokerInfo").Value<string>("ReportBroker");
        }


        private List<SensorConfig> ReadSensorConfigs(JObject config) {
            List<SensorConfig> sensorConfigs = new List<SensorConfig>();
            JArray sensorNodes = config.Value<JArray>("SensorNodes");
            foreach(JObject sensorNode in sensorNodes) {
                SensorConfig sensorConfig = new SensorConfig();
                sensorConfig.id = (int)sensorNode.GetValue("ID");
                sensorConfig.host = sensorNode.GetValue("Host").ToString();
                switch(sensorNode.GetValue("SupoortedType").ToString().ToLower()) {
                    case "int":
                    sensorConfig.type = Enums.SuportedTypes.INT;
                    break;
                    default:
                    _ = Log.Error(new Exception("Wrong or misconfigured Supported Type"), "JSONInterpreter", "Failed to read the Supported type, could be that it is misconfigured or using a type which is not supported");
                    break;
                }
                sensorConfig.topic = sensorNode.GetValue("Topic").ToString();
                sensorConfig.routingKey = sensorNode.GetValue("RoutingKey").ToString();
                sensorConfigs.Add(sensorConfig);
            }
            return sensorConfigs;
        }

        private List<EventConfig> ReadEventConfigs(JObject config) {
            List<EventConfig> eventConfigs = new List<EventConfig>();
            JArray eventNodes = config.Value<JArray>("EventNodes");
            foreach(JObject eventNode in eventNodes) {
                EventConfig eventConfig = new EventConfig();
                eventConfig.id = (int)eventNode.GetValue("ID");
                switch(eventNode.GetValue("SupoortedType").ToString().ToLower()) {
                    case "int":
                        eventConfig.type = Enums.SuportedTypes.INT;
                        break;
                    default:
                        _ = Log.Error(new Exception("Wrong or misconfigured Supported Type"), "JSONInterpreter", "Failed to read the Supported type, could be that it is misconfigured or using a type which is not supported");
                        break;
                }
                eventConfig.nodeIDs = eventNode.Value<JArray>("SensorNodes").ToObject<int[]>().ToList();
                eventConfig.nodeIDs.AddRange(eventNode.Value<JArray>("EventNodes").ToObject<int[]>().ToList());
                eventConfig.model = ToModel(eventNode);
                eventConfig.reportTopic = eventNode.GetValue("ReportTopic").ToString();
                eventConfigs.Add(eventConfig);
            }
            return eventConfigs;
        }

        private IModel ToModel(JObject config) {
            ModelConfig modelConfig = new ModelConfig();
            modelConfig.id = (int)config.GetValue("ID");
            switch(config.GetValue("SupportedType").ToString().ToLower()) {
                case "int":
                    modelConfig.type = Enums.SuportedTypes.INT;
                    break;
                default:
                    _ = Log.Error(new Exception("Wrong or misconfigured Supported Type"), "JSONInterpreter", "Failed to read the Supported type, could be that it is misconfigured or using a type which is not supported");
                    break;
            }
            modelConfig.function = config.GetValue("Function").ToString();
            modelConfig.parameters = config.Value<JArray>("Parameters").ToObject<string[]>().ToList();
            modelConfig.testvalue = (int)config.GetValue("TestValue");
            modelConfig.TestParameterValues = config.Value<JArray>("TestParameterValues").ToObject<double[]>().ToList();
            modelConfig.testTopic = config.GetValue("TestTopic").ToString();
            switch(config.GetValue("Type").ToString().ToLower()) {
                case "simple":
                    return new SimpleModel(modelConfig);
            }
            return null;
        }

        public bool init() {
            StreamReader reader = new StreamReader(path + fileName);
            string json = reader.ReadToEnd();
            JObject config = JObject.Parse(json);
            SetConfigValues(config);
            List<SensorConfig> sensorConfigs = ReadSensorConfigs(config);
            List<EventConfig> eventConfigs = ReadEventConfigs(config);
            
            return true;
        }

        public List<INode> GetNodes() => nodes;

    }
}
