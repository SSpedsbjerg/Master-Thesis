using REPS.Interfaces;
using REPS.Nodes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace REPS {
    //This controlls the relation between nodes and is the main point
    //of reference when a node need to get a reference to a node which
    //it needs to retrieve data from

    public static class NodeController {
        private static List<SensorNode> sensorNodes;
        private static List<EventNode> eventNodes;

        public static void init(List<INode> nodes) {
            sensorNodes = new List<SensorNode>();
            eventNodes = new List<EventNode>();
            foreach(INode node in nodes) {
                if(node.GetType() == typeof(SensorNode)) {
                    sensorNodes.Add(node as SensorNode);
                }
                else if(node.GetType() == typeof(EventNode)) {
                    eventNodes.Add(node as EventNode);
                }
            }
        }


        public static INode? GetNodeByID<T>(int id) {
            if(typeof(T) == typeof(SensorNode)) {
                foreach(SensorNode node in sensorNodes) {
                    if(node.ID == id)
                        return node;
                }
            }
            else if(typeof(T) == typeof(EventNode)) {
                foreach(EventNode node in eventNodes) {
                    if(node.ID == id)
                        return node;
                }
            }
            return null;
        }
    }
}
