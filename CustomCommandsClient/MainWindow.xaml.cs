using CustomCommandsClient.Models;
using Microsoft.Bot.Schema;
using Microsoft.CognitiveServices.Speech;
using Microsoft.CognitiveServices.Speech.Audio;
using Microsoft.CognitiveServices.Speech.Dialog;
using NAudio.Wave;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using Debug = System.Diagnostics.Debug;

namespace CustomCommandsClient
{
    public partial class MainWindow : Window
    {
        private AudioConfig audioConfig = null;
        private DialogServiceConnector connector = null;
        private KeywordRecognizer keywordRecognizer = null;
        private KeywordRecognitionModel keywordRecognitionModel = null;

        private readonly WaveOutEvent player = new WaveOutEvent();
        private readonly Queue<RawSourceWaveStream> playbackStreams = new Queue<RawSourceWaveStream>();

        private bool waitingForUserInput = false;
        private ListenState listeningState = ListenState.NotListening;

        public ObservableCollection<MessageDisplay> Messages { get; private set; } = new ObservableCollection<MessageDisplay>();

        public MainWindow()
        {
            InitializeComponent();

            ConversationHistory.ItemsSource = Messages;
            player.PlaybackStopped += Player_PlaybackStopped;
        }

        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            await InitSpeechConnectorAsync();

            UpdateStatus("New conversation started - type, press the microphone button, or say the wake word");
        }

        private async Task InitSpeechConnectorAsync()
        {
            audioConfig = AudioConfig.FromDefaultMicrophoneInput();
            var config = CustomCommandsConfig.FromSubscription(Constants.CustomCommandsAppId, Constants.SubscriptionKey, Constants.Region);
            config.Language = Constants.Language;

            // Create a new Dialog Service Connector for the above configuration and register to receive events
            connector = new DialogServiceConnector(config, audioConfig);
            connector.ActivityReceived += Connector_ActivityReceived;
            connector.Recognizing += Connector_Recognizing;
            connector.Recognized += Connector_Recognized;
            connector.Canceled += Connector_Canceled;
            connector.SessionStarted += Connector_SessionStarted;
            connector.SessionStopped += Connector_SessionStopped;

            // Open a connection to Direct Line Speech channel
            await connector.ConnectAsync();

            // Start the Keyword Recognizer.
            keywordRecognitionModel = KeywordRecognitionModel.FromFile(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Computer.table"));
            keywordRecognizer = new KeywordRecognizer(audioConfig);
            keywordRecognizer.Recognized += KeywordRecognizer_Recognized;
            StartKeywordRecognition();
        }

        private void KeywordRecognizer_Recognized(object sender, KeywordRecognitionEventArgs e)
        {
            Debug.WriteLine($"{nameof(KeywordRecognizer_Recognized)} ({e.Result.Reason}): {e.Result.Text}");

            if (e.Result.Reason == ResultReason.RecognizedKeyword)
            {
                // Keyword has been recognized, starts the actual speech recognition.
                StartListening();
            }
        }

        private void Connector_SessionStarted(object sender, SessionEventArgs e)
        {
            Debug.WriteLine($"SessionStarted event, id = {e.SessionId}");

            UpdateStatus("Listening ...");
            player.Stop();

            listeningState = ListenState.Listening;
        }

        private void Connector_SessionStopped(object sender, SessionEventArgs e)
        {
            Debug.WriteLine($"SessionStopped event, id = {e.SessionId}");

            UpdateStatus("Stopped listening");
            listeningState = ListenState.NotListening;
        }

        private void Connector_Canceled(object sender, SpeechRecognitionCanceledEventArgs e)
        {
            if (e.Reason == CancellationReason.Error
                && e.ErrorCode == CancellationErrorCode.ConnectionFailure
                && e.ErrorDetails.Contains("1000"))
            {
                // Connection was closed by the remote host.
                // Error code: 1000.
                // Error details: Exceeded maximum websocket connection idle duration (>300000ms = 5 minutes).
                // A graceful timeout after a connection is idle manifests as an error but isn't an
                // exceptional condition -- we don't want it show up as a big red bubble!
                UpdateStatus("Active connection timed out but ready to reconnect on demand.");
            }
            else
            {
                var statusMessage = $"Error ({e.ErrorCode}): {e.ErrorDetails}";
                UpdateStatus(statusMessage);
                listeningState = ListenState.NotListening;

                AddMessage(new MessageDisplay(statusMessage, Sender.Channel));
            }
        }

        private void Connector_Recognized(object sender, SpeechRecognitionEventArgs e)
        {
            Debug.WriteLine($"{nameof(Connector_Recognized)} ({e.Result.Reason}): {e.Result.Text}");

            UpdateStatus(string.Empty);

            if (!string.IsNullOrWhiteSpace(e.Result.Text) && e.Result.Reason == ResultReason.RecognizedSpeech)
            {
                AddMessage(new MessageDisplay(e.Result.Text, Sender.User));
            }
        }

        private void Connector_Recognizing(object sender, SpeechRecognitionEventArgs e)
        {
            UpdateStatus(e.Result.Text, tentative: true);
        }

        private async void Connector_ActivityReceived(object sender, ActivityReceivedEventArgs e)
        {
            var json = e.Activity;
            var activity = JsonConvert.DeserializeObject<Activity>(json);

            if (e.HasAudio && activity.Speak != null)
            {
                var audio = e.Audio;
                using var stream = new MemoryStream();

                await Task.Run(() =>
                {
                    var buffer = new byte[800];
                    uint bytesRead = 0;
                    while ((bytesRead = audio.Read(buffer)) > 0)
                    {
                        stream.Write(buffer, 0, (int)bytesRead);
                    }
                });

                var byteStream = stream.ToArray();
                var wavStream = new RawSourceWaveStream(byteStream, 0, byteStream.Length, new WaveFormat(16000, 16, 1));
                playbackStreams.Enqueue(wavStream);

                if (player.PlaybackState != PlaybackState.Playing)
                {
                    _ = Task.Run(() => PlayFromAudioQueue());
                }
            }

            // Checks whether it is a custom activity sent to client.
            if (activity.Type == "event" && activity.Value != null)
            {
                var message = $"{activity.Name}{Environment.NewLine}{activity.Value}";
                AddMessage(new MessageDisplay(message, Sender.Bot));
            }
            else if (activity.Type == ActivityTypes.Message)
            {
                AddMessage(new MessageDisplay(activity.Text, Sender.Bot));

                if (activity.InputHint == InputHints.ExpectingInput)
                {
                    // The activity expects a further user input.
                    waitingForUserInput = true;
                }
                else
                {
                    // No further user input is expected, restart the keyword recognizer.
                    StartKeywordRecognition();
                }
            }
        }

        private async void StatusBox_KeyUp(object sender, KeyEventArgs e)
        {
            StopKeywordRecognition();
            StopPlayback();
            waitingForUserInput = false;

            if (e.Key == Key.Enter)
            {
                e.Handled = true;

                var activity = Activity.CreateMessageActivity();
                activity.Text = statusBox.Text;

                statusBox.Clear();
                var jsonConnectorActivity = JsonConvert.SerializeObject(activity);

                AddMessage(new MessageDisplay(activity.Text, Sender.User));
                var id = await connector.SendActivityAsync(jsonConnectorActivity);

                Debug.WriteLine($"SendActivityAsync called, id = {id}");
            }
        }

        private bool PlayFromAudioQueue()
        {
            RawSourceWaveStream entry = null;
            lock (playbackStreams)
            {
                if (playbackStreams.Any())
                {
                    entry = playbackStreams.Peek();
                }
            }

            if (entry != null)
            {
                Debug.WriteLine($"Start playing...");

                player.Init(entry);
                player.Play();

                return true;
            }

            return false;
        }

        private void StartKeywordRecognition()
            => _ = keywordRecognizer.RecognizeOnceAsync(keywordRecognitionModel);

        private void StopKeywordRecognition()
            => _ = keywordRecognizer.StopRecognitionAsync();

        private void StartListening()
        {
            if (listeningState == ListenState.NotListening)
            {
                try
                {
                    listeningState = ListenState.Initiated;

                    _ = connector.ListenOnceAsync();

                    Debug.WriteLine("Started ListenOnceAsync");
                }
                catch (Exception ex)
                {
                    UpdateStatus(ex.Message);
                }
            }
        }

        private void Microphone_Click(object sender, RoutedEventArgs e)
        {
            StopKeywordRecognition();
            StopPlayback();

            if (listeningState == ListenState.NotListening)
            {
                StartListening();
            }
        }

        private void Player_PlaybackStopped(object sender, StoppedEventArgs e)
        {
            Debug.WriteLine($"Stopped playing");

            lock (playbackStreams)
            {
                // Checks if there is another audio message to play.
                if (!playbackStreams.Any())
                {
                    return;
                }

                var entry = playbackStreams.Dequeue();
                entry.Dispose();
            }

            if (!PlayFromAudioQueue())
            {
                // After ending all the playback, checks if the service is waiting for user input.
                // In this case, it automatically starts listening again.
                if (waitingForUserInput)
                {
                    waitingForUserInput = false;
                    StartListening();
                }
            }
        }

        private void StopPlayback()
        {
            lock (playbackStreams)
            {
                playbackStreams.Clear();
            }

            if (player.PlaybackState == PlaybackState.Playing)
            {
                player.Stop();
            }
        }

        private void RunOnUiThread(Action action)
        {
            Dispatcher.InvokeAsync(action);
        }

        private void UpdateStatus(string message, bool tentative = true)
        {
            if (Thread.CurrentThread != Dispatcher.Thread)
            {
                RunOnUiThread(() =>
                {
                    UpdateStatus(message, tentative);
                });
            }
            else
            {
                const string pad = "   ";

                if (tentative)
                {
                    statusOverlay.Text = $"{pad}{message}";
                }
                else
                {
                    statusBox.Clear();
                    statusBox.Text = $"{pad}{message}";
                }
            }
        }

        private void AddMessage(MessageDisplay message)
        {
            if (Thread.CurrentThread != Dispatcher.Thread)
            {
                RunOnUiThread(() =>
                {
                    AddMessage(message);
                });
            }
            else
            {
                Messages.Add(message);
                ConversationHistory.ScrollIntoView(ConversationHistory.Items[^1]);
            }
        }

        protected override async void OnClosed(EventArgs e)
        {
            await keywordRecognizer.StopRecognitionAsync();
            keywordRecognizer.Dispose();

            connector.Dispose();
            player.Dispose();
            audioConfig.Dispose();

            base.OnClosed(e);
        }
    }
}
