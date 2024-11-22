using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Speech.Synthesis;
using System.Speech.Recognition;
using System.IO;


namespace SpeechRecognition
{
    public partial class MainForm : Form
    {
        private Weather _weather;

        SpeechRecognitionEngine _recognizer = new SpeechRecognitionEngine();
        SpeechSynthesizer Sarah = new SpeechSynthesizer();
        SpeechRecognitionEngine startlistening = new SpeechRecognitionEngine();
        Random rnd = new Random();
        int RecTimeOut = 0;
        DateTime TimeNow = DateTime.Now;

        public MainForm()
        {
            InitializeComponent();
            _weather = new Weather("bd5e378503939ddaee76f12ad7a97608");
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            _recognizer.SetInputToDefaultAudioDevice();
            _recognizer.LoadGrammarAsync(new Grammar(new GrammarBuilder(new Choices(File.ReadAllLines(@"DefaultCommands.txt")))));
            _recognizer.SpeechRecognized += new EventHandler<SpeechRecognizedEventArgs>(Default_SpeechRecognized);
            _recognizer.SpeechDetected += new EventHandler<SpeechDetectedEventArgs>(_recognizer_SpeechRecognized);
            _recognizer.RecognizeAsync(RecognizeMode.Multiple);

            startlistening.SetInputToDefaultAudioDevice();
            startlistening.LoadGrammarAsync(new Grammar(new GrammarBuilder(new Choices(File.ReadAllLines(@"DefaultCommands.txt")))));
            startlistening.SpeechRecognized += new EventHandler<SpeechRecognizedEventArgs>(startlistening_SpeechRecognized);


        }

        private void Default_SpeechRecognized(object sender, SpeechRecognizedEventArgs e)
        {
            int ranNum;
            string speech = e.Result.Text;

            if(speech == "Hello")
            {
                Sarah.SpeakAsync("Hello, I am here");
            }
            if(speech =="How are you")
            {
                Sarah.SpeakAsync("I am working normally");
            }
            if(speech =="What time is it")
            {
                Sarah.SpeakAsync(DateTime.Now.ToString("h mm tt"));
            }
            if (speech == "What is today's date")
            {
               
                Sarah.SpeakAsync("Today's date is " + DateTime.Now.ToString("MMMM d, yyyy"));
            }
            if (speech =="Stop talking")
            {
                Sarah.SpeakAsyncCancelAll();
                ranNum = rnd.Next(1, 2);
                if (ranNum == 1)
                {
                    Sarah.SpeakAsync("Yes sir");
                }
                if (ranNum == 2)
                {
                    Sarah.SpeakAsync("I am sorry I will be quiet");
                }
            }
            if(speech=="Stop Listening")
            {
                Sarah.SpeakAsync("If you need me just ask");
                _recognizer.RecognizeAsyncCancel();
                startlistening.RecognizeAsync(RecognizeMode.Multiple);
            }
            if(speech=="Show Commands")
            {
                string[] commands = (File.ReadAllLines(@"DefaultCommands.txt"));
                lstCommands.Items.Clear();
                lstCommands.SelectionMode = SelectionMode.None;
                lstCommands.Visible = true;
                foreach (string command in commands)
                {
                    lstCommands.Items.Add(command);
                }
            }
            if(speech=="Hide Commands")
            {
                lstCommands.Visible = false;
            }
            if (speech == "What is the weather")
            {
                GetWeatherAndSpeak();  // Run async helper without needing a city parameter
            }
            if (speech == "Thanks")
            {
                Sarah.SpeakAsync("You are welcome. Is there anything else I can do for you?");
            }
            if (speech == "No")
            {
                Sarah.SpeakAsyncCancelAll();
            }

        }

        private void GetWeatherAndSpeak()
        {
            Task.Run(async () =>
            {
                string weatherInfo = await _weather.GetWeatherAsync("Philadelphia");
                Console.WriteLine(weatherInfo);
                Sarah.SpeakAsync(weatherInfo);
            });
        }
        private void _recognizer_SpeechRecognized(object sender, SpeechDetectedEventArgs e)
        {
            RecTimeOut = 0;
        }

        private void startlistening_SpeechRecognized(object sender, SpeechRecognizedEventArgs e)
        {
            string speech = e.Result.Text;

            if(speech=="Wake up")
            {
                startlistening.RecognizeAsyncCancel();
                Sarah.SpeakAsync("Yes, I am here");
                _recognizer.RecognizeAsync(RecognizeMode.Multiple);
            }
        }

        private void TmrSpeaking_Tick(object sender, EventArgs e)
        {
            if (RecTimeOut == 10)
            {
                _recognizer.RecognizeAsyncCancel();
            }
            else if (RecTimeOut == 11) {
                TmrSpeaking.Stop();
                startlistening.RecognizeAsync(RecognizeMode.Multiple);
                RecTimeOut = 0;
            }
        }

      
    }
}
