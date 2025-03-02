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
            if(NotificationBroker is null)
                _ = Log.Error(new Exception("NotificationBroker is null"), "JSONInterpreter", "");
            NotificationTopic = config.GetValue("BrokerInfo").Value<string>("NotificationTopic");
            if(NotificationTopic is null)
                _ = Log.Error(new Exception("NotificationTopic is null"), "JSONInterpreter", "");
            ReportBroker = config.GetValue("BrokerInfo").Value<string>("ReportBroker");
            if(ReportBroker is null)
                _ = Log.Error(new Exception("ReportBroker is null"), "JSONInterpreter", "");
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"NotificationBroker: {NotificationBroker}, NotificationTopic: {NotificationTopic}, ReportBroker: {ReportBroker}");
            Console.ResetColor();
        }


        private List<SensorConfig> ReadSensorConfigs(JObject config) {
            List<SensorConfig> sensorConfigs = new List<SensorConfig>();
            JArray sensorNodes = null;
            try {
                sensorNodes = config.Value<JArray>("SensorNodes");
            }
            catch {
                _ = Log.Error(new Exception("sensorNodes is null"), "JSONInterpreter", "ReadSensorConfigs");
            }
            foreach(JObject sensorNode in sensorNodes) {
                try {
                    SensorConfig sensorConfig = new SensorConfig();
                    sensorConfig.id = (int)sensorNode.GetValue("ID");
                    sensorConfig.host = sensorNode.GetValue("Host").ToString();
                    switch(sensorNode.GetValue("SupportedType").ToString().ToLower()) {
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
                catch(InvalidCastException ICE) {
                    _ = Log.Error(ICE, "JSONInterpreter", "ReadSensorConfigs");
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine(ICE.Message);
                    Console.ResetColor();
                }
                catch(Exception e) {
                    _ = Log.Error(e, "JSONInterpreter", "ReadSensorConfigs");
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine(e.Message + " Failed to determine the exception type, update the code");
                    Console.ResetColor();
                }
            }
            return sensorConfigs;
        }

        private List<EventConfig> ReadEventConfigs(JObject config) {
            List<EventConfig> eventConfigs = new List<EventConfig>();
            JArray eventNodes = null;
            try {
                eventNodes = config.Value<JArray>("EventNodes");
            }
            catch {
                _ = Log.Error(new Exception("eventNodes is null"), "JSONInterpreter", "ReadEventConfigs");
            }
            foreach(JObject eventNode in eventNodes) {
                try {
                    EventConfig eventConfig = new EventConfig();
                    eventConfig.id = (int)eventNode.GetValue("ID");
                    switch(eventNode.GetValue("SupportedType").ToString().ToLower()) {
                        case "int":
                        eventConfig.type = Enums.SuportedTypes.INT;
                        break;
                        default:
                        _ = Log.Error(new Exception("Wrong or misconfigured Supported Type"), "JSONInterpreter", "Failed to read the Supported type, could be that it is misconfigured or using a type which is not supported");
                        break;
                    }
                    var n = eventNode.Value<JArray>("SensorNodes");
                    var ni = eventNode.Values<JArray>("SensorNodes").Children();

                    eventConfig.nodeIDs = eventNode.Value<JArray>("SensorNodes").ToObject<int[]>().ToList();
                    eventConfig.nodeIDs.AddRange(eventNode.Value<JArray>("EventNodes").ToObject<int[]>().ToList());
                    eventConfig.modelConfig = ToModel(eventNode.Value<JObject>("Model"));
                    eventConfig.reportTopic = eventNode.GetValue("ReportTopic").ToString();
                    eventConfig.host = eventNode.GetValue("Host").ToString();
                    eventConfigs.Add(eventConfig);
                }
                catch(InvalidCastException ICE) {
                    _ = Log.Error(ICE, "JSONInterpreter", "ReadEventConfigs");
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine(ICE.Message);
                    Console.ResetColor();
                }
                catch(Exception e) {
                    _ = Log.Error(e, "JSONInterpreter", "ReadEventConfigs");
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine(e.Message + " Failed to determine the exception type, update the code");
                    Console.ResetColor();
                }
            }
            return eventConfigs;
        }

        private ModelConfig ToModel(JObject config) {
            try {
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
                        modelConfig.modelType = "simple";
                        return modelConfig;
                    default:
                        _ = Log.Error(new Exception("Unable to determine model type"), "JSONInterpreter", "");
                        return modelConfig;
                }
            }
            catch(InvalidCastException ICE) {
                _ = Log.Error(ICE, "JSONInterpreter", "ToModel");
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(ICE.Message);
                Console.ResetColor();
            }
            catch(Exception e) {
                _ = Log.Error(e, "JSONInterpreter", "ToModel");
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(e.Message + " Failed to determine the exception type, update the code");
                Console.ResetColor();
            }
            return new ModelConfig();
        }

        public bool init() {
            try {
                StreamReader reader = new StreamReader(path + fileName);
                string json = reader.ReadToEnd();
                JObject config = JObject.Parse(json);
                SetConfigValues(config);
                List<SensorConfig> sensorConfigs = ReadSensorConfigs(config);
                List<EventConfig> eventConfigs = ReadEventConfigs(config);
                foreach(SensorConfig sensorConfig in sensorConfigs) {
                    nodes.Add(new SensorNode(sensorConfig));
                }
                foreach(EventConfig eventConfig in eventConfigs) {
                    nodes.Add(new EventNode(eventConfig));
                }
            }
            catch (FileNotFoundException fnfe) {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(fnfe.Message);
                Console.ResetColor();
                _ = Log.Error(fnfe, "JSONInterpreter", "");
                return false;
            }
            return true;
        }

        public List<INode> GetNodes() => nodes;
    }
}