using System;
using OPSSDK;
using OPSSDKCommon.Model.Call;
using Ozeki.Media.MediaHandlers;
using Ozeki.VoIP;


namespace ozeki.pbx.voip.client
{
    //These classes is just an example how to use OZEKI OPS SDK, what dll is added to the References in Solution Explorer
    //Feel free to use the other components of this SDK
    //For More information please visit : http://www.ozekiphone.com/-net-api-684.html and http://www.ozekiphone.com/ozeki-pbx-voip-client-998.html
    class CallHandlerSample
    {
        /// <summary>
        /// OpsClient will connect to the PBX, and handle communication
        /// </summary>
        private OpsClient _opsClient;

        /// <summary>
        /// IAPIExtension will forwarding the events and calls
        /// </summary>
        private IAPIExtension _apiExtension;

        /// <summary>
        /// This call object will be used during make and recieve call
        /// </summary>
        private ICall _call;

        private readonly Microphone _mic;
        private readonly Speaker _speaker;

        /// <summary>
        /// Event triggered when the connected API Extension has called
        /// </summary>
        public event EventHandler<VoIPEventArgs<ICall>> IncomingCallReceived;

        /// <summary>
        /// Handler of making call and receiving call
        /// </summary>
        /// <param name="serverAddress">The address of your Ozeki Phone System XE PBX</param>
        /// <param name="username">Valid Username for your Ozeki Phone System XE PBX</param>
        /// <param name="password">Valid Password for the given Username</param>
        /// <param name="apiExtensionId">The ID of an existing API Extension in PBX. You can receive Calls through that Extension.</param>
        public CallHandlerSample(string serverAddress, string username, string password, string apiExtensionId = null)
        {
            if (!TryCreateConnectToClient(serverAddress, username, password)) return;
            if (!TrySetApiExtension(apiExtensionId)) return;

            _mic = Microphone.GetDefaultDevice();
            _speaker = Speaker.GetDefaultDevice();
        }

        /// <summary>
        /// Create an OPS Client, and try to login into PBX with the given parameters
        /// </summary>
        /// <param name="serverAddress">The address of your Ozeki Phone System XE PBX</param>
        /// <param name="username">Valid Username for your Ozeki Phone System XE PBX</param>
        /// <param name="password">Valid Password for the given Username</param>
        /// <returns>Can or cannot connect to the PBX</returns>
        private bool TryCreateConnectToClient(string serverAddress, string username, string password)
        {
            _opsClient = new OpsClient();
            _opsClient.ErrorOccurred += ClientOnErrorOccurred;

            var result = _opsClient.Login(serverAddress, username, password);
            if (!result)
            {
                Console.WriteLine("Cannot connect to the server, please check the login details and the availability of your PBX! Press Enter to continue!");
                Console.ReadLine();
                return false;
            }

            return true;
        }

        /// <summary>
        /// Try to connect to an API extension, what will handle the communication from/to the PBX.
        /// </summary>
        /// <param name="apiExtensionId">The ID of selected API Extension. If not given, use of the default SYSTEM Api Extension. But in this case, you cannot receiving SMS</param>
        /// <returns>Can or cannot connect to the API Extension</returns>
        private bool TrySetApiExtension(string apiExtensionId)
        {
            if (_opsClient == null)
                return false;

            _apiExtension = string.IsNullOrWhiteSpace(apiExtensionId)
                               ? _opsClient.GetAPIExtension()
                               : _opsClient.GetAPIExtension(apiExtensionId);

            if (_apiExtension == null)
            {
                Console.WriteLine("Cannot find API Extension. Press Enter to continue!");
                Console.ReadLine();
                return false;
            }

            SubscribeApiExtensionEvents();
            return true;
        }

        /// <summary>
        /// Create and start the call to the dialed number
        /// </summary>
        /// <param name="dialedNumber"></param>
        public void Call(string dialedNumber)
        {
            CreateCall(dialedNumber);
        }

        #region Outgoing Calls
        /// <summary>
        /// Creating and Starting Call through Ozeki Phone System XE
        /// </summary>
        /// <param name="dialedNumber">The (telephone)number of the recipient device</param>
        private void CreateCall(string dialedNumber)
        {
            if (_apiExtension == null)
                return;

            if (_call != null)
            {
                Console.WriteLine("A call already in progress. Cannot handle another call until that.");
                return;
            }

            _call = _apiExtension.CreateCall(dialedNumber);
            SubscribeCallEvents();

            Console.WriteLine("Outgoing call (" + _call.OtherParty + ") started.");
            _call.Start();
        }


        #endregion

        #region Incoming Calls
        ///// <summary>
        ///// Event triggered, when the API Extension we have connected to recieved a call. Then the call is forwarded to here.
        ///// </summary>
        private void apiExtension_IncomingCall(object sender, VoIPEventArgs<ICall> e)
        {
            if (_call != null)
            {
                e.Item.Reject();
                Console.WriteLine("A call already in progress. Cannot handle another call until that.");
                return;
            }

            _call = e.Item;

            if (_call.CallState == CallState.Ringing)
            {
                Console.WriteLine("Incoming call (" + _call.OtherParty + ") accepted.");
                SubscribeCallEvents();
                OnIncomingCallReceived(e.Item);
            }
        }
        #endregion

        #region Call events
        private void SubscribeCallEvents()
        {
            if (_call != null)
            {
                _call.CallStateChanged += call_CallStateChanged;
                _call.CallErrorOccurred += call_CallErrorOccurred;
            }
        }

        private void UnsubscribeCallEvents()
        {
            if (_call != null)
            {
                _call.CallStateChanged -= call_CallStateChanged;
                _call.CallErrorOccurred -= call_CallErrorOccurred;
            }
        }

        private void call_CallStateChanged(object sender, VoIPEventArgs<CallState> e)
        {
            if (e.Item.IsInCall())
            {
                Console.WriteLine("In call with " + _call.OtherParty + ".");
                ConnectDevicesToCall();
            }
            else if (e.Item.IsCallEnded())
            {
                UnsubscribeCallEvents();
                DisconnectDevicesFromCall();
                Console.WriteLine("Outgoing call (" + _call.OtherParty + ") ended.");

                _call = null;
            }
        }

        private void call_CallErrorOccurred(object sender, VoIPEventArgs<CallError> e)
        {
            Console.WriteLine(e.Item + " Press Enter to Exit!");
            Console.ReadLine();

            UnsubscribeCallEvents();
            DisconnectDevicesFromCall();
            _call = null;
        }

        private void OnIncomingCallReceived(ICall item)
        {
            var handler = IncomingCallReceived;

            if (handler != null)
                handler(this, new VoIPEventArgs<ICall>(item));
        }
        #endregion

        /// <summary>
        /// An error has been occured during communication with the PBX
        /// </summary>
        /// <param name="sender">Information about the sender</param>
        /// <param name="e">Information about the error</param>
        private void ClientOnErrorOccurred(object sender, ErrorInfo e)
        {
            Console.WriteLine(e.Message + " Press Enter to exit!");
            UnsubscribeApiExtensionEvents();
            Console.ReadLine();
            Environment.Exit(0);
        }

        /// <summary>
        /// Connecting the microphone and speaker to the call
        /// </summary>
        private void ConnectDevicesToCall()
        {
            if (_call == null)
                return;

            _call.ConnectAudioSender(_mic);
            _call.ConnectAudioReceiver(_speaker);

            _mic.Start();
            _speaker.Start();
        }

        /// <summary>
        /// Disconnecting the microphone and speaker from the call
        /// </summary>
        private void DisconnectDevicesFromCall()
        {
            if (_call == null)
                return;

            _call.DisconnectAudioSender(_mic);
            _call.DisconnectAudioReceiver(_speaker);
        }

        private void SubscribeApiExtensionEvents()
        {
            if (_apiExtension != null)
                _apiExtension.IncomingCall += apiExtension_IncomingCall;
        }

        private void UnsubscribeApiExtensionEvents()
        {
            if (_apiExtension != null)
                _apiExtension.IncomingCall -= apiExtension_IncomingCall;
        }

        ~CallHandlerSample()
        {
            UnsubscribeApiExtensionEvents();
            UnsubscribeCallEvents();

            if (_mic != null)
                _mic.Dispose();

            if (_speaker != null)
                _speaker.Dispose();

            _call = null;
        }
    }
}
