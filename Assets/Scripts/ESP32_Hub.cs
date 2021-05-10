using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ESP32_Hub : MonoBehaviour
{
    //public string DeviceName;
    //public string ServiceUUID;
    //public string SubscribeCharacteristic;
    //public string WriteCharacteristic;

    enum States
    {
        None,
        Scan,
        ScanRSSI,
        Connect,
        Subscribe,
        Unsubscribe,
        Disconnect,
    }

    //private bool _connected = false;
    //private float _timeout = 0f;
    //private States _state = States.None;
    //private string _deviceAddress;
    //private bool _foundSubscribeID = false;
    //private bool _foundWriteID = false;
    //private byte[] _dataBytes = null;
    //private bool _rssiOnly = false;
    //private int _rssi = 0;
    //public GameObject bluetooth;
    private List<ESP32> devices = new List<ESP32>();
    private Dictionary<string,ESP32> devicesDict = new Dictionary<string, ESP32>();
    private List<string> deviceUUIDs = new List<string>();
    [SerializeField]
    private UnityEngine.UI.Text outputText;
    [SerializeField]
    private Text scanText;
    [SerializeField]
    private Text DeviceNameFoundedText;
    [SerializeField]
    private GameObject cube;
    private Transform cubeTransform;
    private bool processing = false;
    private bool initialised = false;
    private bool scanning = false;
    private bool connecting = false;

    //   [SerializeField]3
    //   private Transform cameraTransform;
    private Quaternion quatLookAtCam;
    private Quaternion rawQuat;
    [SerializeField]
    private GameObject particle;

    public void resetQuat()
    {
        //Quaternion cubeQuat = Quaternion.LookRotation(this.transform.position-cubeTransform.position);
        //quatLookAtCam = cubeQuat * Quaternion.Inverse(rawQuat);
        //quatLookAtCam = Quaternion.Inverse(cubeTransform.rotation);
        quatLookAtCam = Quaternion.Inverse(rawQuat);
        GameObject go = Instantiate(particle);
        GameObject.Destroy(go, 1f);
    }


    //void Reset()
    //{
    //    _connected = false;
    //    _timeout = 0f;
    //    _state = States.None;
    //    _deviceAddress = null;
    //    _foundSubscribeID = false;
    //    _foundWriteID = false;
    //    _dataBytes = null;
    //    _rssi = 0;
    //}

    //void SetState(States newState, float timeout)
    //{
    //    _state = newState;
    //    _timeout = timeout;
    //}

void StartProcess()
    {
        //Reset();
        foreach (ESP32 device in devices)
        {
            device.Reset();
            deviceUUIDs.Add(device.ServiceUUID);
        }

    }

    // Use this for initialization
    void Start()
    {
        //foreach (Transform trans in this.gameObject.transform)
        //    devices.Add(trans.gameObject.GetComponent<ESP32>());
        devices.AddRange(FindObjectsOfType<ESP32>());
        foreach (ESP32 device in devices)
            devicesDict.Add(device.DeviceName, device);
        //devices.Add(bluetooth.GetComponent<ESP32>());
        StartProcess();
        StartCoroutine("Init");
        //cubeTransform = cube.GetComponent<Transform>();
        //quatLookAtCam = Quaternion.identity;
    }

    IEnumerator Init()
    {
        initialised = false;
        BluetoothLEHardwareInterface.Initialize(true, false, () => {

            foreach (ESP32 device in devices)
            {
                device.SetState(ESP32.States.Scan, 0.1f);
                device.stateText.text = device.DeviceName + " " + device.State;
            }
            initialised = true;
        }, (error) => {
            BluetoothLEHardwareInterface.Log("Error during initialize: " + error);
        });
        while(!initialised)
        {
            yield return null;
        }
        StartCoroutine("Scan");
    }

    IEnumerator Scan()
    {
        scanning = true;
        BluetoothLEHardwareInterface.ScanForPeripheralsWithServices(null, (address, name) =>
        {
            //DeviceNameFoundedText.text += name + "\n";
            // if your device does not advertise the rssi and manufacturer specific data
            // then you must use this callback because the next callback only gets called
            // if you have manufacturer specific data
            //device.stateText.text += "\n" + name + " " + count2 + " " + device.cnt;
            if (devicesDict.ContainsKey(name) && !devicesDict[name].Found)
            {
                ESP32 device = devicesDict[name];
                device.Found = true;
                device.Address = address;
                device.SetState(ESP32.States.Connect, 0.5f);
                int cntMissing = 0;
                device.stateText.text = "NEW: " + device.DeviceName + " " + device.Address;
                foreach (ESP32 dev in devices)
                {
                    if (!dev.Found)
                        cntMissing++;
                }
                if (cntMissing == 0)
                {
                    BluetoothLEHardwareInterface.StopScan();
                    scanning = false;
                    connecting = true;
                }
                //BluetoothLEHardwareInterface.StopScan();

                // found a device with the name we want
                // this example does not deal with finding more than one

                //device.SetState(ESP32.States.Connect, 0.5f);
            }
        }, null, false);

        while(scanning)
        {
            yield return null;
        }
        scanText.text = "FINISHED SCANNING!";
        StartCoroutine("Connect");
    }

    IEnumerator Connect()
    {
        foreach(ESP32 device in devices)
        {
            device.FoundSubscribeID = false;
            device.FoundWriteID = false;

            // note that the first parameter is the address, not the name. I have not fixed this because
            // of backwards compatiblity.
            // also note that I am note using the first 2 callbacks. If you are not looking for specific characteristics you can use one of
            // the first 2, but keep in mind that the device will enumerate everything and so you will want to have a timeout
            // large enough that it will be finished enumerating before you try to subscribe or do any other operations.
            BluetoothLEHardwareInterface.ConnectToPeripheral(device.Address, null, null, (address, serviceUUID, characteristicUUID) =>
            {
                if (IsEqual(serviceUUID, device.ServiceUUID))
                {
                    device.FoundSubscribeID = device.FoundSubscribeID || IsEqual(characteristicUUID, device.ServiceUUID);
                    device.FoundWriteID = device.FoundWriteID || IsEqual(characteristicUUID, device.ServiceUUID);

                    // if we have found both characteristics that we are waiting for
                    // set the state. make sure there is enough timeout that if the
                    // device is still enumerating other characteristics it finishes
                    // before we try to subscribe
                    if (device.FoundSubscribeID)// && _foundWriteID)
                    {
                        device.stateText.text = device.DeviceName + " CONNECTED!";
                        device.SetState(ESP32.States.Subscribe, 2f);
                        device.Connected = true;
                    }
                }
            });
            while (!device.Connected)
            {
                yield return null;
            }
        }
        scanText.text = "FINISHED CONNECTING!";
        StartCoroutine("Run");
    }

    // Update is called once per frame
    IEnumerator Run()
    {
        while(true)
        {
            foreach (ESP32 device in devices)
            {
                if (device.Timeout > 0f)
                {
                    device.Timeout -= Time.deltaTime;
                    if (device.Timeout <= 0f)
                    {
                        device.Timeout = 0f;
                        EvaluateBLEStates(device);
                    }
                }
            }
            yield return null;
        }

    }

    int count = 0;
    int count2 = 0;
    void EvaluateBLEStates(ESP32 device)
    {
        switch (device.State)
        {
            case ESP32.States.None:
                break;

            case ESP32.States.Subscribe:
                BluetoothLEHardwareInterface.SubscribeCharacteristicWithDeviceAddress(device.Address, device.ServiceUUID, device.ServiceUUID, null, (address, characteristicUUID, bytes) => {

                    // we don't have a great way to set the state other than waiting until we actually got
                    // some data back. For this demo with the rfduino that means pressing the button
                    // on the rfduino at least once before the GUI will update.
                    device.State = ESP32.States.None;

                    // we received some data from the device
                    device.DataBytes = bytes;
                    //outputText.text = System.Text.Encoding.UTF8.GetString(_dataBytes, 0, _dataBytes.Length);
                    OnLED(device);
                    //float qx = System.BitConverter.ToSingle(bytes, 0);
                    //float qy = System.BitConverter.ToSingle(bytes, 8);
                    //float qz = System.BitConverter.ToSingle(bytes, 4);
                    //float qw = System.BitConverter.ToSingle(bytes, 12)*-1;

                    //rawQuat = new Quaternion(qx, qy, qz, qw);
                    //cubeTransform.rotation = quatLookAtCam*rawQuat;

                    ////DeviceNameFoundedText.text = qx.ToString() + " " + qy.ToString() + " " + qz.ToString() + " " + qw.ToString()+"\n";
                    //DeviceNameFoundedText.text = "raw : "+rawQuat.ToString()+"\n";
                    //DeviceNameFoundedText.text += "cube : "+cubeTransform.rotation.ToString();

                });
                break;

            case ESP32.States.Unsubscribe:
                BluetoothLEHardwareInterface.UnSubscribeCharacteristic(device.Address, device.ServiceUUID, device.ServiceUUID, null);
                device.SetState(ESP32.States.Disconnect, 4f);
                break;

            case ESP32.States.Disconnect:
                if (device.Connected)
                {
                    BluetoothLEHardwareInterface.DisconnectPeripheral(device.Address, (address) => {
                        BluetoothLEHardwareInterface.DeInitialize(() => {

                            device.Connected = false;
                            device.State = ESP32.States.None;
                        });
                    });
                }
                else
                {
                    BluetoothLEHardwareInterface.DeInitialize(() => {

                        device.State = ESP32.States.None;
                    });
                }
                break;
        }
    }

    private bool ledON = false;
    public void OnLED(ESP32 device)
    {
        if(!device.IsReset)
        {
            device.VibrateValue = 100;
            SendByte(device, 0x02, (byte)device.VibrateValue);
            device.LastByteSent = device.VibrateValue;
            device.IsReset = true;
            return;

        }
        if (device.VibrateValue == device.LastByteSent)
            return;
        //if (collisionDetected)
        //{
        //    SendByte((byte)0x01);
        //}
        //else
        //{
        //    SendByte((byte)0x00);
        //}
        SendByte(device, 0x01,(byte)device.VibrateValue);
        device.LastByteSent = device.VibrateValue;
    }

    string FullUUID(string uuid)
    {
        return "0000" + uuid + "-0000-1000-8000-00805f9b34fb";
    }

    bool IsEqual(string uuid1, string uuid2)
    {
        if (uuid1.Length == 4)
            uuid1 = FullUUID(uuid1);
        if (uuid2.Length == 4)
            uuid2 = FullUUID(uuid2);

        return (uuid1.ToUpper().CompareTo(uuid2.ToUpper()) == 0);
    }

    void SendByte(ESP32 device, byte flag, byte value)
    {
        byte[] data = new byte[] { flag, value };
        BluetoothLEHardwareInterface.WriteCharacteristic(device.Address, device.ServiceUUID, device.ServiceUUID, data, data.Length, true, (characteristicUUID) => {

            BluetoothLEHardwareInterface.Log("Write Succeeded");
        });
    }

    //private bool collisionDetected = false;
    //public void SetCollisionDetected(bool activate, int vibrateValue)
    //{
    //    this.collisionDetected = activate;
    //    device.VibrateValue = activate ? vibrateValue : 100;
    //}
}
