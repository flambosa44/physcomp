using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ESP32 : MonoBehaviour
{
    public string DeviceName;
    public string ServiceUUID;
    private string deviceAddress = null;
    private States state = States.None;
    private float timeout = 0f;
    private bool connected = false;
    private bool foundSubscribeID = false;
    private bool foundWriteID = false;
    private byte[] dataBytes = null;
    private bool rssiOnly = false;
    private int rssi = 0;
    private int vibrateValue = 255;
    private int lastByteSent = 255;
    public UnityEngine.UI.Text stateText;
    private bool found = false;
    private bool reset = false;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public bool IsReset
    {
        get { return this.reset; }
        set { this.reset = value; }
    }
    public bool Found
    {
        get { return this.found; }
        set { this.found = value; }
    }
    public string Address
    {
        get { return this.deviceAddress; }
        set { this.deviceAddress = value; }
    }

    public States State
    {
        get { return this.state; }
        set { this.state = value; }
    }

    public float Timeout
    {
        get { return this.timeout; }
        set { this.timeout = value; }
    }

    public bool Connected 
    { get { return this.connected; } set { this.connected = value; } }

    public bool FoundSubscribeID
    { get { return this.foundSubscribeID; } set { this.foundSubscribeID = value; } }

    public bool FoundWriteID
    { get { return this.foundWriteID; } set { this.foundWriteID = value; } }

    public byte[] DataBytes
    { get { return this.dataBytes; } set { this.dataBytes = value; } }

    public bool RssiOnly
    { get { return this.rssiOnly; } set { this.rssiOnly = value; } }

    public int Rssi
    { get { return this.rssi; } set { this.rssi = value; } }

    public int VibrateValue
    { get { return this.vibrateValue; } set { this.vibrateValue = value; } }

    public int LastByteSent
    { get { return this.lastByteSent; } set { this.lastByteSent = value; } }

    public void Reset()
    {
        connected = false;
        timeout = 0f;
        state = States.None;
        deviceAddress = null;
        foundSubscribeID = false;
        foundWriteID = false;
        dataBytes = null;
        rssi = 0;
        reset = false;
    }

    public void SetState(States newState, float timeout)
    {
        this.state = newState;
        this.timeout = timeout;
    }

    public void SetCollisionDetected(bool activate, int vibrateValue)
    {
        this.vibrateValue = activate ? vibrateValue : 100;
    }

    public enum States
    {
        None,
        Scan,
        ScanRSSI,
        Connect,
        Subscribe,
        Unsubscribe,
        Disconnect,
    }
}
