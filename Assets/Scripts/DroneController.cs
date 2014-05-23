using UnityEngine;
using System.Collections;
using AR.Drone.Client;
using AR.Drone.Video;
using AR.Drone.Data;
using AR.Drone.Data.Navigation;
using FFmpeg.AutoGen;
using XInputDotNetPure;
using NativeWifi;

public class DroneController : MonoBehaviour {

	// Stick which is moved analogical to the movement of the gamepad stick
	public Transform Stick;
	// Modifies the stick rotation
	public float StickRotationModifier = 0.15f;
	// Plane on which the main camera is mapped
	public Renderer MainRenderer;
	// Plane on which the secondary camera is mapped
	public Renderer SecondaryRenderer;
	// Rotation limit for the switch between the main camera and the secondary camera
	public float SwitchRotation = 0.4f;
	// The camera used for the switch test
	public Transform CameraForSwitchCheck;
	// Ambient Light
	public Light[] AmbientLights;
	// Status text
	public TextMesh StatusText;
	// Wifi status
	public TextMesh WifiText;
	public TextMesh WifiChart;
	public int maxChartBars = 20;

	// Gamepad variables
	private bool playerIndexSet = false; 
	private XInputDotNetPure.PlayerIndex playerIndex;
	private GamePadState state;
	private GamePadState prevState;

	// Indicates that the drone is landed
	private bool isLanded = true;
	// Indicates that the startButton is Pressed
	private bool startButtonPressed = false;
	// Texture used for the camera content
	private Texture2D cameraTexture;
	// A black texture used for the inactive plane
	private Texture2D blackTexture;
	// byte array which will be filled by the camera data
	private byte[] data;
	// Drone variables
	private VideoPacketDecoderWorker videoPacketDecoderWorker;
	private DroneClient droneClient;
	private NavigationData navigationData;

	// Width and height if the camera
	private int width = 640;
	private int height = 360;

	// wlanclient for signal strength
	private WlanClient client;
	
	// Use this for initialization
	void Start () {
		Debug.Log("Start");
		// initialize data array
		data = new byte[width*height*3];

		// set textures
		MainRenderer.material.mainTexture = cameraTexture;
		SecondaryRenderer.material.mainTexture = blackTexture;
		cameraTexture = new Texture2D (width, height);
		blackTexture = new Texture2D (1, 1);
		blackTexture.SetPixel (0, 0, Color.black);
		blackTexture.Apply ();

		// Initialize drone
		videoPacketDecoderWorker = new VideoPacketDecoderWorker(PixelFormat.BGR24, true, OnVideoPacketDecoded);
		videoPacketDecoderWorker.Start();
		droneClient = new DroneClient("192.168.1.1");
		droneClient.UnhandledException += HandleUnhandledException;
		droneClient.VideoPacketAcquired += OnVideoPacketAcquired;
		droneClient.NavigationDataAcquired += navData => navigationData = navData;
		videoPacketDecoderWorker.UnhandledException += HandleUnhandledException;
		droneClient.Start ();

		// activate main drone camera
		switchDroneCamera (AR.Drone.Client.Configuration.VideoChannelType.Vertical);

		// determine connection
		client = new WlanClient();
	}

	// Update is called once per frame
	void Update () {

		convertCameraData ();

		updateGamepadState ();

		moveStick ();

		// Start or land the drone
		if (state.Buttons.Start.Equals(ButtonState.Pressed) && !startButtonPressed) {
			if (isLanded)
				droneClient.Takeoff();
			else
				droneClient.Land();
			isLanded = !isLanded;
			startButtonPressed = true;
		}
		if (!state.Buttons.Start.Equals(ButtonState.Pressed))
			startButtonPressed = false;

		// exit application
		if (Input.GetKey("escape") || state.Buttons.Back.Equals (ButtonState.Pressed))
			Application.Quit ();

		// Move the drone
		var pitch = -state.ThumbSticks.Left.Y;
		var roll = state.ThumbSticks.Left.X;
		var gaz = state.Triggers.Right - state.Triggers.Left;
		var yaw = state.ThumbSticks.Right.X;
		droneClient.Progress(AR.Drone.Client.Command.FlightMode.Progressive, pitch: pitch, roll: roll, gaz: gaz, yaw: yaw); 

		// Switch drone camera
		if (CameraForSwitchCheck.rotation.x >= SwitchRotation) {
			if (SecondaryRenderer.material.mainTexture != cameraTexture) {
				MainRenderer.material.mainTexture = blackTexture;
				SecondaryRenderer.material.mainTexture = cameraTexture;
				switchDroneCamera (AR.Drone.Client.Configuration.VideoChannelType.Vertical);
			}
		} else {
			if (MainRenderer.material.mainTexture != cameraTexture) {
				SecondaryRenderer.material.mainTexture = blackTexture;
				MainRenderer.material.mainTexture = cameraTexture;
				switchDroneCamera (AR.Drone.Client.Configuration.VideoChannelType.Horizontal);
			}
		}

		// set status text
		if (navigationData != null) {
			StatusText.text = string.Format("Battery: {0} % \nYaw: {1:f} \nPitch: {2:f} \nRoll: {3:f} \nAltitude: {4} m",
			                                navigationData.Battery.Percentage,navigationData.Yaw, navigationData.Pitch,
			                                navigationData.Roll,navigationData.Altitude);
		}

		// determine wifi strength 
		determineWifiStrength ();
	}

	// Called if the gameobject is destroyed
	void OnDestroy(){
		droneClient.Land();
		droneClient.Stop();
		droneClient.Dispose ();
		videoPacketDecoderWorker.Stop ();
		videoPacketDecoderWorker.Dispose();
	}

	/// <summary>
	/// Log the unhandled exception.
	/// </summary>
	/// <param name="arg1">Arg1.</param>
	/// <param name="arg2">Arg2.</param>
	void HandleUnhandledException (object arg1, System.Exception arg2)
	{
		Debug.Log (arg2); 
	}

	/// <summary>
	/// Switchs the drone camera.
	/// </summary>
	/// <param name="Type">Video channel type.</param>
	private void switchDroneCamera(AR.Drone.Client.Configuration.VideoChannelType Type){
		var configuration = new AR.Drone.Client.Configuration.Settings();
		configuration.Video.Channel = Type;
		droneClient.Send(configuration);
	}

	/// <summary>
	/// Converts the camera data to a color array and applies it to the texture.
	/// </summary>
	private void convertCameraData(){
		int r = 0;
		int g = 0;
		int b = 0;
		int total = 0;
		var colorArray = new Color32[data.Length/3];
		for(var i = 0; i < data.Length; i+=3)
		{
			colorArray[i/3] = new Color32(data[i + 2], data[i + 1], data[i + 0], 1);
			r += data[i + 2];
			g += data[i + 1];
			b += data[i + 0];
			total++;
		}
		r /= total;
		g /= total;
		b /= total;
		cameraTexture.SetPixels32(colorArray);
		cameraTexture.Apply();
		foreach (Light light in AmbientLights)
			light.color = new Color32 (System.Convert.ToByte(r), System.Convert.ToByte(g), System.Convert.ToByte(b), 1);
	}

	/// <summary>
	/// Updates the state of the gamepad.
	/// </summary>
	void updateGamepadState(){
		// Find a PlayerIndex, for a single player game
		// Will find the first controller that is connected and use it
		if (!playerIndexSet || !prevState.IsConnected)
		{
			for (int i = 0; i < 4; ++i)
			{
				PlayerIndex testPlayerIndex = (PlayerIndex)i;
				GamePadState testState = GamePad.GetState(testPlayerIndex);
				if (testState.IsConnected)
				{
					Debug.Log(string.Format("GamePad found {0}", testPlayerIndex));
					playerIndex = testPlayerIndex;
					playerIndexSet = true;
				}
			}
		}
		
		prevState = state;
		state = GamePad.GetState(playerIndex);
	}

	/// <summary>
	/// Determines what happens if a video packet is acquired.
	/// </summary>
	/// <param name="packet">Packet.</param>
	private void OnVideoPacketAcquired(VideoPacket packet)
	{
		if (videoPacketDecoderWorker.IsAlive)
			videoPacketDecoderWorker.EnqueuePacket(packet);
	}

	/// <summary>
	/// Determines what happens if a video packet is decoded.
	/// </summary>
	/// <param name="frame">Frame.</param>
	private void OnVideoPacketDecoded(VideoFrame frame)
	{
		data = frame.Data;
	}

	/// <summary>
	/// Sets the rotation of the stick
	/// </summary>
	private void moveStick(){
		var newRotation = Stick.rotation;
		newRotation.x = StickRotationModifier* state.ThumbSticks.Left.Y;
		newRotation.z =	-StickRotationModifier * state.ThumbSticks.Left.X;
		Stick.rotation = newRotation;
	}

	/// <summary>
	/// Determine the wifi strength.
	/// </summary>
	private void determineWifiStrength(){
		int signalQuality = 0;
		foreach (WlanClient.WlanInterface wlanIface in client.Interfaces)
		{
			try {
				signalQuality = (int)wlanIface.CurrentConnection.wlanAssociationAttributes.wlanSignalQuality;
			}
			catch (System.Exception e ){
				Debug.Log ("No Wifi Connection");
			}
		}

		if (signalQuality != 0) {
			WifiChart.text = new string('|',signalQuality / (100 / maxChartBars));
			WifiText.text = "Wifi: " + signalQuality.ToString() + "%";
		}
		else {
			WifiChart.text = "";
			WifiText.text = "Wifi: 0%";
		}
	}
}
