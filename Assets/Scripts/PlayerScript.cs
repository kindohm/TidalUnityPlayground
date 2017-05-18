using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityOSC;

public class PlayerScript : MonoBehaviour
{

    private Queue queue;
    GameObject cube;
    GameObject cylinder;
    GameObject sphere;
    GameObject capsule;

    // Use this for initialization
    void Start()
    {
        
        Debug.Log("teh player");

        OSCHandler.Instance.Init();// TargetAddr, OutGoingPort, InComingPort);

        queue = new Queue();
        queue = Queue.Synchronized(queue);

        var server = OSCHandler.Instance.Servers["TidalClient"].server;
        server.PacketReceivedEvent += OnPacketReceived;

        Debug.Log("Loading shapes.");
        cube = Resources.Load("Cube", typeof(GameObject)) as GameObject;
        cylinder = Resources.Load("Cylinder", typeof(GameObject)) as GameObject;
        capsule = Resources.Load("Capsule", typeof(GameObject)) as GameObject;
        sphere = Resources.Load("Sphere", typeof(GameObject)) as GameObject;

    }

    void OnPacketReceived(OSCServer server, OSCPacket packet)
    {
        Debug.Log("recieved");
        queue.Enqueue(packet);
    }

    void Update()
    {
        //Debug.Log("Updating");
        if (queue == null)
        {
            //Debug.Log("nada");
            return;
        }


        while (0 < queue.Count)
        {
            
            //Debug.Log("HAS STUFF");
            OSCPacket packet = queue.Dequeue() as OSCPacket;
            if (packet.IsBundle())
            {
                // if OSCBundl
                OSCBundle bundle = packet as OSCBundle;
                foreach (OSCMessage msg in bundle.Data)
                {

                }
            }
            else
            {
                OSCMessage msg = packet as OSCMessage;
                //Debug.Log(msg.Address);
                if ("/unity_osc" == msg.Address)
                {
                    //print (msg.Data[1].ToString());
                    var thing = msg.Data[1].ToString();
                    var pos = new Vector3((float)msg.Data[2], (float)msg.Data[3], (float)msg.Data[4]);

                    //rb.MovePosition(pos);

                    var dur = (float)packet.Data[5];

                    AppendItem(thing, pos, dur);
                }
            }
        }


    }

    void AppendItem(String item, Vector3 pos, float dur)
    {
        Debug.Log("AppendItem()");
        GameObject thing = null;
        switch (item)
        {
            case "cube":
                thing = cube;
                break;
            case "sphere":
                thing = sphere;
                break;
            case "capsule":
                thing = capsule;
                break;
            case "cylinder":
                thing = cylinder;
                break;
        }

        if (thing == null)
        {
            return;
        }
        
        var rotation = new Quaternion(UnityEngine.Random.Range(-10, 10), UnityEngine.Random.Range(-10, 10), UnityEngine.Random.Range(-10, 10), UnityEngine.Random.Range(-10, 10));
        
        var clone = Instantiate(thing, pos, rotation);
        clone.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);

        Destroy(clone, dur);
    }

}
