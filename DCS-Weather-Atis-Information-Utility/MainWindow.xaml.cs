using Ionic.Zip;
using LsonLib;
using Microsoft.Win32;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Speech.Synthesis;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Path = System.IO.Path;


/* Welcome to DCS-WAIFU (WPF Version)
 * This project will mirror DCS-WeatherMan-ConsoleVersion.
 * The differences will include:
 * -A GUI
 * -User saved locations via .json files
 * -Maybe voice (maybe, remember, thats like 751 different samples)
 * -Maybe make checkbox options for stuff that the user wants to hear or does not want to hear
 * 
 * Story Board:
 * Player launches dcs
 * player launches DCS-WAIFU (or it launches on its own, idk yet. Maybe roll it into DCS editor???
 * player selects their dcs saved games folder (or MP tracks folder?)
 * player joins multiplayer server
 * player presses a brief button
 * the program reads the lastest mp track mission file
 * the program gives the player a weather brief
 * DCS-WAIFU can be used multiple times during the session to get the weather
 * DCS-WAIFU does (does not?) close when dcs closes
 * 
 * 
 * DONE:
 * port over dcs-weatherman-consoleVersion (Complete)
 * Have the ATIS text populate in the log (Complete)
 * Clean up the .json (Complete)
 * Make the new .json options (Complete)
 *  options location (Complete)
 *  continous atis option (Complete)
 *  read speed (complete)
 *  volume (complete)
 * save data when the user presses any button (except maybe "stop atis") (complete)
 * dont let the player execute a brief or atis without having a saved games locaton in the program (complete)
 * consider making this an auto-detect app (not possible due to the order DCS writes to the SP and MP locations)
 * SP weather option (complete)
 * custom kneeboard background (complete)
 * remove all hard links (complete)
 * put in the runway directions for pg, nevada, the channel, normandy, syria (complete)
 * winds should be rounded to the nearest 10 degrees https://mediawiki.ivao.aero/index.php?title=METAR_explanation (complete)
 * winds 3kts or greater are reported. less than 3kts is calm (complete)
 * Airport Selection dropdown? will provide rwy and approaches in use. (complete)
 * If you get the WX before the jet is spawned, the info is written on the custom kneeboard (wow) (complete)
 * use this to redo the reported visability //http://www.moratech.com/aviation/metar-class/metar-pg7-vis.html (complete)
 * combine temp and dew on one line on kneeboard (complete)
 * combine qnh and altimeter on one line on kneeboard (complete)
 * replace "at" with "@" on kneeboard (complete)
 * implement fog and dust remarks (complete)
 * If Clear below contains "000", replace the zeros with "k" (complete)
 * name the program (complete)
 * make a program icon (complete)
 * when temp is below 0, the guy says "minus" (complete)
 * do bad weather testing (complete)
 * When you press "STOP ATIS" button the core process usage goes up to 44% and
 *  it goes back to near zero after voice talking resumes (fixed. the problem was a silent looping 
 *  command with the repeat atis option))
 * make log mode (complete)
 * reconsider program name: Weather Atis InFormation Utility DCS-Waifu (complete) https://www.codeproject.com/articles/697108/visual-studio-painlessly-renaming-your-project-and
 * kneboard background will autoigenerate if not preset (complete)
 * write readme (complete)
 * he says "clouds at thirty three hundred feet" instead of "coulds at three thousand three hundred feet" (fixed)
 * create kneeboard folder directory if the user does not have one (complete)
 * add in manpads alerts (requested by Jak) (complete)
 * make the app runable on other devices (complete) thanks Fody https://www.youtube.com/watch?v=pDGjfQCcPuU
 * clean assets folder (complete)
 * make icon for ED Userfiles (complete)
 * make a "file exists" check for MP (complete)
 * automatically detect players options.lua (complete)
 * make a log mode in the ini (complete)
 * test on other devices (complete)
 * make  a nice background for the program (complete)
 * clean up the getLuaValues logic to merge them for SP and MP (complete)
 * make a "hide mode" or "click to view" or "hover to view" to the user path display (complete)
 * have ppl demo it (complete)
 * release on github (complete)
 * release on ED UserFiles (complete)
 * Release forum post (complete)
 * waifu wont run in waifu is already running (complete)
 * waifu closes based on special menu options (complete)
 * make the dcs/mods/options icons for the module bar (complete)
 * Launch With DCS (requested by Hornel/Lion) (complete)
 * clean up the kneeboard after close (complete)
 * 
 * 
 * TODO 2:
 * check to make sure that the app used the lua method for reading the file vice the parse method
 * fix the errors that were created by DCS v2.7
 * streamline the addition of new airports (likely with easy instructions
 * have the airports in a dictionary with the runways as the value and the airport as the key
 * see if you can get the information straight from the dcs install. maybe
 * have the dewpoint calculate up to the higest cloud layer
 * in fog, temp == dewpoint (?)
 * test waifu mode on a different setup
 * make a demo video (keep it short!) and explain the features (with waifu voice)
 * 
 * 
 * TODO 9 eventually
 * maybe play sound https://stackoverflow.com/questions/29552373/playing-sounds-in-wpf-as-a-resource
 * find out how atis calls out snow
 * find out the dewpoint of snowing weather
 * merge the "generate file locations" so that theree arent random one in the code
 * make list of .wav files to be recorded
 * see if you can Async the human brief somehow. maybe join the file and then play it.
 * try to get a better editing tool for the hundereds of voiceover clips, idk
 * look into having a custom font
 *  this may be able to achieved in 2 ways
 *      1. make all of the combos in photoshop and cut them out and then call them for each of the ocassions.
 *      2. make my own custom .tga and .txt to format the font and then use it.
 * 
 * 
 * Known Issues:
 * None (yay!)
 * 
 * 
 * Notes:
 * -Due to the fact that DCS makes the files in the DCS Temp location, saves them to the MP location,
 * and then re-edits the files in the Temp Location, I cannot tell if the player wants SP or MP atis
 * with only one button.
 * -cloud image https://www.clipartmax.com/middle/m2i8Z5K9K9K9Z5b1_free-vector-clouds-png-cartoon-pictures-of-clouds/
 * -atis examples https://www.youtube.com/playlist?list=PL2FA94DECFBA07712
 * 
 * 
 * Wishlist:
 * -Imperial and metric version
 * -Japanese synth voice
 * -Solve that one persons folder structure problem by allowing them to select their folder if the 
 *      predetermined one does not exist.
 * 
 * 
 * Goals:
 * Fun
 * 
 * 
 * Folders of Interesst: 
 * C:\Users\...\Saved Games\DCS.openbeta\Tracks\Multiplayer\MPmission.trk
 * C:\Users\...\AppData\Local\Temp\DCS.openbeta\tempMission.miz
 * C:\Users\...\AppData\Local\Temp\DCS.openbeta\Mission\mission
 * 
 * G:\Games\DCS World OpenBeta\Scripts\Aircrafts\_Common\Cockpit\KNEEBOARD
 * G:\Games\DCS World OpenBeta\Scripts\Aircrafts\_Common\Cockpit\KNEEBOARD\indicator\CUSTOM\test_user_chart.lua
 * 
 * Files of Interest:
 * G:\Games\DCS World OpenBeta\Mods\terrains\Caucasus\Radio.lua
 * G:\Games\DCS World OpenBeta\Mods\terrains\Caucasus\AirfieldsCfgs\ (multiple)
 * 
 * 
 * Version Notes:
 * v1
 * -Initial Release
 * v2
 * -Added options for WAIFU to launch and close with DCS
 * v3
 * -Cleans up the dynamic kneeboard after WAIFU closes
 * v4
 *-Added Syria Airports
 *-Known Bug: DCS v2.7 causes inaccurate reports in some cases
 */

namespace DCS_Weather_Atis_Information_Utility
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    /// 
    public static class ExtensionMethods
    {//this is the logic for the rounding stuff
        public static int RoundOff(this int i)
        {
            return ((int)Math.Round(i / 10.0)) * 10;
        }
    }

    public partial class MainWindow : Window
    {
        
        
        //-----Global Variables-----///
        FileSystemWatcher watcher = new FileSystemWatcher();//this...is actually not necessary unless i let the user use the option. likely not....

        SpeechSynthesizer synth = new SpeechSynthesizer();//the thing that allows the computer to talk. maybe have voice options

        

        string userSelectedFilepath;
        string userSelectedFilepathParent;//used for the filewatcher
        string dcsSavedGamesFolder;
        string spTrackFolder;//this is the folder located in the users AppData/temp
        string mpTrackFolder;
        
        string dcsVersionFolderName;//this should be "DCS", or "DCS.openbeta", or "DCS.openalpha"
        string missionFileToDelete;
        string mizName;
        string programLocation = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        string DcsWaifuSettingsLocation;

        string dcsVersionUserDriveName;
        string dcsVersionUserFolder;
        string dcsVersionUserName;

        bool isContinuousAtisChecked;
        bool playBrief;
        bool isLogModeEnabled;//this is a "cheat" for log mode. put on "false for distro"

        string appPath = System.AppDomain.CurrentDomain.BaseDirectory;//path of this program exe
        string kneeboardImageFile = Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, "customKneeboardPicture.png");//path of theimage should be placed here
        
        string jsonSaveFileFull;

        DateTime programStartTime = DateTime.Now;//internally logs the start of the program to be used for later

        FileInfo newestFileName;

        FileSystemWatcher fileSystemWatcher1;

        //i dont think these three things are actually used for WAIFU
        string dcs_topFolderPath;
        bool isDcsLocationSet;
        bool isDCSrunning;
        bool isWaifuModeOn;

        int secondsToCheckIfDcsIsAlive = 1;//DiCE will check if DCS.exe is running at this rate. 2 was fine

        //this inits the timer. Putting here allows it to be used in the whole program
        System.Windows.Threading.DispatcherTimer dispatcherTimer = new System.Windows.Threading.DispatcherTimer();


        //-----Global Variables-----///
       
        public MainWindow()
        {
        
            //-----------------------------------------------------------------------------------------
            //-----Is DCS WAIFU Already Running Check
            //-----------------------------------------------------------------------------------------

            //simple check to make sure that an instance of dice is not launched while an instance is already running
            //https://stackoverflow.com/questions/7182949/how-to-check-if-a-wpf-application-is-already-running
            string procName = Process.GetCurrentProcess().ProcessName;

            // get the list of all processes by the "procName"       
            Process[] processes = Process.GetProcessesByName(procName);

            if (processes.Length > 1)
            {
                if (isLogModeEnabled)
                    MessageBox.Show(procName + " already running.");//i dont want a message, just block the program
                System.Windows.Application.Current.Shutdown();
                return;
            }
            else
            {
                // Application.Run(...);
            }

            InitializeComponent();

            //-------
            //Waifu Check
            //------
            int random_number_waifuCheck = new Random().Next(1, 50);//chose a random number between 1 and 999
            //MessageBox.Show(random_number_waifuCheck.ToString());
            if (random_number_waifuCheck == 23) //if the number turns out to be 23 https://stackoverflow.com/questions/10079912/c-sharp-probability-and-random-numbers
            {
                isWaifuModeOn = true;
            }

            //-----------------------------------------------------------------------------------------
            //------------Timer Init
            //-----------------------------------------------------------------------------------------

            //https://stackoverflow.com/questions/5410430/wpf-timer-like-c-sharp-timer

            dispatcherTimer.Tick += dispatcherTimer_Tick;
            dispatcherTimer.Interval = new TimeSpan(0, 0, secondsToCheckIfDcsIsAlive);//set the time for the DCS process check here (seconds, minutes, hours)

            //-----------------------------------------------------------------------------------------
            //--------Get The DCS.exe Location
            //-----------------------------------------------------------------------------------------

            //https://stackoverflow.com/questions/51148/how-do-i-find-out-if-a-process-is-already-running-using-c
            //https://stackoverflow.com/questions/5497064/how-to-get-the-full-path-of-running-process
            //This gets the full path of DCS when DCS.exe is running as long as the process is called 'DCS'
            //this actually works!

            foreach (Process clsProcess in Process.GetProcesses())
            {
                //now we're going to see if any of the running processes
                //match the currently running processes. Be sure to not
                //add the .exe to the name you provide, i.e: NOTEPAD,
                //not NOTEPAD.EXE or false is always returned even if
                //notepad is running.
                //Remember, if you have the process running more than once, 
                //say IE open 4 times the loop thr way it is now will close all 4,
                //if you want it to just close the first one it finds
                //then add a return; after the Kill
                //for some reason you need this https://stackoverflow.com/questions/34070969/i-get-a-32-bit-processes-cannot-access-modules-of-a-64-bit-process-exception
                //targeting 64 bit fixed it
                if (clsProcess.ProcessName.Equals("DCS"))
                {
                    //if the process is found to be running then we
                    //return a true
                    var process = Process.GetCurrentProcess(); // Or whatever method you are using (I dont think this does anything as is)
                    string userDcs_Full_pathWithExtention = clsProcess.MainModule.FileName;
                    //MessageBox.Show(userDcs_Full_pathWithExtention);//shows the path od dcss to include the exe when dcs is running
                    if (isLogModeEnabled)
                        richTextBox_log.AppendText(Environment.NewLine + DateTime.Now + ": " + "DCS.exe path: '" + userDcs_Full_pathWithExtention + "'");
                    richTextBox_log.ScrollToEnd();
                    dcs_topFolderPath = userDcs_Full_pathWithExtention.Remove(userDcs_Full_pathWithExtention.Length - 12);//remove the bin and exe so that you get to the top folder
                    isDcsLocationSet = true;
                    isDCSrunning = true;//using this instead of the else ofr reasons explained below
                    dispatcherTimer.Start();//starts the timer
                }
                else
                {
                    //richTextBox_log.AppendText(Environment.NewLine + DateTime.Now + ": " + "DCS is not detected. Close DiCE, make sure you have installed DiCE correctly, and then re-start DCS.");
                    //for some reason this repeats, a lot, on launch.OOOOOOooooh, thats because its in the 'foreach', so it sends the message for every process that isn't DCS.
                }
            }

            //https://stackoverflow.com/questions/14899422/how-to-navigate-a-few-folders-up



            //i think the code starts here. Sweet
            //get rid of the extra space in the rich text box after each entry
            //https://stackoverflow.com/questions/325075/how-do-i-change-richtextbox-paragraph-spacing

            synth.SpeakCompleted += syth_SpeakCompleted;//flags when speach is complete
                                                        //synth.SelectVoiceByHints(VoiceGender.Female, VoiceAge.Teen);//sets the character of the voice
                                                        //synth.SelectVoice("Microsoft Haruka Desktop");


            VoiceInfo info;

            foreach (InstalledVoice voice in synth.GetInstalledVoices())
            {
                info = voice.VoiceInfo;
                //MessageBox.Show(" Voice Name: " + info.Name);
                synth.SelectVoice(info.Name);
                //synth.Speak("This is a demo.");
            }
            





            synth.SetOutputToDefaultAudioDevice();//says which speaker/device to use
            //https://docs.microsoft.com/en-us/dotnet/api/system.speech.synthesis.speechsynthesizer.volume?view=netframework-4.8
            //https://www.wpf-tutorial.com/misc-controls/the-slider-control/
            synth.Rate = 0;//default. -10 to 10
            synthSpeed_slider.Value = 0;//the actual slider goes from 0 to 20 because it was necessary for the slider coloring
            synth.Volume = 100;//default is 100. Goes from 0 to 100. 
            synthVolume_slider.Value = 100;
            //the two lines below make sure that the black fill is set to the correct location on init
            synthSpeed_slider.SelectionEnd = synthSpeed_slider.Value;
            synthVolume_slider.SelectionEnd = synthVolume_slider.Value;
            //synth.SelectVoice("Microsoft Haruka Desktop");
            //isLogModeEnabled = false;



            //-----Check to see if Info file has already been created----//

            DcsWaifuSettingsLocation = Path.Combine(programLocation, "DCS-WAIFU Files");
            Directory.CreateDirectory(DcsWaifuSettingsLocation);
            jsonSaveFileFull = Path.Combine(DcsWaifuSettingsLocation, "DCS-WAIFU.ini");//you can cange the same of the save file here
            //MessageBox.Show(jsonSaveFileFull);
            if (File.Exists(jsonSaveFileFull))//if the file exists, load the data
            {
                using (StreamReader file = File.OpenText(jsonSaveFileFull))//this is the .json
                using (JsonTextReader reader = new JsonTextReader(file))
                {
                    JObject json_object = (JObject)JToken.ReadFrom(reader);//read the json and load the json info
                   

                    dynamic stuff = JsonConvert.DeserializeObject(json_object.ToString());

                    //user location
                    userSelectedFilepath = stuff.userSelectedFilepath;
                    //selectedFileLabel.Content = stuff.userSelectedFilepath;
                    userChosenPath_textBox.Text = stuff.userSelectedFilepath;

                    //checkbox
                    isContinuousAtisChecked = stuff.ContinuousAtisCheckedcheck;
                    if (isContinuousAtisChecked.Equals(true))
                    {
                        continuous_atis_checkbox.IsChecked = true;
                    }
                    else
                    {
                        continuous_atis_checkbox.IsChecked = false;
                    }

                    //synth options
                    synth.Rate = stuff.AtisReadSpeed;
                    synthSpeed_slider.Value = stuff.AtisReadSpeed;

                    synth.Volume = stuff.AtisVolume;
                    synthVolume_slider.Value = stuff.AtisVolume;
                    isLogModeEnabled = stuff.logMode;
                    //MessageBox.Show(userSelectedFilepath);

                    if (isLogModeEnabled)//supper shorthand for "if true do the next line"
                        richTextBox_log.AppendText(Environment.NewLine + DateTime.Now + ": " + "Backupsettings found in - " + jsonSaveFileFull);


                    if (isLogModeEnabled)//supper shorthand for "if true do the next line"
                        richTextBox_log.AppendText(Environment.NewLine + DateTime.Now + ": " + "Test Result - " + userSelectedFilepath);
                    richTextBox_log.ScrollToEnd();

                    generateFilePaths();

                    if (isLogModeEnabled)//supper shorthand for "if true do the next line"
                        richTextBox_log.AppendText(Environment.NewLine + DateTime.Now + ": " + "Your MP Tracks Folder is - " + mpTrackFolder);
                    if (isLogModeEnabled)//supper shorthand for "if true do the next line"
                        richTextBox_log.AppendText(Environment.NewLine + DateTime.Now + ": " + "Your SP Tracks Folder is - " + spTrackFolder);
                    richTextBox_log.ScrollToEnd();
                }
            }
            else//no config was found. try to guess the users path
            {//this is for the %USERPROFILE% https://stackoverflow.com/questions/49634412/why-cant-i-set-userprofile-for-downloads-in-my-c-sharp-project
                if (File.Exists(Environment.ExpandEnvironmentVariables(@"%USERPROFILE%\Saved Games\DCS.openbeta\Config\options.lua")))
                {
                    userSelectedFilepath = Environment.ExpandEnvironmentVariables(@"%USERPROFILE%\Saved Games\DCS.openbeta\Config\options.lua");
                    userChosenPath_textBox.Text = userSelectedFilepath;
                    richTextBox_log.AppendText(Environment.NewLine + DateTime.Now + ": " + "WAIFU automatically found an Options.lua. You can select a different one if you like.");
                    generateFilePaths();

                    if (isLogModeEnabled)//supper shorthand for "if true do the next line"
                        richTextBox_log.AppendText(Environment.NewLine + DateTime.Now + ": " + "Your MP Tracks Folder is - " + mpTrackFolder);
                    if (isLogModeEnabled)//supper shorthand for "if true do the next line"
                        richTextBox_log.AppendText(Environment.NewLine + DateTime.Now + ": " + "Your SP Tracks Folder is - " + spTrackFolder);
                    richTextBox_log.ScrollToEnd();
                    saveSettings();
                }
                else if (File.Exists(Environment.ExpandEnvironmentVariables(@"%USERPROFILE%\Saved Games\DCS\Config\options.lua")))
                {
                    userSelectedFilepath = Environment.ExpandEnvironmentVariables(@"%USERPROFILE%\Saved Games\DCS\Config\options.lua");
                    userChosenPath_textBox.Text = userSelectedFilepath;
                    richTextBox_log.AppendText(Environment.NewLine + DateTime.Now + ": " + "WAIFU automatically found an Options.lua. You can select a different one if you like.");
                    generateFilePaths();

                    if (isLogModeEnabled)//supper shorthand for "if true do the next line"
                        richTextBox_log.AppendText(Environment.NewLine + DateTime.Now + ": " + "Your MP Tracks Folder is - " + mpTrackFolder);
                    if (isLogModeEnabled)//supper shorthand for "if true do the next line"
                        richTextBox_log.AppendText(Environment.NewLine + DateTime.Now + ": " + "Your SP Tracks Folder is - " + spTrackFolder);
                    richTextBox_log.ScrollToEnd();
                    saveSettings();
                }
                else if (File.Exists(Environment.ExpandEnvironmentVariables(@"%USERPROFILE%\Saved Games\DCS.openalpha\Config\options.lua")))
                {
                    userSelectedFilepath = Environment.ExpandEnvironmentVariables(@"%USERPROFILE%\Saved Games\DCS.openalpha\Config\options.lua");
                    userChosenPath_textBox.Text = userSelectedFilepath;
                    richTextBox_log.AppendText(Environment.NewLine + DateTime.Now + ": " + "WAIFU automatically found an Options.lua. You can select a different one if you like.");
                    generateFilePaths();

                    if (isLogModeEnabled)//supper shorthand for "if true do the next line"
                        richTextBox_log.AppendText(Environment.NewLine + DateTime.Now + ": " + "Your MP Tracks Folder is - " + mpTrackFolder);
                    if (isLogModeEnabled)//supper shorthand for "if true do the next line"
                        richTextBox_log.AppendText(Environment.NewLine + DateTime.Now + ": " + "Your SP Tracks Folder is - " + spTrackFolder);
                    richTextBox_log.ScrollToEnd();
                    saveSettings();
                }
                else
                {
                    //couldnt predict it. oh well.
                    richTextBox_log.AppendText(Environment.NewLine + DateTime.Now + ": " + "WAIFU could not automatically find an Options.lua. :(");
                }
                richTextBox_log.ScrollToEnd();
            }

            //-------------------------------------------------------------------------------
            //-----Read options lua to see if the program should automatically close
            //-----------------------------------------------------------------------------------


            //this has to be put here because i think the delay is messing with the resulkts of the inquiry when "invoked"
            var optionsLuaText = LsonVars.Parse(File.ReadAllText(userSelectedFilepath));//put the contents of the options.lua into a lua read
            closeWaifuAfterDcsClose = optionsLuaText["options"]["plugins"]["DCS WAIFU"]["closeWaifuAfterDcsClose"].GetBool();//this will get either true of false
            closeWaifuAfterDcsLaunch = optionsLuaText["options"]["plugins"]["DCS WAIFU"]["closeWaifuAfterDcsLaunch"].GetBool();//this will get either true or false

            if (closeWaifuAfterDcsLaunch == true)
            {
                //MessageBox.Show("Closing WAIFU because the value is" + closeWaifuAfterDcsLaunch.ToString());
                CloseDcsWaifu();
            }
            else
            {
                //dont close dcsWaifu
                //MessageBox.Show("Not Closing WAIFU because the value is " + closeWaifuAfterDcsLaunch.ToString());
            }



            //MessageBox.Show(Environment.ExpandEnvironmentVariables(@"%USERPROFILE%\Saved Games\DCS.openbeta\Config\options.lua"));
            savedGamesSelectionRectangeCheck();
            if (isLogModeEnabled)//supper shorthand for "if true do the next line"
                richTextBox_log.AppendText(Environment.NewLine + DateTime.Now + ": " + "This program has been launched from - " + programLocation);
            richTextBox_log.ScrollToEnd();
            listenToUsersOptionsFile();
            if (isLogModeEnabled)//supper shorthand for "if true do the next line"
                richTextBox_log.AppendText(Environment.NewLine + DateTime.Now + ": " + 
                    "Listening to options.lua");
            richTextBox_log.ScrollToEnd();
        }

        private void listenToUsersOptionsFile()
        {//this makes the timer
            //https://www.c-sharpcorner.com/UploadFile/ad8d1c/watch-a-folder-for-updation-in-wpf-C-Sharp/
            fileSystemWatcher1 = new FileSystemWatcher(userSelectedFilepathParent, "options.lua");//userSelectedFilepath is the options lua
            //richTextBox_log.AppendText(Environment.NewLine + DateTime.Now + ": " + "Watching for changes in: " + optionsLua_topFolderPath + "\\options.lua");
            //richTextBox_log.ScrollToEnd();

            fileSystemWatcher1.EnableRaisingEvents = true;
            fileSystemWatcher1.IncludeSubdirectories = true;
            //This event will check for  new files added to the watching folder
            //fileSystemWatcher1.Created += new FileSystemEventHandler(newfile);
            //This event will check for any changes in the existing files in the watching folder
            fileSystemWatcher1.Changed += new FileSystemEventHandler(fs_Changed);
            //this event will check for any rename of file in the watching folder
            //fileSystemWatcher1.Renamed += new RenamedEventHandler(fs_Renamed);
            //this event will check for any deletion of file in the watching folder
            //fileSystemWatcher1.Deleted += new FileSystemEventHandler(fs_Deleted);

            //var pathWithEnv = @"%USERPROFILE%\Saved Games"; //this does not work because options.lua is too deep
            //var filePath = Environment.ExpandEnvironmentVariables(pathWithEnv);
            //fileSystemWatcher1.Path = (filePath);
            fileSystemWatcher1.Path = Directory.GetParent(userSelectedFilepath).ToString();
            fileSystemWatcher1.Filter = "options.lua";
            if (isLogModeEnabled)
                richTextBox_log.AppendText(Environment.NewLine + DateTime.Now + ": " +
                    "Listening to " + Path.Combine(Directory.GetParent(userSelectedFilepath).ToString(), "options.lua"));
            //MessageBox.Show("listening via filesystemwatcher");
        }

        DateTime fsLastRaised = DateTime.Now;//this is going to be used for making sure that 
        //there isnt an overreaction to the options lua being changed

        bool closeWaifuAfterDcsClose;
        bool closeWaifuAfterDcsLaunch;

        public void fs_Changed(object fschanged, FileSystemEventArgs changeEvent)
        {
            ReadOptionsLuaFile();//the sender shouldnt matter because i have a fliter and i know what file i want
        }

        public void ReadOptionsLuaFile()
        {
            if (DateTime.Now.Subtract(fsLastRaised).TotalMilliseconds > 1000)//this hopefully prevents the options.lua to be read multiple times within 1 second
            {
                fsLastRaised = DateTime.Now;
                //MessageBox.Show("The file was chaged");//this works, trust me. Try to get it to watch, specifically, the options.lua file.
                //https://docs.microsoft.com/en-us/dotnet/csharp/programming-guide/file-system/how-to-read-from-a-text-file
                //these are the names of the identifiers in the options.lua file

                Thread.Sleep(500);//is hopefully prevents the read of the below file during a DCS write. has not failed at 1000, yet.
                                  //2000 kinda takes too long, personally
                                  //500 seems to be working fine with the multi-change check
                                  //https://stackoverflow.com/questions/9732709/the-calling-thread-cannot-access-this-object-because-a-different-thread-owns-it
                                  //where the invoke thing comes from

                this.Dispatcher.Invoke(() =>
                {
                    if (isLogModeEnabled)
                        richTextBox_log.AppendText(Environment.NewLine + DateTime.Now + ": " + "A change in Options.lua was detected.");
                    try
                    {
                        var optionsLuaText = LsonVars.Parse(File.ReadAllText(userSelectedFilepath));//put the contents of the options.lua into a lua read
                        closeWaifuAfterDcsClose = optionsLuaText["options"]["plugins"]["DCS WAIFU"]["closeWaifuAfterDcsClose"].GetBool();//this will get either true of false

                        if (closeWaifuAfterDcsClose == true)
                        {
                            if (isLogModeEnabled)
                                richTextBox_log.AppendText(Environment.NewLine + DateTime.Now + ": " +
                            "DCS WAIFU will close after DCS closes.");
                            richTextBox_log.ScrollToEnd();
                        }
                        

                        closeWaifuAfterDcsLaunch = optionsLuaText["options"]["plugins"]["DCS WAIFU"]["closeWaifuAfterDcsLaunch"].GetBool();//this will get either true or false

                        if (closeWaifuAfterDcsLaunch == true)
                        {
                            if (isLogModeEnabled)
                                richTextBox_log.AppendText(Environment.NewLine + DateTime.Now + ": " +
                            "DCS WAIFU will close after DCS launches.");
                            richTextBox_log.ScrollToEnd();
                        }
                    }
                    catch (IOException f)
                    {
                        richTextBox_log.AppendText(Environment.NewLine + DateTime.Now + ": " + "DCS-WAIFU could not read the options.lua");
                        richTextBox_log.AppendText(Environment.NewLine + DateTime.Now + ": " + f.Message);
                        richTextBox_log.ScrollToEnd();
                    }
                });
            }
        }


        public void CheckOptionsLusSettingsOnLaunch()
        {
            try
            {
                var optionsLuaText = LsonVars.Parse(File.ReadAllText(userSelectedFilepath));//put the contents of the options.lua into a lua read
                closeWaifuAfterDcsClose = optionsLuaText["options"]["plugins"]["DCS WAIFU"]["closeWaifuAfterDcsClose"].GetBool();//this will get either true of false
                richTextBox_log.AppendText(Environment.NewLine + DateTime.Now + ": " +
                    "DCS WAIFU will close after DCS closes.");
                richTextBox_log.ScrollToEnd();

                closeWaifuAfterDcsLaunch = optionsLuaText["options"]["plugins"]["DCS WAIFU"]["closeWaifuAfterDcsLaunch"].GetBool();//this will get either true or false
                richTextBox_log.AppendText(Environment.NewLine + DateTime.Now + ": " +
                    "DCS WAIFU will close after DCS launches.");
                richTextBox_log.ScrollToEnd();
            }
            catch (IOException f)
            {
                richTextBox_log.AppendText(Environment.NewLine + DateTime.Now + ": " + "DCS-WAIFU could not read the options.lua");
                richTextBox_log.AppendText(Environment.NewLine + DateTime.Now + ": " + f.Message);
                richTextBox_log.ScrollToEnd();
            }
        }

        private void SelectDCS_button_click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "options.lua|*.lua";

            //try to determine which directory the file may be in
            if (File.Exists(Environment.ExpandEnvironmentVariables(@"%USERPROFILE%\Saved Games\DCS.openbeta\Config\options.lua")))
            {
                openFileDialog.InitialDirectory = @"C:\%USERPROFILE%\Saved Games\DCS.openbeta\Config\";
            }
            else if (File.Exists(Environment.ExpandEnvironmentVariables(@"%USERPROFILE%\Saved Games\DCS\Config\options.lua")))
            {
                openFileDialog.InitialDirectory = @"C:\%USERPROFILE%\Saved Games\DCS\Config\";
            }
            else if (File.Exists(Environment.ExpandEnvironmentVariables(@"%USERPROFILE%\Saved Games\DCS.openalpha\Config\options.lua")))
            {
                openFileDialog.InitialDirectory = @"%USERPROFILE%\Saved Games\DCS.openalpha\Config\";
            }
            else
            {
                openFileDialog.InitialDirectory = @"%USERPROFILE%\Saved Games\DCS.openbeta\Config\";
            }

            openFileDialog.Title = "Select options.lua (Example: C:\\Users\\USERPROFILE\\Saved Games\\DCS\\Config\\options.lua)";
            if (openFileDialog.ShowDialog() == true)//if user chose a file
            {
                userSelectedFilepath = openFileDialog.FileName;
                //do something
                if (userSelectedFilepath.Contains(@"\options.lua"))
                {

                    richTextBox_log.AppendText(Environment.NewLine + DateTime.Now + ": " + "It looks like you have selected the correct file - " + userSelectedFilepath);
                    richTextBox_log.ScrollToEnd();
                    generateFilePaths();
                    //dcsSavedGamesFolder = System.IO.Path.GetFullPath(System.IO.Path.Combine(userSelectedFilepath, @"..\..\"));
                    if (isLogModeEnabled)//supper shorthand for "if true do the next line"
                        richTextBox_log.AppendText(Environment.NewLine + DateTime.Now + ": " + "Your DCS Saved Games folder is - " + dcsSavedGamesFolder);
                    richTextBox_log.ScrollToEnd();

                    //either way works
                    //string[] paths = { dcsSavedGamesFolder, "tracks", "Multiplayer"};
                    //mpTrackFolder = System.IO.Path.Combine(paths);
                    //mpTrackFolder = Path.Combine(dcsSavedGamesFolder, "tracks", "Multiplayer");
                    if (isLogModeEnabled)//supper shorthand for "if true do the next line"
                        richTextBox_log.AppendText(Environment.NewLine + DateTime.Now + ": " + "Your MP Tracks Folder is - " + mpTrackFolder);
                    if (isLogModeEnabled)//supper shorthand for "if true do the next line"
                        richTextBox_log.AppendText(Environment.NewLine + DateTime.Now + ": " + "Your SP Tracks Folder is - " + spTrackFolder);
                    richTextBox_log.ScrollToEnd();

           
                    //this is where you can have the program settings. yay
                    saveSettings();

                    userChosenPath_textBox.Text = userSelectedFilepath;


                }
                else
                {
                    richTextBox_log.AppendText(Environment.NewLine + DateTime.Now + ": " + "It looks like you have not selected the correct file - " + userSelectedFilepath);
                    richTextBox_log.ScrollToEnd();
                }
            }
            savedGamesSelectionRectangeCheck();
        }

        public void savedGamesSelectionRectangeCheck()
        {//https://stackoverflow.com/questions/2886537/how-do-you-color-a-rectangle-in-c-sharp-that-has-been-declared-in-xaml-in-wpf
            if (dcsSavedGamesFolder == null)//selection check
            {
                SavedGamesSelectionRectange.Fill = new SolidColorBrush(System.Windows.Media.Colors.Red);
            }
            else
            {
                SavedGamesSelectionRectange.Fill = new SolidColorBrush(System.Windows.Media.Colors.Green);
            }
        }

      

        string theatre;

        int generalVisibilityLua;
        int dustVisibilityLua;
        int fogVisibilityLua;
        string isFogEnabledLua;
        string isDustEnabledLua;
        int minimumVis;

        string cloudDensityNumberLua;
        Double cloudHeightLua;
        int precipValue;

        Double temperatureLua;
        Double qnhLua;

        public static FileInfo GetNewestFile(DirectoryInfo directory)
        {
            return directory.GetFiles()
                .Union(directory.GetDirectories().Select(d => GetNewestFile(d)))
                .OrderByDescending(f => (f == null ? DateTime.MinValue : f.LastWriteTime))
                .FirstOrDefault();
        }

        public void generateFilePaths()
        {
            //MessageBox.Show(userSelectedFilepath);
            userSelectedFilepathParent = Directory.GetParent(userSelectedFilepath).ToString();
            dcsSavedGamesFolder = System.IO.Path.GetFullPath(System.IO.Path.Combine(userSelectedFilepath, @"..\..\"));
            mpTrackFolder = Path.Combine(dcsSavedGamesFolder, "tracks", "Multiplayer");
            missionFileToDelete = Path.Combine(DcsWaifuSettingsLocation, "mission");//deletes the mission file if there was one. 
            
            //----------The following should take the details from the chosen options.lua path
            //and construct the path for the DCS Temp locaiton
            //https://stackoverflow.com/questions/401304/how-does-one-extract-each-folder-name-from-a-path
            //C:\Users\...\AppData\Local\Temp\DCS.openbeta\tempMission.miz
            string mypath = userSelectedFilepath;
            string[] directories = mypath.Split(Path.DirectorySeparatorChar);
       
            dcsVersionFolderName = (directories[directories.Length - 3]);

            //if (isLogModeEnabled)//supper shorthand for "if true do the next line"
            //    foreach (string folderName in directories){
            //    richTextBox_log.AppendText(Environment.NewLine + DateTime.Now + ": " + folderName);
            //}
            //richTextBox_log.ScrollToEnd();



            dcsVersionFolderName = (directories[directories.Length - 3]);//this gets what is hopfully the name of the dcs version assumuing
            //that the DCS folder structure in intact.
            dcsVersionUserDriveName = (directories[0]);
            dcsVersionUserFolder = (directories[1]);
            dcsVersionUserName = (directories[2]);
            spTrackFolder = Path.Combine(dcsVersionUserDriveName + "\\", dcsVersionUserFolder, dcsVersionUserName, "AppData",
                "Local", "Temp", dcsVersionFolderName);//idk why i had to add the double slashes even though there was a "combine"
            if (isLogModeEnabled)//supper shorthand for "if true do the next line"
                richTextBox_log.AppendText(Environment.NewLine + DateTime.Now + ": " + 
                "The DCS folder name version is " + dcsVersionFolderName);
            if (isLogModeEnabled)//supper shorthand for "if true do the next line"
                richTextBox_log.AppendText(Environment.NewLine + DateTime.Now + ": " +
                "The Temp DCS folder path is " + spTrackFolder);
            richTextBox_log.ScrollToEnd();

            //https://stackoverflow.com/questions/13175868/how-to-get-full-file-path-from-file-name
            //formats the string for lua
            kneeboardImageFile = Path.Combine(DcsWaifuSettingsLocation, "customKneeboardPicture.png");//path of theimage should be placed here
           
            FileInfo f = new FileInfo(kneeboardImageFile);
            kneeboardImageFullName = f.FullName;
            kneeboardImageFullName = kneeboardImageFullName.Replace("\\","\\\\");
            if (isLogModeEnabled)//supper shorthand for "if true do the next line"
                richTextBox_log.AppendText(Environment.NewLine + DateTime.Now + ": " + 
                    "Kneeboard Image Full Name " + kneeboardImageFullName);
            richTextBox_log.ScrollToEnd();

            //make the kneeboard export paths
            
            kneeboardExportFolderPath = Path.Combine(dcsSavedGamesFolder,"Kneeboard");
            kneeboardExportFilePath = Path.Combine(kneeboardExportFolderPath, "DCS-WAIFU.lua");
            if (isLogModeEnabled)//supper shorthand for "if true do the next line"
                richTextBox_log.AppendText(Environment.NewLine + DateTime.Now + ": " +
                "You ATIS kneeboard will be exported to " + kneeboardExportFilePath);
            richTextBox_log.ScrollToEnd();

            //make the sound path
            samplesTempLocation = Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, "DCS-WAIFU Sounds");

        }



        string kneeboardExportFolderPath;
        string kneeboardExportFilePath;


        string kneeboardImageFullName;

        public void getTheatreFromLua()
        {

        }

        string hourNow;
        string atisInformationLetter;
        string atisInformationLetterForKneeboard;
        public void getInformationLetter()
        {
            //get ATIS info. Base it on the zulu hour of when the person is playing
            hourNow = System.DateTime.Now.ToString("HH");
            atisInformationLetter = ("Alpha");//init variable
            var arlist1 = new ArrayList();
            arlist1.Add("Alpha");
            arlist1.Add("Bravo");
            arlist1.Add("Charlie");
            arlist1.Add("Delta");
            arlist1.Add("Echo");
            arlist1.Add("Foxtrot");
            arlist1.Add("Golf");
            arlist1.Add("Hotel");
            arlist1.Add("India");
            arlist1.Add("Juliet");
            arlist1.Add("Kilo");
            arlist1.Add("Lima");
            arlist1.Add("Mike");
            arlist1.Add("November");
            arlist1.Add("Oscar");
            arlist1.Add("Papa");
            arlist1.Add("Quebec");
            arlist1.Add("Romeo");
            arlist1.Add("Sierra");
            arlist1.Add("Tango");
            arlist1.Add("Uniform");
            arlist1.Add("Victor");
            arlist1.Add("Whiskey");
            arlist1.Add("X ray");
            arlist1.Add("Yankee");
            arlist1.Add("Zulu");

            switch (hourNow)
            {
                case "00":
                    atisInformationLetter = (string)arlist1[0];
                    atisInformationLetterForKneeboard = (string)arlist1[0].ToString().Substring(0,1);
                    break;
                case "01":
                    atisInformationLetter = (string)arlist1[1];
                    break;
                case "02":
                    atisInformationLetter = (string)arlist1[2];
                    break;
                case "03":
                    atisInformationLetter = (string)arlist1[3];
                    break;
                case "04":
                    atisInformationLetter = (string)arlist1[4];
                    break;
                case "05":
                    atisInformationLetter = (string)arlist1[5];
                    break;
                case "06":
                    atisInformationLetter = (string)arlist1[6];
                    break;
                case "07":
                    atisInformationLetter = (string)arlist1[7];
                    break;
                case "08":
                    atisInformationLetter = (string)arlist1[8];
                    break;
                case "09":
                    atisInformationLetter = (string)arlist1[9];
                    break;
                case "10":
                    atisInformationLetter = (string)arlist1[10];
                    break;
                case "11":
                    atisInformationLetter = (string)arlist1[11];
                    break;
                case "12":
                    atisInformationLetter = (string)arlist1[12];
                    break;
                case "13":
                    atisInformationLetter = (string)arlist1[13];
                    break;
                case "14":
                    atisInformationLetter = (string)arlist1[14];
                    break;
                case "15":
                    atisInformationLetter = (string)arlist1[15];
                    break;
                case "16":
                    atisInformationLetter = (string)arlist1[16];
                    break;
                case "17":
                    atisInformationLetter = (string)arlist1[17];
                    break;
                case "18":
                    atisInformationLetter = (string)arlist1[18];
                    break;
                case "19":
                    atisInformationLetter = (string)arlist1[19];
                    break;
                case "20":
                    atisInformationLetter = (string)arlist1[20];
                    break;
                case "21":
                    atisInformationLetter = (string)arlist1[21];
                    break;
                case "22":
                    atisInformationLetter = (string)arlist1[22];
                    break;
                case "23":
                    atisInformationLetter = (string)arlist1[23];
                    break;
                case "24":
                    atisInformationLetter = (string)arlist1[24];
                    break;
                case "25":
                    atisInformationLetter = (string)arlist1[25];
                    break;
                case "26":
                    atisInformationLetter = (string)arlist1[26];
                    break;
            }
            //this will get the first letter of the chosen word for the kneeboard shorthand
            atisInformationLetterForKneeboard = atisInformationLetter.Substring(0, 1);
            //MessageBox.Show(atisInformationLetterForKneeboard);
        }

        string windDirectionGroundLua;
        double windSpeedGroundLua;
        double metersPerSec_to_knots_multiplier;
        double windSpeedGround;
        string formatedWindsText;
        string windSpeedGroundString;
        PromptBuilder windsFormat = new PromptBuilder();
        public void getWindsFromLua()
        {
            metersPerSec_to_knots_multiplier = 1.94384;
            windSpeedGround = windSpeedGroundLua * metersPerSec_to_knots_multiplier;

            //windSpeedGroundString = ("Calm");

            if (windSpeedGround < 3)//if the wind is less than 3 knts, it will be "calm"
            {

                windSpeedGround = 0;
                windSpeedGroundString = ("Calm");
            }
            else
            {
                windSpeedGroundString = Convert.ToInt32(windSpeedGround).ToString();
            }

            //FROM direction of the wind
            //this gets the correct direction of the weind from dcs. it takes the wind direction and reverses it
            //MessageBox.Show(windDirectionGroundLua.ToString());
            if (Convert.ToInt32(windDirectionGroundLua) <= 180)
            {//note that the string should stay a string, but the math is done va int converters
                windDirectionGroundLua = (Convert.ToInt32(windDirectionGroundLua) + 180).ToString();//math on the string
            }
            else
            {
                windDirectionGroundLua = (Convert.ToInt32(windDirectionGroundLua) - 180).ToString();
            }

            //adds the preceeding 0s so that they are said in the atis
            if (windDirectionGroundLua.ToString().Length == 1)//if the length of the int is 1 (eg directions 0 - 9)
            {//add two leading zeros
                //https://stackoverflow.com/questions/1014292/concatenate-integers-in-c-sharp
                windDirectionGroundLua = "00" + windDirectionGroundLua;
            }
            else if (windDirectionGroundLua.ToString().Length == 2)//if the length of the int is 2 (eg directions 10 -99)
            {
                windDirectionGroundLua = "0" + windDirectionGroundLua;
            }
            //Console.WriteLine(windSpeedGroundString);//conversion from meters per sec to knots
            //https://stackoverflow.com/questions/27630739/system-speech-synthesis-speechsynthesizer-handling-of-numbers



            //this catches the case that the winds are calm. formats the text correctly
            if (windSpeedGroundString == "Calm")
            {
                windsFormat.AppendText(windSpeedGroundString);
                windsFormat.AppendText("at");
                windsFormat.AppendTextWithHint(windDirectionGroundLua.ToString(), SayAs.SpellOut);
                windsFormat.AppendText("degrees");
                windsFormat.ToString();
                //formatedWindsText = ("Winds:" + windSpeedGroundString + " at " + windDirectionGroundLua.ToString() + " degrees");
                //Console.WriteLine("Winds:" + windSpeedGroundString + " at " + windDirectionGroundLua.ToString() + " degrees");
            }
            else
            {
                windsFormat.AppendTextWithHint(windDirectionGroundLua.ToString(), SayAs.SpellOut);
                windsFormat.AppendText("degrees");//or "at"
                windsFormat.AppendText(windSpeedGroundString);//can either spell out or not i guess
                windsFormat.AppendText("knots.");
                windsFormat.ToString();
                //formatedWindsText = ("Winds:" + windDirectionGroundLua.ToString() + " degrees " + windSpeedGroundString + " knots");
                //Console.WriteLine("Winds:" + windDirectionGroundLua.ToString() + " degrees " + windSpeedGroundString + " knots");
            }
            //synth.Speak(talk);
            //talk.ClearContent();
        }

        string formatedVisText;
        string formatedVisSay;
        string formatedVisForKneeboard;
        public void GetVisibilityFromLua()
        {
            if (isFogEnabledLua.Contains("true") && isDustEnabledLua.Contains("true"))//if there is fog and dust enabled, consider them
            {
                minimumVis = Math.Min(generalVisibilityLua, Math.Min(dustVisibilityLua, fogVisibilityLua));
                //Console.WriteLine("There is fog and dust. Minium vis is: " + minimumVis);
            }
            else if (isFogEnabledLua.Contains("false") && isDustEnabledLua.Contains("true"))//fog is off, dust is on
            {
                minimumVis = Math.Min(generalVisibilityLua, dustVisibilityLua);
                //Console.WriteLine("There is dust. Minium vis is: " + minimumVis);
            }
            else if (isFogEnabledLua.Contains("true") && isDustEnabledLua.Contains("false"))//fog is on, dust is off
            {
                minimumVis = Math.Min(generalVisibilityLua, fogVisibilityLua);
                //Console.WriteLine("There is fog. Minium vis is: " + minimumVis);
            }
            else//both dust and fog are off
            {
                minimumVis = generalVisibilityLua;
                //Console.WriteLine("There is no fog or dust. Minium vis is: " + minimumVis);
            }

            //this converts the visibility from metric to official miles based on http://www.moratech.com/aviation/metar-class/metar-pg7-vis.html
            //values are for an automated system
            //i am rounding down for the meter decimals
            //M1/4 (less than quarter mile) = less than 402 meters
            //1/4 = 402 meters
            //603
            //1/2 = 804
            //1005
            //3/4 = 1207
            //1408
            //1 = 1609
            //1810
            //1 1/4 = 2011
            //2212
            //1 1/2 = 2414
            //2615
            //1 3/4 = 2816
            //3017
            //2 = 3218
            //3620
            //2 1/2 = 4023
            //4425
            //3 = 4828
            //5632
            //4 = 6437
            //7241
            //5 = 8046
            //8851
            //6 = 9656
            //	10460
            //7 = 11265
            //12069
            //8 = 12874
            //13679
            //9 = 14484
            //15288
            //10 = 16093
            //more than 10 miles
           

            if (minimumVis > 16093)
            {
                //talk.AppendText("Visibility: Greater than ten miles");
                formatedVisText = ("Visibility: Greater than ten miles");
                formatedVisSay = "Visibility: Greater than ten miles";
                formatedVisForKneeboard = "Vis +10nm";
                //Console.WriteLine("Visibility: Greater than ten miles");
                //synth.Speak(talk);
                //talk.ClearContent();
            }
            else if (minimumVis < 402)
            {
                formatedVisText = ("Visibility: M1/4nm");
                formatedVisSay = "Visibility: Less than one quarter mile";
                formatedVisForKneeboard = "Vis >1/4nm";
            }
            else if (minimumVis >= 402 && minimumVis <= 603)//1/4
            {
                formatedVisText = ("Visibility: 1/4nm");
                formatedVisSay = "Visibility: One quater mile";
                formatedVisForKneeboard = "Vis 1/4nm";
            }
            else if (minimumVis >= 604 && minimumVis <= 1005)//1/2
            {
                formatedVisText = ("Visibility: 1/2nm");
                formatedVisSay = "Visibility: One half mile";
                formatedVisForKneeboard = "Vis 1/2nm";
            }
            else if (minimumVis >= 1006 && minimumVis <= 1408)//3/4
            {
                formatedVisText = ("Visibility: 3/4nm");
                formatedVisSay = "Visibility: Three fourths mile";
                formatedVisForKneeboard = "Vis 3/4nm";
            }
            else if (minimumVis >= 1409 && minimumVis <= 1810)//1
            {
                formatedVisText = ("Visibility: 1nm");
                formatedVisSay = "Visibility: One mile";
                formatedVisForKneeboard = "Vis 1nm";
            }
            else if (minimumVis >= 1811 && minimumVis <= 2212)//1 1/4
            {
                formatedVisText = ("Visibility: 1 1/4mn");
                formatedVisSay = "Visibility: One and 1 fourths miles";
                formatedVisForKneeboard = "Vis 1 1/4mn";
            }
            else if (minimumVis >= 2213 && minimumVis <= 2615)//1 1/2
            {
                formatedVisText = ("Visibility: 1 1/2nm");
                formatedVisSay = "Visibility: One and one half miles";
                formatedVisForKneeboard = "Vis 1 1/2nm";
            }
            else if (minimumVis >= 2616 && minimumVis <= 3017)//1 3/4
            {
                formatedVisText = ("Visibility: 1 3/4nm");
                formatedVisSay = "Visibility: One and three fourths miles";
                formatedVisForKneeboard = "Vis 1 3/4nm";
            }
            else if (minimumVis >= 3018 && minimumVis <= 3620)//2
            {
                formatedVisText = ("Visibility: 2nm");
                formatedVisSay = "Visibility: Two miles";
                formatedVisForKneeboard = "Vis 2nm";
            }
            else if (minimumVis >= 3621 && minimumVis <= 4425)//2 1/2
            {
                formatedVisText = ("Visibility: 2 1/2nm");
                formatedVisSay = "Visibility: Two and one half miles";
                formatedVisForKneeboard = "Vis 2 1/2nm";
            }
            else if (minimumVis >= 4426 && minimumVis <= 5632)//3
            {
                formatedVisText = ("Visibility: 3nm");
                formatedVisSay = "Visibility: Three miles";
                formatedVisForKneeboard = "Vis 3nm";
            }
            else if (minimumVis >= 5633 && minimumVis <= 7241)//4
            {
                formatedVisText = ("Visibility: 4nm");
                formatedVisSay = "Visibility: Four miles";
                formatedVisForKneeboard = "Vis 4nm";
            }
            else if (minimumVis >= 7242 && minimumVis <= 8851)//5
            {
                formatedVisText = ("Visibility: 5nm");
                formatedVisSay = "Visibility: Five miles";
                formatedVisForKneeboard = "Vis 5nm";
            }
            else if (minimumVis >= 8852 && minimumVis <= 10460)//6
            {
                formatedVisText = ("Visibility: 6nm");
                formatedVisSay = "Visibility: Six miles";
                formatedVisForKneeboard = "Vis 6nm";
            }
            else if (minimumVis >= 10461 && minimumVis <= 12069)//7
            {
                formatedVisText = ("Visibility: 7nm");
                formatedVisSay = "Visibility: Seven miles";
                formatedVisForKneeboard = "Vis 7nm";
            }
            else if (minimumVis >= 12070 && minimumVis <= 13679)//8
            {
                formatedVisText = ("Visibility: 8nm");
                formatedVisSay = "Visibility: Eight miles";
                formatedVisForKneeboard = "Vis 8nm";
            }
            else if (minimumVis >= 13680 && minimumVis <= 15288)//9
            {
                formatedVisText = ("Visibility: 9nm");
                formatedVisSay = "Visibility: Nine miles";
                formatedVisForKneeboard = "Vis 9nm";
            }
            else if (minimumVis >= 15289 && minimumVis <= 16093)//10
            {
                formatedVisText = ("Visibility: 10nm");
                formatedVisSay = "Visibility: Ten miles";
                formatedVisForKneeboard = "Vis 10nm";
            }
        }

        string formatedCloudsText;
        string formatedRainOrTstormText;
        string cloudDensityAmountLua;
        int cloudHeightFeetDecimal_rounded_int;
        string cloudDensityAmountforKneeboard;
        string cloudHeightFeetDecimal_rounded_String;
        public void GetCloudsFromLua()
        {
            switch (cloudDensityNumberLua)
            {
                case "0":
                    cloudDensityAmountLua = ("Clear below");
                    cloudDensityAmountforKneeboard = ("CLR below");
                    break;
                case "1":
                    cloudDensityAmountLua = ("Clear below");
                    cloudDensityAmountforKneeboard = ("CLR below");
                    break;
                case "2":
                    cloudDensityAmountLua = ("Clear below");
                    cloudDensityAmountforKneeboard = ("CLR below");
                    break;
                case "3":
                    cloudDensityAmountLua = ("Clouds: Few at");
                    cloudDensityAmountforKneeboard = ("FEW @");
                    break;
                case "4":
                    cloudDensityAmountLua = ("Clouds: Few at");
                    cloudDensityAmountforKneeboard = ("FEW @");
                    break;
                case "5":
                    cloudDensityAmountLua = ("Clouds: Few at");
                    cloudDensityAmountforKneeboard = ("FEW @");
                    break;
                case "6":
                    cloudDensityAmountLua = ("Clouds: Scattered at");
                    cloudDensityAmountforKneeboard = ("SCT @");
                    break;
                case "7":
                    cloudDensityAmountLua = ("Clouds: Scattered at");
                    cloudDensityAmountforKneeboard = ("SCT @");
                    break;
                case "8":
                    cloudDensityAmountLua = ("Clouds: Broken at");
                    cloudDensityAmountforKneeboard = ("BKN @");
                    break;
                case "9":
                    cloudDensityAmountLua = ("Clouds: Overcast at");
                    cloudDensityAmountforKneeboard = ("OVC @");
                    break;
                case "10":
                    cloudDensityAmountLua = ("Clouds: Overcast at");
                    cloudDensityAmountforKneeboard = ("OVC @");
                    break;
            }

            

            Double metersToFeet_conversion = 3.28084;
            double cloudHeightFeet = cloudHeightLua * metersToFeet_conversion;


            //talk.AppendText("Sky conditions:");
            //talk.AppendText(cloudDensityAmountLua);//clounds few at
            //this rounds to the nearest hundred
            cloudHeightFeetDecimal_rounded_int = ((int)Math.Ceiling(cloudHeightFeet / 100.0)) * 100;


            if (cloudHeightFeetDecimal_rounded_int < 10000 && cloudDensityAmountLua.Contains("Clear below"))
            {//this catches the case that the base is set at low and "clear"
                //it effectively says "10k is as high as we are gonna say it is clear"
                cloudHeightFeetDecimal_rounded_int = 10000;
            }

            //make some logic for saying the height of the clouds properly
            //there are a few cases
            //cloud values are already rounded to the nearest hundred

            //Case A - Clouds between 0 and 900 ft
            //Case A solves itself

            //Case B - Clouds between 1,000 and 9,900 feet
            //I want the robot to say digit1 thousand digit2 hundred feet
            //if digit2 is 0, then you dont have to do the hundred part

            cloudHeightFeetDecimal_rounded_String = cloudHeightFeetDecimal_rounded_int.ToString();

            if (cloudHeightFeetDecimal_rounded_String.Length == 4)//case B
            {
                string firstDigit = cloudHeightFeetDecimal_rounded_String.Substring(0, 1);//grabs the first digit and puts it in the string
               
                string secondDigit = cloudHeightFeetDecimal_rounded_String.Substring(1, 1);//grabs the second digit and puts it in the string

                if (secondDigit.Equals("0"))//if the second digit is 0, we have a special case where we dont want to say it.
                {
                    formatedCloudsText = ConvertNumbersToWords(firstDigit) + " thousand";
                }
                else//in the case that the second digit is not zero, which is most cases
                {
                    formatedCloudsText = ConvertNumbersToWords(firstDigit) + " thousand" + ConvertNumbersToWords(secondDigit) + "hundred";
                }
            }

            //Case C - Clouds between 10,000 and 99,900 feet
            //I want the robot to say digit1 digit2 thousand digit3 hundred feet
            //if digit3 is 0, then you dont have to do the hundred part

            if (cloudHeightFeetDecimal_rounded_String.Length == 5)//case C
            {
                string firstDigit = cloudHeightFeetDecimal_rounded_String.Substring(0, 1);//grabs the first digit and puts it in the string

                string secondDigit = cloudHeightFeetDecimal_rounded_String.Substring(1, 1);//grabs the second digit and puts it in the string

                string thirdDigit = cloudHeightFeetDecimal_rounded_String.Substring(2, 1);//grabs the third digit and puts it in the string

                if (thirdDigit.Equals("0"))//if the third digit is 0, we have a special case where we dont want to say it.
                {
                    formatedCloudsText = ConvertNumbersToWords(firstDigit) + " " + ConvertNumbersToWords(secondDigit) + " thousand";//the space is there so the bot does not combine the numbers
                }
                else//in the case that the second digit is not zero, which is most cases
                {
                    formatedCloudsText = ConvertNumbersToWords(firstDigit) + " " + ConvertNumbersToWords(secondDigit) + " thousand" + ConvertNumbersToWords(thirdDigit) + "hundred";//the space is there so the bot does not combine the numbers
                }
                //MessageBox.Show("rounded string is " + cloudHeightFeetDecimal_rounded_String + "\n" + "Formated voice text is " + formatedCloudsText);
            }
            //done with Case A, B, and C



            //formatedCloudsText = (cloudHeightFeetDecimal_rounded_int.ToString() + "feet.");//this is the old way that does not
            //take into accound how the aviation standards say numbers

            //Console.WriteLine("Sky conditions: " + cloudDensityAmountLua + " " + cloudHeightFeetDecimal_rounded_int.ToString() + " feet");
            //synth.Speak(talk);
            //talk.ClearContent();

            if (precipValue == 1)
            {
                //talk.AppendText("Rain in the vicinity");
                //synth.Speak(talk);
                //talk.ClearContent();
                //talk.AppendText("Runway is wet");
                //Console.WriteLine("Rain, wet runway");
                formatedRainOrTstormText = "Rain in the vicinity. Runway is wet.";
            }
            if (precipValue == 2)
            {
                //talk.AppendText("Rain and thunderstorms in the vicinity");
                //synth.Speak(talk);
                //talk.ClearContent();
                //talk.AppendText("Runway is wet");
                //Console.WriteLine("Rain, thunderstorms, wet runway");
                formatedRainOrTstormText = "Rain and thunderstorms in the vicinity. Runway is wet.";
            }
            if (precipValue == 3)
            {
                //talk.AppendText("Rain and thunderstorms in the vicinity");
                //synth.Speak(talk);
                //talk.ClearContent();
                //talk.AppendText("Runway is wet");
                //Console.WriteLine("Rain, thunderstorms, wet runway");
                formatedRainOrTstormText = "Snow in the vicinity. Runway is wet.";
            }

            //synth.Speak(talk);
            //talk.ClearContent();
        }

        int relativeHumidity;
        string dewpointTemperature_string;
        int dewpointTemperature;
        int temperatureActual;
        Double dewpointTemperatureDouble;

        public void GetTempAndDewPointFromLua()
        {
            //if it is raining, the dewpoint equals the temperature

            /*So if there is a cloud layer reported in DCS you could work the formula backwards to get the dewpoint. 
             * For instance temp is 10 and the clouds are at 4000, So take cloud altitude in thousands of feet and take 
             * it times 2.5 to find the expected temp to dewpoint spread, so in this example= 4*2.5=8.5. Now that we have 
             * the temp and the spread, we subtract the spread from temp to get dewpoint, so 10-8.5=1.5 celsius dewpoint. -Snake122*/

            //that equation will be:
            //airTemp - ((cloudAltitude/1000) * 2.5) = dewPoint

            relativeHumidity = 60;//the relative in percent. DCS does not give this number. I made it up. you can look for data 
            //that crossreferences date and temnperature to get the humidity. ED will hopefully have this in the WX update

            //https://www.calculator.net/dew-point-calculator.html?airtemperature=18&airtemperatureunit=celsius&humidity=50&dewpoint=&dewpointunit=celsius&x=32&y=13
            //https://www.eldoradoweather.com/forecast/middleeast/middleeasthumidity.html

            //if there are clouds at a ceiling
            if (cloudHeightFeetDecimal_rounded_int < 10000)//if cloudheight is less than 10000 (i just chose that number kinda randomly)
            {//then calculate the dewpoint off of that
                dewpointTemperatureDouble = temperatureLua - ((cloudHeightFeetDecimal_rounded_int / 1000) * 2.5);
            }
            else
            {//else calculate the dewpoint off of 60% humidity
                dewpointTemperatureDouble = temperatureLua - ((100 - relativeHumidity) / 5);
            }
            

            dewpointTemperature = Convert.ToInt32(dewpointTemperatureDouble);

            //if ["iprecptns"] = 1, it is rain
            //if ["iprecptns"] = 2, it is rain and thunderstorm
            //if ["iprecptns"] = 1, it is snow
            if (precipValue > 0)
            {
                dewpointTemperature = Convert.ToInt32(temperatureLua);//if it is dewpointTemperatureraining, the dewpoint equals the temperature
            }

            temperatureActual = Convert.ToInt32(temperatureLua);//makes sure the temp is an int32

          

            //talk.AppendText("Temperature");
            //talk.AppendTextWithHint(temperatureLua, SayAs.SpellOut);
            //Console.WriteLine("Temperature " + temperatureLua);
            //synth.Speak(talk);
            //talk.ClearContent();
            //talk.AppendText("Dewpoint");
            //talk.AppendTextWithHint(dewpointTemperature_string, SayAs.SpellOut);
            //Console.WriteLine("Dewpoint " + dewpointTemperature_string);
            //synth.Speak(talk);
            //talk.ClearContent();
        }

        string pressureInchesMg;
        string firstTwoInMgDigits;
        string lastTwoInMgDigits;
        int qnhActual;
        int millibarFinal;
        public void GetQnhFromLua()
        {
            //talk.AppendTextWithHint("QNH", SayAs.SpellOut);
            //talk.AppendTextWithHint(qnhLua, SayAs.SpellOut);
            //talk.AppendText("hectopascals");
            //Console.WriteLine("QNH " + qnhLua);
            //synth.Speak(talk);
            //talk.ClearContent();

            Double mmHgToInMg_conversion = 0.0393701;
            Double mmHgToMillibar_conversion = 1.33322;
            pressureInchesMg = (Convert.ToDouble(qnhLua) * mmHgToInMg_conversion).ToString();
            firstTwoInMgDigits = pressureInchesMg.Substring(0, 2);//this could use some error checking...maybe
            lastTwoInMgDigits = pressureInchesMg.Substring(3, 2);//this could use some error checking...maybe
            millibarFinal = Convert.ToInt32(Convert.ToDouble(qnhLua) * mmHgToMillibar_conversion);
            //talk.AppendText("Altimeter");
            //talk.AppendTextWithHint(firstTwoInMgDigits, SayAs.SpellOut);
            //talk.AppendText("decimal");
            //talk.AppendTextWithHint(lastTwoInMgDigits, SayAs.SpellOut);
            ////talk.AppendText("inches");
            //Console.WriteLine("Altimeter " + firstTwoInMgDigits + "." + lastTwoInMgDigits + " inches");
            //synth.Speak(talk);
            //talk.ClearContent();
            qnhActual = Convert.ToInt32(qnhLua);
        }

        PromptBuilder atisBrief = new PromptBuilder();

        string incomingNumber;//this is for the conversion that take place in "ConvertNumbersToWords()"
        //"ConvertNumbersToWords()" takes strings

        string kneeboardAtisString = "";
        //i am making the kneeboard version more realistic to what is actually written, not what was said
        public void generateAtisBrief()
        {
            //test
            //incomingNumber = ConvertNumbersToWords("1007");
            //MessageBox.Show(incomingNumber);
            //test works
            
            //atisBrief.StartVoice(new CultureInfo("en-US"));
            if (isWaifuModeOn)
                atisBrief.StartVoice(new CultureInfo("ja-JP"));//this is how you get the japanese female voice
            //the pair for this is EndVoice
            //Microsoft Haruka Desktop
            //https://docs.microsoft.com/en-us/dotnet/api/system.globalization.cultureinfo?view=net-5.0

            //en-US is usa
            //ja-JP is japan
            //es-ES is spanish
            //zh-TW is chinese Taiwan
            //atisBrief.AppendText("one two three four five six test");
            //atisBrief.AppendText("three  three three four five six test");

            richTextBox_log.AppendText(Environment.NewLine + DateTime.Now + ": " +
                " ATIS Requested ");
            kneeboardAtisString = "";
            richTextBox_log.AppendText(Environment.NewLine +
                "=====ATIS Start=====");
            //kneeboardAtisString = kneeboardAtisString + "=====ATIS Start=====" + "\\n";

            if (runway1.Equals(runway2))
            {
                atisBrief.AppendText(theatre);//cauc
            }
            else
            {
                atisBrief.AppendText(theatre);
            }

            
           

            atisBrief.AppendText("Information");//informatiuon
            atisBrief.AppendText(atisInformationLetter);//lima
          

            richTextBox_log.AppendText(Environment.NewLine +
                theatre + " Information " + atisInformationLetter);
            kneeboardAtisString = kneeboardAtisString + 
                theatre + " " + atisInformationLetterForKneeboard + "\\n";

            atisBrief.AppendBreak();


            atisBrief.AppendText("Winds");//winds
            if (windSpeedGroundString == "Calm")
            {
                //atisBrief.AppendTextWithHint(ConvertNumbersToWords(windSpeedGroundString.ToString()), SayAs.SpellOut);//this spells out every letter. oops
                atisBrief.AppendText(windSpeedGroundString.ToString());
                //atisBrief.AppendText(windSpeedGroundString);
                atisBrief.AppendText(" at ");
                //atisBrief.AppendTextWithHint(ConvertNumbersToWords(windDirectionGroundLua.ToString()), SayAs.SpellOut);
                atisBrief.AppendText(ConvertNumbersToWords(windDirectionGroundLua.ToString()));
                atisBrief.AppendText("degrees");

                richTextBox_log.AppendText(Environment.NewLine +
                "Winds: " + windSpeedGroundString + " at " + windDirectionGroundLua.ToString() + " degrees");
                kneeboardAtisString = kneeboardAtisString +
                 windSpeedGroundString + "@" + windDirectionGroundLua.ToString() +
                "\\n";//removed " degrees" for better irl formating
            }
            else
            {
                //atisBrief.AppendTextWithHint(ConvertNumbersToWords(windDirectionGroundLua.ToString()), SayAs.SpellOut);
                atisBrief.AppendText(ConvertNumbersToWords(windDirectionGroundLua.ToString()));
                atisBrief.AppendText("degrees");//or "at"
                //atisBrief.AppendTextWithHint(ConvertNumbersToWords(windSpeedGroundString), SayAs.SpellOut);//can either spell out or not
                atisBrief.AppendText(ConvertNumbersToWords(windSpeedGroundString.ToString()));
                atisBrief.AppendText("knots");

                //MessageBox.Show(ConvertNumbersToWords(windDirectionGroundLua.ToString()));

                richTextBox_log.AppendText(Environment.NewLine +
                "Winds: " + windDirectionGroundLua.ToString() + " degrees " + windSpeedGroundString + " knots");
                kneeboardAtisString = kneeboardAtisString +
                windDirectionGroundLua.ToString() + "@" + windSpeedGroundString +
                "\\n";//removed " knots" for better irl formating
            }

            atisBrief.AppendBreak();

            if (minimumVis >= 16093)
            {
                //atisBrief.AppendText("Visibility: Greater than ten miles");
                atisBrief.AppendText(formatedVisSay);
                richTextBox_log.AppendText(Environment.NewLine +
                formatedVisText);
                kneeboardAtisString = kneeboardAtisString +
                "Vis +10nm" +
                "\\n";
            }
            else
            {
                //richTextBox_log.AppendText(Environment.NewLine +
                //"Visibility: " + generalVisibilityMiles + " miles");
                atisBrief.AppendText(formatedVisSay);
                richTextBox_log.AppendText(Environment.NewLine +
                formatedVisText);
                kneeboardAtisString = kneeboardAtisString +
                formatedVisForKneeboard +
                "\\n";
            }

            atisBrief.AppendBreak();

            atisBrief.AppendText("Sky conditions: ");
            atisBrief.AppendText(cloudDensityAmountLua);//clounds few at
            atisBrief.AppendText(formatedCloudsText);//123456 (number of feet)
            atisBrief.AppendText("feet");

            string cloudHeightFeetDecimal_rounded_intToString = cloudHeightFeetDecimal_rounded_int.ToString();

            //this is the code that handels the conversion of the 000 to k on the kneeboard
            //if the last three characters of the string are "000"
            if (cloudHeightFeetDecimal_rounded_intToString.Substring(cloudHeightFeetDecimal_rounded_intToString.Length-3, 3).Equals("000"))
            {
                //MessageBox.Show(cloudHeightFeetDecimal_rounded_intToString.Substring(cloudHeightFeetDecimal_rounded_intToString.Length - 3, 3));//shows 000
                //https://stackoverflow.com/questions/15564944/remove-the-last-three-characters-from-a-string
                //remove the last three numbers
                cloudHeightFeetDecimal_rounded_intToString = cloudHeightFeetDecimal_rounded_intToString.Remove(cloudHeightFeetDecimal_rounded_intToString.Length - 3);
                //MessageBox.Show(cloudHeightFeetDecimal_rounded_intToString);//should show "10" in the case of "10000"
                //add a "k"
                cloudHeightFeetDecimal_rounded_intToString = cloudHeightFeetDecimal_rounded_intToString.Insert(cloudHeightFeetDecimal_rounded_intToString.Length, "K");
                //MessageBox.Show(cloudHeightFeetDecimal_rounded_intToString);//should show "10k" in the case of "10000"
            }


            richTextBox_log.AppendText(Environment.NewLine +
                "Sky conditions: " + cloudDensityAmountLua + " " + cloudHeightFeetDecimal_rounded_String +
                " feet");
            kneeboardAtisString = kneeboardAtisString +
                cloudDensityAmountforKneeboard + " " + cloudHeightFeetDecimal_rounded_intToString +
                //" ft" +//removed for irl shorthandness
                "\\n";

            if (precipValue == 1)
            {
                atisBrief.AppendBreak();
                atisBrief.AppendText("Rain in the vicinity:");
                atisBrief.AppendText("Runway is wet");
                richTextBox_log.AppendText(Environment.NewLine +
                "Rain in the vicinity, runway is wet");
                kneeboardAtisString = kneeboardAtisString +
                "Rain, wet rwy" +
                "\\n";
            }

            if (precipValue == 2)
            {
                atisBrief.AppendBreak();
                atisBrief.AppendText("Rain and thunderstorms in the vicinity:");
                atisBrief.AppendText("Runway is wet");
                richTextBox_log.AppendText(Environment.NewLine +
                "Rain and thunderstorms in the vicinity, runway is wet");
                kneeboardAtisString = kneeboardAtisString +
               "Rain, T storm, wet rwy" +
               "\\n";
            }

            if (precipValue == 3)
            {
                atisBrief.AppendBreak();
                atisBrief.AppendText("Snow in the vicinity:");
                richTextBox_log.AppendText(Environment.NewLine +
                "Snow");
                kneeboardAtisString = kneeboardAtisString +
               "Snow" +
               "\\n";
            }


            atisBrief.AppendBreak();

            atisBrief.AppendText("Temperature: ");
            //this is to prevent the synth from saying something like "Temperature hypen one three"
            if (temperatureActual < 0)
            {
                int temperatureActualAbsolute = Math.Abs(temperatureActual);
                atisBrief.AppendText("Minus");
                //atisBrief.AppendTextWithHint(temperatureActualAbsolute.ToString(), SayAs.SpellOut);
                atisBrief.AppendText(ConvertNumbersToWords(temperatureActualAbsolute.ToString()));
            }
            else
            {
                //atisBrief.AppendTextWithHint(temperatureActual.ToString(), SayAs.SpellOut);
                atisBrief.AppendText(ConvertNumbersToWords(temperatureActual.ToString()));
            }


            richTextBox_log.AppendText(Environment.NewLine +
                "Temperature: " + temperatureActual + "C");
            kneeboardAtisString = kneeboardAtisString +
               //"Temp " + 
               temperatureActual +
               "C" +
               //"\\n";
                " / ";
            //i took the "\n" out and added the  "/" for better irl formating
            atisBrief.AppendBreak();

            //this is to prevent the synth from saying something like "dewpoint hypen one eight"
            atisBrief.AppendText("Dewpoint: ");
            if (dewpointTemperature < 0)
            {
                int dewpointTemperatureAbsolute = Math.Abs(dewpointTemperature);
                atisBrief.AppendText("Minus");
                //atisBrief.AppendTextWithHint(dewpointTemperatureAbsolute.ToString(), SayAs.SpellOut);
                atisBrief.AppendText(ConvertNumbersToWords(dewpointTemperatureAbsolute.ToString()));
            }
            else
            {
                //atisBrief.AppendTextWithHint(dewpointTemperature.ToString(), SayAs.SpellOut);
                atisBrief.AppendText(ConvertNumbersToWords(dewpointTemperature.ToString()));
            }

            richTextBox_log.AppendText(Environment.NewLine +
                "Dewpoint: " + dewpointTemperature + "C");
            kneeboardAtisString = kneeboardAtisString +
               //"Dew " + 
               dewpointTemperature +
               "dew" +
               "\\n";

            atisBrief.AppendBreak();

            atisBrief.AppendText("QNH: ");
            //atisBrief.AppendTextWithHint(qnhActual.ToString(), SayAs.SpellOut);
            atisBrief.AppendText(ConvertNumbersToWords(qnhActual.ToString()));
            richTextBox_log.AppendText(Environment.NewLine +
                "QNH: " + qnhActual + " mmHg");
            kneeboardAtisString = kneeboardAtisString +
               qnhActual +
               //"\\n";
               " / ";


            atisBrief.AppendBreak();

            atisBrief.AppendText("Millibar: ");
            //atisBrief.AppendTextWithHint(millibarFinal.ToString(), SayAs.SpellOut);
            atisBrief.AppendText(ConvertNumbersToWords(millibarFinal.ToString()));
            richTextBox_log.AppendText(Environment.NewLine +
                "Millibar: " + millibarFinal + " hPa");
            kneeboardAtisString = kneeboardAtisString +
               millibarFinal +
               //"\\n";
               " / ";


            //i took the "\n" out and added the  "/" for better irl formating
            atisBrief.AppendBreak();

            atisBrief.AppendText("Altimeter: ");
            //atisBrief.AppendTextWithHint(firstTwoInMgDigits, SayAs.SpellOut);
            atisBrief.AppendText(ConvertNumbersToWords(firstTwoInMgDigits.ToString()));
            atisBrief.AppendText("decimal");
            //atisBrief.AppendTextWithHint(lastTwoInMgDigits, SayAs.SpellOut);
            atisBrief.AppendText(ConvertNumbersToWords(lastTwoInMgDigits.ToString()));
            richTextBox_log.AppendText(Environment.NewLine +
                "Altimeter: " + firstTwoInMgDigits + "." + lastTwoInMgDigits + " inHg");
            kneeboardAtisString = kneeboardAtisString +
               firstTwoInMgDigits + "." + lastTwoInMgDigits + 
               "\\n";

            atisBrief.AppendBreak();

            //if there is fog or dust, this will say that it is present
            if (isFogEnabledLua.Contains("true") || isDustEnabledLua.Contains("true"))
            {
                atisBrief.AppendBreak();
                atisBrief.AppendText("Remarks:");
                richTextBox_log.AppendText(Environment.NewLine +
               "Remarks: ");
                kneeboardAtisString = kneeboardAtisString +
               "RMKs";
                atisBrief.AppendBreak();
                if (isFogEnabledLua.Contains("true"))
                {
                    atisBrief.AppendText("Fog");
                    richTextBox_log.AppendText("/Fog ");
                    kneeboardAtisString = kneeboardAtisString +
               "/Fog";
                    atisBrief.AppendBreak();
                }

                if (isDustEnabledLua.Contains("true"))
                {
                    atisBrief.AppendText("Dust");
                    richTextBox_log.AppendText("/Dust ");
                    kneeboardAtisString = kneeboardAtisString +
               "/Dust";
                    atisBrief.AppendBreak();
                }
                kneeboardAtisString = kneeboardAtisString +
               "\\n";//this is the end of the remarks line
            }

            //manpad
            int random_number = new Random().Next(1, 1000);//chose a random number between 1 and 999
            //MessageBox.Show(random_number.ToString());
            if (random_number == 23) //if the number turns out to be 1 https://stackoverflow.com/questions/10079912/c-sharp-probability-and-random-numbers
            {
                //read the file to see if the string "SA-18 Igla manpad" is in it
                string text = (File.ReadAllText(Path.Combine(DcsWaifuSettingsLocation, "mission")));//this works for both MP and SP bc the mission file is uinzipped to the MP location
                if (text.Contains("SA-18 Igla manpad"))
                {
                    atisBrief.AppendText("MANPADS alert. Exercise extreme caution. " +
                        "MANPADS threat reported by Allied Forces in the area. ");
                    richTextBox_log.AppendText(Environment.NewLine + "MANPADS alert. Exercise extreme caution. " +
                        "MANPADS threat reported by Allied Forces in the area. ");
                    kneeboardAtisString = kneeboardAtisString +
                        "/Manpad";
                    atisBrief.AppendBreak();
                    kneeboardAtisString = kneeboardAtisString +
                    "\\n";//this is the end of the remarks line
                }
                
            }
            


            if (runway1.Equals(runway2))//this means that generic WX was selectedc
            {
             //dont do anthing
            }
            else
            {
                atisBrief.AppendText("Landing and departing runway ");
                GetlandingAndDepartingRunway();
                //atisBrief.AppendTextWithHint(runwayToTakeoffFromInt2DigitsToVoice, SayAs.SpellOut);
                atisBrief.AppendText(ConvertNumbersToWords(runwayToTakeoffFromInt2DigitsToVoice.ToString()));
                atisBrief.AppendText("in use");
                richTextBox_log.AppendText(Environment.NewLine +
                "Runway: " + runwayToTakeoffFromInt2DigitsToVoice);
                kneeboardAtisString = kneeboardAtisString +
              "Rwy " + runwayToTakeoffFromInt2DigitsToVoice +
              "\\n";
            }
            

            atisBrief.AppendBreak();

            atisBrief.AppendText("Advise you have information: ");
            atisBrief.AppendText(atisInformationLetter);
            richTextBox_log.AppendText(Environment.NewLine +
                "Advise you have information: " + atisInformationLetter);
            //kneeboardAtisString = kneeboardAtisString +
              //"Advise you have information: " + atisInformationLetterForKneeboard +
              //"\\n";
            richTextBox_log.AppendText(Environment.NewLine +
                "=====ATIS End=====" );
            //kneeboardAtisString = kneeboardAtisString +
              //"=====ATIS End=====" +
              //"\\n";
            richTextBox_log.ScrollToEnd();

            exportWxKneeboardToDcs();

            if (isWaifuModeOn)
                atisBrief.EndVoice();//this is how you end the japanese voice.. the pair for this is StartVoice

            sayAtisBrief();
        }

        
        public void exportWxKneeboardToDcs()
        {
            string[] luaExportString = {
            "dofile(LockOn_Options.common_script_path..\"KNEEBOARD/indicator/definitions.lua\")",
            "SetScale(FOV)",
            "dx  = 50/512",
            //"add_picture(\"pilot_KA50_notepad\",0,0,2,2*GetAspect(),dx,0,1 - 2*dx,1)",
            //"add_picture('E:\\\\Games\\\\DCS Documents\\\\Projects\\\\DCS-Weatherman\\\\customKneeboardPicture.png')",
            "add_picture('" + kneeboardImageFullName + "')",
            //"add_text(\"\",0.05,0.05)",//this is the original version

            "add_text(\"" + "\\n\\n" + kneeboardAtisString + "\",0.05,0.05)",//this is the modified version
            "--Exported via DCS-WAIFU by Bailey " + System.DateTime.Now};

            //this creates the kneeboard folder in saved games if the user did not have one already
            System.IO.Directory.CreateDirectory(kneeboardExportFolderPath);
            

            //this creates the background if there isnt one already
            if (File.Exists(kneeboardImageFile))
            {
                //if the file exists, do nothing
                if (isLogModeEnabled)//supper shorthand for "if true do the next line"
                    richTextBox_log.AppendText(Environment.NewLine + DateTime.Now + ": " +
                    "A customKneeboardPicture.png is already in the correct folder.");
            }
            else
            {
                //the file does not exist, so make the file exist
                if (isLogModeEnabled)//supper shorthand for "if true do the next line"
                    richTextBox_log.AppendText(Environment.NewLine + DateTime.Now + ": " +
                    "A customKneeboardPicture.png is not in the correct folder. Creating new one.");
                //https://stackoverflow.com/questions/16071371/c-sharp-wpf-load-images-from-the-exe-folder

                //https://stackoverflow.com/questions/35804375/how-do-i-save-a-bitmapimage-from-memory-into-a-file-in-wpf-c

                BitmapImage bmp = new BitmapImage(new Uri("pack://application:,,,/Assets/customKneeboardPicture.png"));
                BitmapEncoder encoder = new PngBitmapEncoder();
                encoder.Frames.Add(BitmapFrame.Create(bmp));
                using (var fileStream = new System.IO.FileStream(kneeboardImageFile, System.IO.FileMode.Create))
                {
                    encoder.Save(fileStream);
                }
                //i think that the above 4 lines of code took 3 hours of straight research, trial, and error. whooo!
            }

            try
            {
                System.IO.File.WriteAllLines(kneeboardExportFilePath, luaExportString);
                //https://stackoverflow.com/questions/5920882/file-move-does-not-work-file-already-exists

                richTextBox_log.AppendText(Environment.NewLine + "Kneeboard exported.");
                richTextBox_log.ScrollToEnd();
            }
            catch (IOException h)
            {
                richTextBox_log.AppendText(Environment.NewLine + DateTime.Now + ": " + "Kneeboard not exported.");
                richTextBox_log.AppendText(Environment.NewLine + DateTime.Now + ": " + h.Message);
                richTextBox_log.ScrollToEnd();
            }
        }


        public void sayAtisBrief()
        {

            //synth.SelectVoice("Microsoft Haruka Desktop");
            
            synth.SpeakAsync(atisBrief);

            /*     synth.SelectVoice("Microsoft Haruka Desktop");
             synth.SpeakAsync(atisBrief);*/
        }

        private void syth_SpeakCompleted(object sender, SpeakCompletedEventArgs e)
        {//https://stackoverflow.com/questions/22966534/close-a-form-after-speechsynthesizer-class-instance-stops-speaking

            if (continuous_atis_checkbox.IsChecked ?? true)
            {//https://stackoverflow.com/questions/31734227/wpf-checkbox-check-ischecked
                if (playBrief == true)
                {
                    if (isLogModeEnabled)
                        richTextBox_log.AppendText(Environment.NewLine + DateTime.Now + ": " + "Repeating ATIS");
                    richTextBox_log.ScrollToEnd();
                    sayAtisBrief();
                }
                else
                {
                    //dont do anything bc the user most recently pressed stop
                }
            }
            else
            {
                atisBrief.ClearContent();
            }
            //throw new NotImplementedException();
        }

        string samplesTempLocation; //TODO 9: the link to the sound samples goes here, eventually

        private void requestWxBrief_button_click(object sender, RoutedEventArgs e)
        {

            if (dcsSavedGamesFolder == null)//selection check
            {
                richTextBox_log.AppendText(Environment.NewLine + DateTime.Now + ": " +
                    "Please select your DCS Saved Games Options.lua file." + mizName);
                richTextBox_log.ScrollToEnd();
                return;
            }
            saveSettings();//saved the settings to the json file
            synth.SpeakAsyncCancelAll();//stops the atis broadcast if it was going
            atisBrief.ClearContent();//clears the atis broadcast queue
            /*
            Script: (starts with line 1)
            Hello, my name is Bailey, I will be your weather briefer today. Welcome to 
            [brefierLocation/(5)]. 
            Todays Date is 
            [brefierMonth/(12)], 
            [brefierDay/(31)], 
            [brefierYear/(200)]. 
            Time is now.
            For takeoff winds, are 
            [brefierWindsDegrees/(72)]
            at 
            [brefierknots/(52)]
            knots. Visibility is 
            [brefierVisibility/(14)] 
            miles.
            [brefierRain/(2)]. 
            [brefierThunderstorm/(2)]. 
            There is low level 
            [brefierFogDust/(4)].
            Clouds are 
            [clear/not clear/(2)] 
            [brefiercloudCoverage/(4)] 
            at 
            [brefierCloudBase/(20)].
            Temperature is 
            [brefierTemperature/(45)] 
            degrees. Dewpoint 
            [brefierDewpoint/(60)]. 
            Altimeter is about 
            [brefierAltimeterInHg/(50)]. 
            That's 
            [brefierAltimetermmHg/(50)] 
            milimeters murcury
            or 
            [brefierAltimeterMillibar/(50)] 
            millibars for you internationl folks. Mission weather brief complete.
            Good luck out there.
             */

            //https://stackoverflow.com/questions/3502311/how-to-play-a-sound-in-c-net

            //System.Media.SoundPlayer player = new System.Media.SoundPlayer(@"E:\Music\Good job, Mobius 1!.wav");
            ////player.Play();
            //player.PlaySync();
            //player = new System.Media.SoundPlayer(@"E:\Music\Childish_Gambino_-_This_Is_America.wav");
            ////player.Play();
            //player.PlaySync();

            
            var soundPlayer = new System.Media.SoundPlayer();
            
            //soundPlayer.SoundLocation = @"E:\Music\Good job, Mobius 1!.wav";//is this thing on?
            //soundPlayer.PlaySync();


            soundPlayer.SoundLocation = samplesTempLocation + @"Welcome-1.wav";//intro
            soundPlayer.PlaySync();
            
            soundPlayer.SoundLocation = samplesTempLocation + @"winds.wav";//intro
            soundPlayer.PlaySync();
            
            soundPlayer.SoundLocation = samplesTempLocation + @"120.wav";//intro
            soundPlayer.PlaySync();
          
            soundPlayer.SoundLocation = samplesTempLocation + @"at 12 knots.wav";//intro
            soundPlayer.PlaySync();
            soundPlayer.SoundLocation = samplesTempLocation + @"spacer.wav";//intro
            soundPlayer.PlaySync();
            soundPlayer.SoundLocation = samplesTempLocation + @"temperature.wav";//intro
            soundPlayer.PlaySync();
          
            soundPlayer.SoundLocation = samplesTempLocation + @"9 degrees.wav";//intro
            soundPlayer.PlaySync();
            soundPlayer.SoundLocation = samplesTempLocation + @"spacer.wav";//intro
            soundPlayer.PlaySync();
            soundPlayer.SoundLocation = samplesTempLocation + @"sky conditions.wav";//intro
            soundPlayer.PlaySync();
          
         
            soundPlayer.SoundLocation = samplesTempLocation + @"10 thousand broken.wav";//intro
            soundPlayer.PlaySync();
            soundPlayer.SoundLocation = samplesTempLocation + @"spacer.wav";//intro
            soundPlayer.PlaySync();
            soundPlayer.SoundLocation = samplesTempLocation + @"dust and fog.wav";//intro
            soundPlayer.PlaySync();
            soundPlayer.SoundLocation = samplesTempLocation + @"spacer.wav";//intro
            soundPlayer.PlaySync();
            soundPlayer.SoundLocation = samplesTempLocation + @"vis 2 miles.wav";//intro
            soundPlayer.PlaySync();
            soundPlayer.SoundLocation = samplesTempLocation + @"spacer.wav";//intro
            soundPlayer.PlaySync();
            soundPlayer.SoundLocation = samplesTempLocation + @"altimeter 2880.wav";//intro
            soundPlayer.PlaySync();
            soundPlayer.SoundLocation = samplesTempLocation + @"spacer.wav";//intro
            soundPlayer.PlaySync();
            soundPlayer.SoundLocation = samplesTempLocation + @"brief complete.wav";//intro
            soundPlayer.PlaySync();
        }

        private void stopAtis_button_click(object sender, RoutedEventArgs e)
        {
            playBrief = false;
            synth.SpeakAsyncCancelAll();
            atisBrief.ClearContent();
        }


        string windDirectionGroundLuaStringRounded;
        int windDirectionGroundLuaIntRounded;
        string windDirectionGroundLuaStringRounded2Digits;
        int windDirectionGroundLuaIntRounded2Digits;
        int runwayToTakeoffFromInt2Digits;
        string runwayToTakeoffFromInt2DigitsToVoice;//this is so that the voice says two numbers at all times
        //even leading zeros
        public void GetlandingAndDepartingRunway()
        {//https://stackoverflow.com/questions/43096857/how-to-truncate-or-pad-a-string-to-a-fixed-length-in-c-sharp
            //MessageBox.Show(windDirectionGroundLua.ToString());
            windDirectionGroundLuaIntRounded = Convert.ToInt32(windDirectionGroundLua).RoundOff();//rounds the int
            windDirectionGroundLuaStringRounded = windDirectionGroundLuaIntRounded.ToString();//12
            windDirectionGroundLuaStringRounded2Digits = windDirectionGroundLuaStringRounded.Substring(0, 2);
            windDirectionGroundLuaIntRounded2Digits = Convert.ToInt32(windDirectionGroundLuaStringRounded2Digits);

            

            if (Math.Abs(Convert.ToInt32(runway1)- windDirectionGroundLuaIntRounded2Digits) < 
                Math.Abs(Convert.ToInt32(runway2) - windDirectionGroundLuaIntRounded2Digits))
            {//takeoff runway 1
                runwayToTakeoffFromInt2Digits = Convert.ToInt32(runway1);
            }
            else
            {//takeoff runway 2 if they equal eachother or if the weather is calm (i think)
                runwayToTakeoffFromInt2Digits = Convert.ToInt32(runway2);
            }

            if (runwayToTakeoffFromInt2Digits.ToString().Length < 2)//if the number has less than two digits, add a leading zero
            {
                runwayToTakeoffFromInt2DigitsToVoice = "0" + runwayToTakeoffFromInt2Digits.ToString();
            }
            else//else, the number should already have two digits
            {
                runwayToTakeoffFromInt2DigitsToVoice =runwayToTakeoffFromInt2Digits.ToString();
            }
        }

        private void synthSpeed_slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {//https://stackoverflow.com/questions/14557965/how-to-change-sliders-selection-color-in-wpf
            synth.Rate = (int)synthSpeed_slider.Value;
            synthSpeed_slider.SelectionEnd = synthSpeed_slider.Value;
        }

        private void synthVolume_slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            synth.Volume = (int)synthVolume_slider.Value;
            synthVolume_slider.SelectionEnd = synthVolume_slider.Value;
        }

        public void saveSettings()
        {
            if (continuous_atis_checkbox.IsChecked ?? true)
            {
                isContinuousAtisChecked = true;
            }
            else
            {
                isContinuousAtisChecked = false;
            }


            var my_jsondata = new
            {
                userSelectedFilepath = userSelectedFilepath,
                ContinuousAtisCheckedcheck = isContinuousAtisChecked,
                AtisReadSpeed = synth.Rate,
                AtisVolume = synth.Volume,
                logMode = isLogModeEnabled,
                //FileName = "my_file.zip"
            };

            //Tranform it to Json object
            string json_data = JsonConvert.SerializeObject(my_jsondata);

            System.IO.File.WriteAllText(jsonSaveFileFull, json_data);


            //Parse the json object
            JObject json_object = JObject.Parse(json_data);

        }

        DateTime spFileCreationTime;

        private void getAtisSP_button_click(object sender, RoutedEventArgs e)
        {
            whatButtonDidTheyPress = "theSpButton";
            userPressedSpOrMpButton();
        }

        DateTime mpFileCreationTime;

        private void getAtisMP_button_click(object sender, RoutedEventArgs e)
        {
            whatButtonDidTheyPress = "theMpButton";
            userPressedSpOrMpButton();
        }
        
        
        //runway 1-10 will support up to 10 indivdual runway directions
        string runway1;
        string runway2;
        string runway3;
        string runway4;
        string runway5;
        string runway6;
        string runway7;
        string runway8;
        string runway9;
        string runway10;
        //List<string> airports = new List<int, Tuple<string, int, int>>();


        string selectedAirport;
        //https://www.dotnetperls.com/combobox-wpf
      

        private void airport_comboBox_loaded(object sender, RoutedEventArgs e)
        {
            
        }




        private void airport_comboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
           
            // ... Get the ComboBox.
            var comboBox = sender as ComboBox;
            //do the variabvle aswsigning for the airports here
            //there is a better way to do this, but idk hojw. it uses the "Get" stuff
            selectedAirport = airport_comboBox.SelectedItem as string;
            if (selectedAirport == null) return;
            if (selectedAirport.Contains("General WX"))
            {
                runway1 = "00";
                runway2 = "00";
            }
            else if (selectedAirport.Contains("Anapa"))
            {
                runway1 = "04";
                runway2 = "22";
            }
            else if (selectedAirport.Contains("Batumi"))
            {
                runway1 = "13";
                runway2 = "31";
            }
            else if (selectedAirport.Contains("Beslan"))
            {
                runway1 = "10";
                runway2 = "28";
            }
            else if (selectedAirport.Contains("Gelendzhik"))
            {
                runway1 = "01";
                runway2 = "19";
            }
            else if (selectedAirport.Contains("Gudauta"))
            {
                runway1 = "15";
                runway2 = "33";
            }
            else if (selectedAirport.Contains("Kobuleti"))
            {
                runway1 = "07";
                runway2 = "25";
            }
            else if (selectedAirport.Contains("Krasnodar-Center"))
            {
                runway1 = "09";
                runway2 = "27";
            }
            else if (selectedAirport.Contains("Krasnodar-Pashkovsky"))
            {
                runway1 = "05";
                runway2 = "23";
            }
            else if (selectedAirport.Contains("Krymsk"))
            {
                runway1 = "04";
                runway2 = "22";
            }
            else if (selectedAirport.Contains("Kutaisi"))
            {
                runway1 = "08";
                runway2 = "26";
            }
            else if (selectedAirport.Contains("Maykop"))
            {
                runway1 = "04";
                runway2 = "22";
            }
            else if (selectedAirport.Contains("Mineral'nye Vody"))
            {
                runway1 = "12";
                runway2 = "30";
            }
            else if (selectedAirport.Contains("Mozdok"))
            {
                runway1 = "08";
                runway2 = "26";
            }
            else if (selectedAirport.Contains("Nalchik"))
            {
                runway1 = "06";
                runway2 = "24";
            }
            else if (selectedAirport.Contains("Novorossiysk"))
            {
                runway1 = "04";
                runway2 = "22";
            }
            else if (selectedAirport.Contains("Senaki"))
            {
                runway1 = "09";
                runway2 = "27";
            }
            else if (selectedAirport.Contains("Sochi"))
            {
                runway1 = "06";
                runway2 = "24";
            }
            else if (selectedAirport.Contains("Soganlug"))
            {
                runway1 = "14";
                runway2 = "32";
            }
            else if (selectedAirport.Contains("Sukhumi"))
            {
                runway1 = "12";
                runway2 = "30";
            }
            else if (selectedAirport.Contains("Tblisi"))
            {
                runway1 = "13";
                runway2 = "31";
            }
            else if (selectedAirport.Contains("Vaziani"))
            {
                runway1 = "13";
                runway2 = "31";
            }
            //end of caucasus airports
            //start of pg airports
            else if (selectedAirport.Contains("Aba Musa Island Airport"))
            {
                runway1 = ("08");
                runway2 = ("26");
            }
            else if (selectedAirport.Contains("Abu Dhabi Intl Airport"))
            {
                runway1 = ("13");
                runway2 = ("31");
            }
            else if (selectedAirport.Contains("Al Ain Intl Airport"))
            {
                runway1 = ("01");
                runway2 = ("19");
            }
            else if (selectedAirport.Contains("Al Dhafra AB"))
            {
                runway1 = ("13");
                runway2 = ("31");
            }
            else if (selectedAirport.Contains("Al Maktoum Intl"))
            {
                runway1 = ("12");
                runway2 = ("30");
            }
            else if (selectedAirport.Contains("Al Minhad AB"))
            {
                runway1 = ("09");
                runway2 = ("27");
            }
            else if (selectedAirport.Contains("Al-Bateen Airport"))
            {
                runway1 = ("13");
                runway2 = ("31");
            }
            else if (selectedAirport.Contains("Bandar Abbas Intl"))
            {
                runway1 = ("03");
                runway2 = ("21");
            }
            else if (selectedAirport.Contains("Bandar Lengeh"))
            {
                runway1 = ("08");
                runway2 = ("26");
            }
            else if (selectedAirport.Contains("Bander-e-Jask Airfield"))
            {
                runway1 = ("06");
                runway2 = ("24");
            }
            else if (selectedAirport.Contains("Dubai International"))
            {
                runway1 = ("12");
                runway2 = ("30");
            }
            else if (selectedAirport.Contains("Fujairah Intl"))
            {
                runway1 = ("11");
                runway2 = ("29");
            }
            else if (selectedAirport.Contains("Havadarya"))
            {
                runway1 = ("08");
                runway2 = ("26");
            }
            else if (selectedAirport.Contains("Jiroft Airport"))
            {
                runway1 = ("13");
                runway2 = ("31");
            }
            else if (selectedAirport.Contains("Kerman Airport"))
            {
                runway1 = ("16");
                runway2 = ("34");
            }
            else if (selectedAirport.Contains("Khasab"))
            {
                runway1 = ("01");
                runway2 = ("19");
            }
            else if (selectedAirport.Contains("Kish Intl Airport"))
            {
                runway1 = ("09");
                runway2 = ("27");
            }
            else if (selectedAirport.Contains("Lar Airbase"))
            {
                runway1 = ("09");
                runway2 = ("27");
            }
            else if (selectedAirport.Contains("Lavan Island Airport"))
            {
                runway1 = ("11");
                runway2 = ("29");
            }

            else if (selectedAirport.Contains("Liwa Airbase"))
            {
                runway1 = ("13");
                runway2 = ("31");
            }
            else if (selectedAirport.Contains("Qeshm Island"))
            {
                runway1 = ("05");
                runway2 = ("23");
            }
            else if (selectedAirport.Contains("Ras Al Khaimah"))
            {
                runway1 = ("16");
                runway2 = ("34");
            }
            else if (selectedAirport.Contains("Sas Al Nakheel Airport"))
            {
                runway1 = ("16");
                runway2 = ("34");
            }
            else if (selectedAirport.Contains("Sharjah Intl"))
            {
                runway1 = ("12");
                runway2 = ("30");
            }
            else if (selectedAirport.Contains("Shiraz Intl Airport"))
            {
                runway1 = ("11");
                runway2 = ("29");
            }
            else if (selectedAirport.Contains("Sir Abu Nuayr"))
            {
                runway1 = ("10");
                runway2 = ("28");
            }
            else if (selectedAirport.Contains("Sirri Island"))
            {
                runway1 = ("12");
                runway2 = ("30");
            }
            else if (selectedAirport.Contains("Tunb Island AFB"))
            {
                runway1 = ("03");
                runway2 = ("21");
            }
            else if (selectedAirport.Contains("Tunb Kochak"))
            {
                runway1 = ("08");
                runway2 = ("26");
            }
            //end of pg airports done
            //start of syria
            else if (selectedAirport.Contains("Abu al-Duhur AB"))
            {
                runway1 = ("09");
                runway2 = ("27");
            }
            else if (selectedAirport.Contains("Adana Sakirpasa Airport"))
            {
                runway1 = ("05");
                runway2 = ("23");
            }
            else if (selectedAirport.Contains("Al Qusayr"))
            {
                runway1 = ("10");
                runway2 = ("28");
            }
            else if (selectedAirport.Contains("Al-Dumayr Military Airport"))
            {
                runway1 = ("06");
                runway2 = ("24");
            }
            else if (selectedAirport.Contains("Aleppo Int Airport"))
            {
                runway1 = ("09");
                runway2 = ("27");
            }
            else if (selectedAirport.Contains("An Nasiriyah AB"))
            {
                runway1 = ("04");
                runway2 = ("22");
            }
            else if (selectedAirport.Contains("Bassel Al-Assad Int Airport"))
            {
                runway1 = ("17");
                runway2 = ("35");
            }
            else if (selectedAirport.Contains("Beirut-Rafic Hariri Int"))
            {
                runway1 = ("17");
                runway2 = ("35");
                runway3 = ("03");
                runway4 = ("21");
                runway5 = ("16");
                runway6 = ("34");
            }
            else if (selectedAirport.Contains("Damascus Int Airport"))
            {
                runway1 = ("05");
                runway2 = ("23");
            }
            else if (selectedAirport.Contains("Eyn Shemer"))
            {
                runway1 = ("09");
                runway2 = ("27");
            }
            else if (selectedAirport.Contains("Haifa Airport"))
            {
                runway1 = ("16");
                runway2 = ("34");
            }
            else if (selectedAirport.Contains("Hama Military Airport"))
            {
                runway1 = ("09");
                runway2 = ("27");
            }
            else if (selectedAirport.Contains("Hatay Airport"))
            {
                runway1 = ("04");
                runway2 = ("22");
            }
            else if (selectedAirport.Contains("Incirlik AB"))
            {
                runway1 = ("05");
                runway2 = ("23");
            }
            else if (selectedAirport.Contains("Jirah AB"))
            {
                runway1 = ("10");
                runway2 = ("28");
            }
            else if (selectedAirport.Contains("Khalkhalah AB"))
            {
                runway1 = ("07");
                runway2 = ("25");
                runway3 = ("15");
                runway4 = ("33");
            }
            else if (selectedAirport.Contains("King Hussein Air College"))
            {
                runway1 = ("13");
                runway2 = ("31");
            }
            else if (selectedAirport.Contains("Kiryat Shmona Airport"))
            {
                runway1 = ("03");
                runway2 = ("21");
            }
            else if (selectedAirport.Contains("Kuweires AB"))
            {
                runway1 = ("10");
                runway2 = ("28");
            }
            else if (selectedAirport.Contains("Marj as Sultan Heliport North"))
            {
                runway1 = ("08");
                runway2 = ("26");
            }
            else if (selectedAirport.Contains("Marj as Sultan Heliport South"))
            {
                runway1 = ("09");
                runway2 = ("27");
            }
            else if (selectedAirport.Contains("Marj Ruhayyil AB"))
            {
                runway1 = ("06");
                runway2 = ("24");
            }
            else if (selectedAirport.Contains("Megiddo Airport"))
            {
                runway1 = ("09");
                runway2 = ("27");
            }
            else if (selectedAirport.Contains("Mezzeh Military Airport"))
            {
                runway1 = ("06");
                runway2 = ("24");
            }
            else if (selectedAirport.Contains("Minakh AB"))
            {
                runway1 = ("10");
                runway2 = ("28");
                runway3 = ("04");
                runway4 = ("22");
            }
            else if (selectedAirport.Contains("Palmyra Airport"))
            {
                runway1 = ("08");
                runway2 = ("26");
            }
            else if (selectedAirport.Contains("Qabr as Sitt Heliport"))
            {
                runway1 = ("06");
                runway2 = ("24");
            }
            else if (selectedAirport.Contains("Ramat David AB"))
            {
                runway1 = ("14");
                runway2 = ("32");
                runway3 = ("09");
                runway4 = ("27");
            }
            else if (selectedAirport.Contains("Rayak Air Base"))
            {
                runway1 = ("04");
                runway2 = ("22");
            }
            else if (selectedAirport.Contains("Rene Mouawad AB"))
            {
                runway1 = ("06");
                runway2 = ("24");
            }
            else if (selectedAirport.Contains("Tabqa AB"))
            {
                runway1 = ("09");
                runway2 = ("27");
            }
            else if (selectedAirport.Contains("Taftanaz AB"))
            {
                runway1 = ("10");
                runway2 = ("28");
            }
            else if (selectedAirport.Contains("Wujah Al Hajar AB"))
            {
                runway1 = ("02");
                runway2 = ("20");
            }
            else if (selectedAirport.Contains("H4"))
            {
                runway1 = ("10");
                runway2 = ("28");
            }
            else if (selectedAirport.Contains("Gaziantep"))
            {
                runway1 = ("10");
                runway2 = ("28");
            }
            else if (selectedAirport.Contains("Rosh Pina"))
            {
                runway1 = ("15");
                runway2 = ("33");
            }
            else if (selectedAirport.Contains("Sayqal"))
            {
                runway1 = ("06");
                runway2 = ("24");
                runway3 = ("08");
                runway4 = ("26");
            }
            else if (selectedAirport.Contains("Shayrat"))
            {
                runway1 = ("11");
                runway2 = ("29");
            }
            else if (selectedAirport.Contains("Tiyas"))
            {
                runway1 = ("09");
                runway2 = ("27");
            }
            else if (selectedAirport.Contains("Tha'lah"))
            {
                runway1 = ("05");
                runway2 = ("23");
            }
            else if (selectedAirport.Contains("Naqoura"))
            {
                runway1 = ("06");
                runway2 = ("24");
            }
            //end of syria
            //start of nevada
            else if (selectedAirport.Contains("Beatty Airport"))
            {
                runway1 = ("16");
                runway2 = ("34");
            }
            else if (selectedAirport.Contains("Boulder City Airport"))
            {
                runway1 = ("15");
                runway2 = ("33");
            }
            else if (selectedAirport.Contains("Creech AFB"))
            {
                runway1 = ("08");
                runway2 = ("26");
            }
            else if (selectedAirport.Contains("Echo Bay"))
            {
                runway1 = ("06");
                runway2 = ("24");
            }
            else if (selectedAirport.Contains("Groom Lake AFB"))
            {
                runway1 = ("14");
                runway2 = ("32");
            }
            else if (selectedAirport.Contains("Henderson Executive"))
            {
                runway1 = ("17");
                runway2 = ("35");
            }
            else if (selectedAirport.Contains("Jean Airport"))
            {
                runway1 = ("02");
                runway2 = ("20");
            }
            else if (selectedAirport.Contains("Laughlin Airport"))
            {
                runway1 = ("16");
                runway2 = ("34");
            }
            else if (selectedAirport.Contains("Lincoln County"))
            {
                runway1 = ("17");
                runway2 = ("35");
            }
            else if (selectedAirport.Contains("McCarren Intl Airport"))
            {
                runway1 = ("07");
                runway2 = ("25");
            }
            else if (selectedAirport.Contains("Mesquite"))
            {
                runway1 = ("01");
                runway2 = ("19");
            }
            else if (selectedAirport.Contains("Mina Airport"))
            {
                runway1 = ("13");
                runway2 = ("31");
            }
            else if (selectedAirport.Contains("Nellis AFB"))
            {
                runway1 = ("03");
                runway2 = ("21");
            }
            else if (selectedAirport.Contains("North Las Vegas"))
            {
                runway1 = ("07");
                runway2 = ("25");
            }
            else if (selectedAirport.Contains("Pahute Mesa Airstrip"))
            {
                runway1 = ("18");
                runway2 = ("36");
            }
            else if (selectedAirport.Contains("Tonopah Airport"))
            {
                runway1 = ("11");
                runway2 = ("29");
            }
            else if (selectedAirport.Contains("Tonopah Test Range Airfield"))
            {
                runway1 = ("14");
                runway2 = ("32");
            }
            //end of nevada
            //start of channel
            else if (selectedAirport.Contains("Detling"))
            {
                runway1 = ("05");
                runway2 = ("23");
            }
            else if (selectedAirport.Contains("Hawkinge"))
            {
                runway1 = ("01");
                runway2 = ("19");
                runway3 = ("04");
                runway4 = ("22");
            }
            else if (selectedAirport.Contains("High Halden"))
            {
                runway1 = ("11");
                runway2 = ("29");
                runway3 = ("03");
                runway4 = ("21");
            }
            else if (selectedAirport.Contains("Lympne"))
            {
                runway1 = ("13");
                runway2 = ("31");
                runway3 = ("16");
                runway4 = ("34");
                runway5 = ("02");
                runway6 = ("20");
            }
            else if (selectedAirport.Contains("Manston"))
            {
                runway1 = ("10");
                runway2 = ("28");
            }
            else if (selectedAirport.Contains("Abberville Drucat"))
            {
                runway1 = ("02");
                runway2 = ("20");
                runway3 = ("09");
                runway4 = ("27");
                runway5 = ("13");
                runway6 = ("31");
            }
            else if (selectedAirport.Contains("Dunkirk Mardyck"))
            {
                runway1 = ("08");
                runway2 = ("26");
            }
            else if (selectedAirport.Contains("Merville Calonne"))
            {
                runway1 = ("03");
                runway2 = ("21");
            }
            else if (selectedAirport.Contains("Saint Omer Longuenesse"))
            {
                runway1 = ("08");
                runway2 = ("26");
            }
            //end of channel
            //start of normandy
            else if (selectedAirport.Contains("Argentan"))
            {
                runway1 = ("12");
                runway2 = ("30");
            }
            else if (selectedAirport.Contains("Azeville"))
            {
                runway1 = ("07");
                runway2 = ("25");
            }
            else if (selectedAirport.Contains("Bacqueville"))
            {
                runway1 = ("09");
                runway2 = ("27");
            }
            else if (selectedAirport.Contains("Barville"))
            {
                runway1 = ("10");
                runway2 = ("28");
            }
            else if (selectedAirport.Contains("Bazenville"))
            {
                runway1 = ("05");
                runway2 = ("32");
            }
            else if (selectedAirport.Contains("Beny sur Mer"))
            {
                runway1 = ("17");
                runway2 = ("35");
            }
            else if (selectedAirport.Contains("Beuzeville"))
            {
                runway1 = ("05");
                runway2 = ("23");
            }
            else if (selectedAirport.Contains("Biniville"))
            {
                runway1 = ("14");
                runway2 = ("32");
            }
            else if (selectedAirport.Contains("Brucheville"))
            {
                runway1 = ("07");
                runway2 = ("28");
            }
            else if (selectedAirport.Contains("Cardonville"))
            {
                runway1 = ("15");
                runway2 = ("33");
            }
            else if (selectedAirport.Contains("Carpiquet"))
            {
                runway1 = ("12");
                runway2 = ("30");
            }
            else if (selectedAirport.Contains("Chippelle"))
            {
                runway1 = ("06");
                runway2 = ("24");
            }
            else if (selectedAirport.Contains("Conches"))
            {
                runway1 = ("04");
                runway2 = ("22");
            }
            else if (selectedAirport.Contains("Cretteville"))
            {
                runway1 = ("13");
                runway2 = ("31");
            }
            else if (selectedAirport.Contains("Cricqueville en Bessin"))
            {
                runway1 = ("17");
                runway2 = ("35");
            }
            else if (selectedAirport.Contains("Deux Jumeaux"))
            {
                runway1 = ("10");
                runway2 = ("28");
            }
            else if (selectedAirport.Contains("Essay"))
            {
                runway1 = ("09");
                runway2 = ("27");
            }
            else if (selectedAirport.Contains("Evreux"))
            {
                runway1 = ("16");
                runway2 = ("34");
            }
            else if (selectedAirport.Contains("Goulet"))
            {
                runway1 = ("03");
                runway2 = ("21");
            }
            else if (selectedAirport.Contains("Hauterive"))
            {
                runway1 = ("14");
                runway2 = ("32");
            }
            else if (selectedAirport.Contains("Lantheuil"))
            {
                runway1 = ("06");
                runway2 = ("24");
            }
            else if (selectedAirport.Contains("Le Molay"))
            {
                runway1 = ("04");
                runway2 = ("22");
            }
            else if (selectedAirport.Contains("Lessay"))
            {
                runway1 = ("06");
                runway2 = ("24");
            }
            else if (selectedAirport.Contains("Lignerolles"))
            {
                runway1 = ("11");
                runway2 = ("29");
            }
            else if (selectedAirport.Contains("Longues sur Mer"))
            {
                runway1 = ("12");
                runway2 = ("30");
            }
            else if (selectedAirport.Contains("Maupertus"))
            {
                runway1 = ("10");
                runway2 = ("28");
            }
            else if (selectedAirport.Contains("Meautis"))
            {
                runway1 = ("08");
                runway2 = ("26");
            }
            else if (selectedAirport.Contains("Picauville"))
            {
                runway1 = ("11");
                runway2 = ("29");
            }
            else if (selectedAirport.Contains("Rucqueville"))
            {
                runway1 = ("09");
                runway2 = ("27");
            }
            else if (selectedAirport.Contains("Sainte Croix sur Mer"))
            {
                runway1 = ("09");
                runway2 = ("27");
            }
            else if (selectedAirport.Contains("Sainte Laurent sur Mer"))
            {
                runway1 = ("11");
                runway2 = ("29");
            }
            else if (selectedAirport.Contains("Sainte Pierre Du Mont"))
            {
                runway1 = ("09");
                runway2 = ("27");
            }
            else if (selectedAirport.Contains("Sommervieu"))
            {
                runway1 = ("09");
                runway2 = ("27");
            }
            else if (selectedAirport.Contains("Vrigny"))
            {
                runway1 = ("14");
                runway2 = ("32");
            }
            else if (selectedAirport.Contains("Chailey"))
            {
                runway1 = ("17");
                runway2 = ("35");
            }
            else if (selectedAirport.Contains("Ford AF"))
            {
                runway1 = ("09");
                runway2 = ("27");
            }
            else if (selectedAirport.Contains("Funtington"))
            {
                runway1 = ("01");
                runway2 = ("19");
            }
            else if (selectedAirport.Contains("Needs Oar Point"))
            {
                runway1 = ("16");
                runway2 = ("34");
            }
            else if (selectedAirport.Contains("Tangmere"))
            {
                runway1 = ("11");
                runway2 = ("29");
            }
            //end of normandy

            //turn the selected thing into a string and put it in a variable          
            theatre = comboBox.SelectedItem as string;
            //tell the user that the airport is selected
            DateTime currentTime = DateTime.Now;
            //do the below if the program has been running for more than 5 seconds. you can make this lower if you want
            if (currentTime > programStartTime.AddSeconds(5))
            {
                richTextBox_log.AppendText(Environment.NewLine + DateTime.Now + ": " +
                    theatre + " is selected.");
                richTextBox_log.ScrollToEnd();
            }
            
            if (theatre.Contains("Caucasus: "))
            {
                theatre = theatre.Remove(0, 10);
            }
            else if (theatre.Contains("The Channel: "))
            {
                theatre = theatre.Remove(0, 13);
            }
            else if (theatre.Contains("Syria: "))
            {
                theatre = theatre.Remove(0, 7);
            }
            else if (theatre.Contains("Nevada: "))
            {
                theatre = theatre.Remove(0, 8);
            }
            else if (theatre.Contains("Normandy: "))
            {
                theatre = theatre.Remove(0, 10);
            }
            else if (theatre.Contains("Persian Gulf: "))
            {
                theatre = theatre.Remove(0, 14);
            }
        }

        private void theatre_comboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (theatre_comboBox.SelectedItem.ToString().Contains("General WX"))
            {
                airport_comboBox.ItemsSource = new string[] { "General WX" };
                airport_comboBox.IsEnabled = false;
                
            }
            else if (theatre_comboBox.SelectedItem.ToString().Contains("Caucasus"))
            {
                addCaucasusAirports();
                airport_comboBox.IsEnabled = true;
            }
            else if (theatre_comboBox.SelectedItem.ToString().Contains("The Channel"))
            {
                AddChannelAirports();
                airport_comboBox.IsEnabled = true;
            }
            else if (theatre_comboBox.SelectedItem.ToString().Contains("Syria"))
            {
                AddSyriaAirports();
                airport_comboBox.IsEnabled = true;
            }
            else if (theatre_comboBox.SelectedItem.ToString().Contains("Persian Gulf"))
            {
                AddPersianGulfAirports();
                airport_comboBox.IsEnabled = true;
            }
            else if (theatre_comboBox.SelectedItem.ToString().Contains("Normandy"))
            {
                AddNormandyAirports();
                airport_comboBox.IsEnabled = true;
            }
            else if (theatre_comboBox.SelectedItem.ToString().Contains("Nevada"))
            {
                AddNevadaAirports();
                airport_comboBox.IsEnabled = true;
            }
            airport_comboBox.SelectedIndex = 0;
        }

        private void theatre_comboBox_loaded(object sender, RoutedEventArgs e)
        {
            List<string> treatres = new List<string>();
            //https://stackoverflow.com/questions/14987156/one-key-to-multiple-values-dictionary-in-c
            //var airports = new Dictionary<string, Tuple<string, string, string>>();
            treatres.Add("General WX");
            treatres.Add("Caucasus");
            treatres.Add("The Channel");
            treatres.Add("Syria");
            treatres.Add("Persian Gulf");
            treatres.Add("Normandy");
            treatres.Add("Nevada");

            // ... Get the ComboBox reference.
            var comboBox = sender as ComboBox;

            // ... Assign the ItemsSource to the List.
            comboBox.ItemsSource = treatres;

            // ... Make the first item selected.
            comboBox.SelectedIndex = 0;
        }

        public void addCaucasusAirports()
        {//https://stackoverflow.com/questions/9123822/adding-string-array-to-combo-box
            airport_comboBox.ItemsSource = new string [] { "Anapa", "Batumi", "Gelendzhik",
            "Gudauta", "Kobuleti", "Krasnodar-Center","Krasnodar-Pashkovsky", "Krymsk", "Kutaisi",
                "Maykop", "Mineral'nye Vody", "Mozdok", "Nalchik", "Novorossiysk", "Senaki", "Sochi",
                "Soganlug", "Sukhumi", "Tblisi", "Vaziani"};
        }

        public void AddChannelAirports()
        {//https://stackoverflow.com/questions/9123822/adding-string-array-to-combo-box
            airport_comboBox.ItemsSource = new string[] { "Detling", "Hawkinge", "High Halden",
            "Lympne", "Abberville Drucat", "Dunkirk Mardyck","Merville Calonne", "Saint Omer Longuenesse"};
        }

        public void AddSyriaAirports()
        {//https://stackoverflow.com/questions/9123822/adding-string-array-to-combo-box
            airport_comboBox.ItemsSource = new string[] { "Abu al-Duhur AB", "Adana Sakirpasa Airport", 
                "Al Qusayr", "Al-Dumayr Military Airport", "Aleppo Int Airport", "An Nasiriyah AB", 
                "Bassel Al-Assad Int Airport", "Beirut-Rafic Hariri Int", "Damascus Int Airport", "Eyn Shemer", 
                "Haifa Airport", "Hama Military Airport", "Hatay Airport", "Incirlik AB", "Jirah AB", 
                "Khalkhalah AB", "King Hussein Air College", "Kiryat Shmona Airport", "Kuweires AB", 
                "Marj as Sultan Heliport North", "Marj as Sultan Heliport South", "Marj Ruhayyil AB", 
                "Megiddo Airport", "Mezzeh Military Airport", "Minakh AB", "Palmyra Airport", 
                "Qabr as Sitt Heliport", "Ramat David AB", "Rayak Air Base", "Rene Mouawad AB", 
                "Tabqa AB", "Taftanaz AB", "Wujah Al Hajar AB",
                "H4", "Gaziantep", "Rosh Pina", "Sayqal", "Shayrat", "Tiyas", "Tha'lah", "Naqoura"};//added with DCS v2.7
        }

        public void AddNormandyAirports()
        {//https://stackoverflow.com/questions/9123822/adding-string-array-to-combo-box
            airport_comboBox.ItemsSource = new string[] { "Argentan", "Azeville", "Bacqueville", 
                "Barville", "Bazenville", "Beny sur Mer", "Beuzeville", "Biniville", "Brucheville", 
                "Cardonville", "Carpiquet", "Chippelle", "Conches", "Cretteville", "Cricqueville en Bessin", 
                "Deux Jumeaux", "Essay", "Evreux", "Goulet", "Hauterive", "Lantheuil", "Le Molay", "Lessay",
                "Lignerolles", "Longues sur Mer", "Maupertus", "Meautis", "Picauville", "Rucqueville",
                "Sainte Croix sur Mer", "Sainte Laurent sur Mer", "Sainte Pierre Du Mont", "Sommervieu", 
                "Vrigny", "Chailey", "Ford AF", "Funtington", "Needs Oar Point", "Tangmere" };
        }

        public void AddNevadaAirports()
        {//https://stackoverflow.com/questions/9123822/adding-string-array-to-combo-box
            airport_comboBox.ItemsSource = new string[] { "Beatty Airport", "Boulder City Airport", 
                "Creech AFB", "Echo Bay", "Groom Lake AFB", "Henderson Executive", "Jean Airport", 
                "Laughlin Airport", "Lincoln County", "McCarren Intl Airport", "Mesquite", "Mina Airport", 
                "Nellis AFB", "North Las Vegas", "Pahute Mesa Airstrip", "Tonopah Airport", 
                "Tonopah Test Range Airfield" };
        }

        public void AddPersianGulfAirports()
        {//https://stackoverflow.com/questions/9123822/adding-string-array-to-combo-box
            airport_comboBox.ItemsSource = new string[] { "Aba Musa Island Airport", "Abu Dhabi Intl Airport", 
                "Al Ain Intl Airport", "Al Dhafra AB", "Al Maktoum Intl", "Al Minhad AB", "Al-Bateen Airport ", 
                "Bandar Abbas Intl", "Bandar Lengeh", "Bander-e-Jask Airfield", "Dubai International ", 
                "Fujairah Intl", "Havadarya", "Jiroft Airport", "Kerman Airport", "Khasab", "Kish Intl Airport", 
                "Lar Airbase", "Lavan Island Airport", "Liwa Airbase", "Qeshm Island", "Ras Al Khaimah", 
                "Sas Al Nakheel Airport", "Sharjah Intl", "Shiraz Intl Airport", "Sir Abu Nuayr", "Sirri Island ",
                "Tunb Island AFB", "Tunb Kochak" };
        }

        private void SavedGamesSelectionRectange_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            userChosenPath_textBox.Text = "";
        }

        //cheats
        private void log_label_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {//https://stackoverflow.com/questions/10805134/how-to-clear-text-content-in-richtextbox
            //clears the log when you click the log label
            
            //the L key with clicking the ATIS word will toggle Log mode. hold L
            if (Keyboard.IsKeyDown(Key.L) && isLogModeEnabled == false)//log mode
            {
                richTextBox_log.AppendText(Environment.NewLine + DateTime.Now + ": " + "Log Mode Enabled");
                richTextBox_log.ScrollToEnd();
                isLogModeEnabled = true;
                //showInitLogs();
                saveSettings();
            }
            if (Keyboard.IsKeyDown(Key.O) && isLogModeEnabled == true)//log mode
            {
                richTextBox_log.AppendText(Environment.NewLine + DateTime.Now + ": " + "Log Mode Disabled");
                richTextBox_log.ScrollToEnd();
                isLogModeEnabled = false;
                //showInitLogs();
                saveSettings();
            }
            if (Keyboard.IsKeyDown(Key.C))//clear
            {
                richTextBox_log.Document.Blocks.Clear();
            }
            if (Keyboard.IsKeyDown(Key.R))//reset
            {
                richTextBox_log.Document.Blocks.Clear();
                richTextBox_log.AppendText("Welcome to:");
                richTextBox_log.AppendText(Environment.NewLine + "DCS Weather Atis Information Utility (WAIFU)");
                richTextBox_log.AppendText(Environment.NewLine + "~Bailey");
                richTextBox_log.ScrollToEnd();
            }
            if (Keyboard.IsKeyDown(Key.W))//waifu mode
            {
                richTextBox_log.AppendText(Environment.NewLine + DateTime.Now + ": " + "WAIFU Mode Enabled");
                isWaifuModeOn = true;
            }
        }
        



        private void titleBar_leftButtonDown(object sender, MouseButtonEventArgs e)
        {
            //this moves the window when the titlebar is clicked and held down
            //I made the custom title bar and removed the default one to match the feel of DCS itself
            if (e.ChangedButton == MouseButton.Left)
                DragMove();
        }

        private void closeButton_Click(object sender, RoutedEventArgs e)
        {
            CloseDcsWaifu();
        }

        private void CloseDcsWaifu()
        {
            dispatcherTimer.Stop(); //https://stackoverflow.com/questions/5410430/wpf-timer-like-c-sharp-timer

            if (isLogModeEnabled)//supper shorthand for "if true do the next line"
                richTextBox_log.AppendText(Environment.NewLine + DateTime.Now + ": " + "Closing...");
            richTextBox_log.ScrollToEnd();
            if (File.Exists(missionFileToDelete))
            {
                File.Delete(missionFileToDelete);//cleaning up the file
            }

            if (File.Exists(kneeboardExportFilePath))
            {
                File.Delete(kneeboardExportFilePath);//cleaning up the kneeboard file
            }
            
            System.Windows.Application.Current.Shutdown();
        }

        public void showInitLogs()
        {
            richTextBox_log.AppendText(Environment.NewLine + DateTime.Now + ": " + "This program has been launched from - " + programLocation);
            richTextBox_log.AppendText(Environment.NewLine + DateTime.Now + ": " + "Backupsettings found in - " + jsonSaveFileFull);
            richTextBox_log.AppendText(Environment.NewLine + DateTime.Now + ": " + "Test Result - " + userSelectedFilepath);
            richTextBox_log.AppendText(Environment.NewLine + DateTime.Now + ": " + "Your MP Tracks Folder is - " + mpTrackFolder);
            richTextBox_log.AppendText(Environment.NewLine + DateTime.Now + ": " + "Your SP Tracks Folder is - " + spTrackFolder);
            richTextBox_log.AppendText(Environment.NewLine + DateTime.Now + ": " + "Your DCS Saved Games folder is - " + dcsSavedGamesFolder);
            richTextBox_log.AppendText(Environment.NewLine + DateTime.Now + ": " + "Your MP Tracks Folder is - " + mpTrackFolder);
            richTextBox_log.AppendText(Environment.NewLine + DateTime.Now + ": " + "Your SP Tracks Folder is - " + spTrackFolder);
            richTextBox_log.AppendText(Environment.NewLine + DateTime.Now + ": " +
                            "The DCS folder name version is " + dcsVersionFolderName);
            richTextBox_log.AppendText(Environment.NewLine + DateTime.Now + ": " +
                            "The Temp DCS folder path is " + spTrackFolder);
            richTextBox_log.AppendText(Environment.NewLine + DateTime.Now + ": " + kneeboardImageFullName);
            richTextBox_log.AppendText(Environment.NewLine + DateTime.Now + ": " +
                            "You ATIS kneeboard will be exported to " + kneeboardExportFilePath);
            richTextBox_log.AppendText(Environment.NewLine + DateTime.Now + ": " +
                                "The most recent SP mission file was created - " + spFileCreationTime);
            richTextBox_log.AppendText(Environment.NewLine + DateTime.Now + ": " +
                                "The SP mission file is newer.");
            richTextBox_log.AppendText(Environment.NewLine + DateTime.Now + ": " +
                                "Newest .trk or .miz has been located - " + mizName);
            richTextBox_log.AppendText(Environment.NewLine + DateTime.Now + ": " +
                                "The most recent MP mission file was created - " + mpFileCreationTime);
            richTextBox_log.AppendText(Environment.NewLine + DateTime.Now + ": " +
                                "The MP mission file is newer.");
            richTextBox_log.AppendText(Environment.NewLine + DateTime.Now + ": " +
                                "Newest .trk or .miz has been located - " + mizName);
            richTextBox_log.ScrollToEnd();
        }

        string whatButtonDidTheyPress;
        string correctButtonTrackFolder;
        FileInfo newestFile;
        DateTime spOrMpFileCreationTime;

        public void userPressedSpOrMpButton()
        {
            //make sure you have set the variable "whatButtonDidTheyPress" with "theSpButton" or "theMpButton"
            playBrief = true;//sets the variable to have the brief play later

            //https://stackoverflow.com/questions/10753661/how-to-check-a-var-for-null-value
            if (dcsSavedGamesFolder == null)//selection check
            {
                richTextBox_log.AppendText(Environment.NewLine + DateTime.Now + ": " +
                    "Please select your DCS Saved Games Options.lua file." + mizName);
                richTextBox_log.ScrollToEnd();
                return;
            }

            saveSettings();//saved the settings to the json
            synth.SpeakAsyncCancelAll();//stops the atis broadcast if it was going
            atisBrief.ClearContent();//clears the atis broadcast queue

            if (File.Exists(missionFileToDelete))
            {
                File.Delete(missionFileToDelete);
            }

            //https://stackoverflow.com/questions/1179970/how-to-find-the-most-recent-file-in-a-directory-using-net-and-without-looping
            if (whatButtonDidTheyPress.Contains("theSpButton"))
            {
                correctButtonTrackFolder = spTrackFolder;
                newestFile = GetNewestFile(new DirectoryInfo(spTrackFolder));

            }
            else if (whatButtonDidTheyPress.Contains("theMpButton"))
            {
                correctButtonTrackFolder = mpTrackFolder;
                newestFile = GetNewestFile(new DirectoryInfo(mpTrackFolder));
            }

            //https://stackoverflow.com/questions/23081412/check-if-folder-contains-files-with-certain-extensions
            //https://stackoverflow.com/questions/4366976/how-to-compare-two-files-based-on-datetime

            ////this needs to be an && because if one is true it trips and thinks there are no files
            if (Directory.GetFiles(correctButtonTrackFolder, "*.miz").Length == 0 && Directory.GetFiles(correctButtonTrackFolder, "*.trk").Length == 0)
            {
                if (isLogModeEnabled)//supper shorthand for "if true do the next line"
                    richTextBox_log.AppendText(Environment.NewLine + DateTime.Now + ": " + "No files found.");
                if (isLogModeEnabled)//supper shorthand for "if true do the next line"
                    richTextBox_log.AppendText(Environment.NewLine + DateTime.Now + ": " + "Number of .miz files is " +
                        (Directory.GetFiles(correctButtonTrackFolder, "*.miz").Length.ToString()));
                if (isLogModeEnabled)//supper shorthand for "if true do the next line"
                    richTextBox_log.AppendText(Environment.NewLine + DateTime.Now + ": " + "Number of .trk files is " +
                        (Directory.GetFiles(correctButtonTrackFolder, "*.trk").Length.ToString()));

                if (whatButtonDidTheyPress.Contains("theSpButton"))
                {
                    //a miz or trk isnt detected so tell the user and stop processing stuff
                    richTextBox_log.AppendText(Environment.NewLine + DateTime.Now + ": " +
                        "Please make sure you are in a Single Player mission. No SP mission file was located in  - " + correctButtonTrackFolder);
                }
                else if (whatButtonDidTheyPress.Contains("theMpButton"))
                {
                    richTextBox_log.AppendText(Environment.NewLine + DateTime.Now + ": " +
                      "Please make sure you are in a Multiplayer mission. No MP mission file was located in  - " + correctButtonTrackFolder);
                }
                richTextBox_log.ScrollToEnd();
                return;
            }
            else//yay, it found at least one of the files
            {
                if (isLogModeEnabled)//supper shorthand for "if true do the next line"
                    richTextBox_log.AppendText(Environment.NewLine + DateTime.Now + ": " + "Found .miz or .trk files.");
                if (isLogModeEnabled)//supper shorthand for "if true do the next line"
                    richTextBox_log.AppendText(Environment.NewLine + DateTime.Now + ": " + "Number of .miz files is " +
                        (Directory.GetFiles(correctButtonTrackFolder, "*.miz").Length.ToString()));
                if (isLogModeEnabled)//supper shorthand for "if true do the next line"
                    richTextBox_log.AppendText(Environment.NewLine + DateTime.Now + ": " + "Number of .miz files is " +
                        (Directory.GetFiles(correctButtonTrackFolder, "*.trk").Length.ToString()));
                //continue
            }

            spOrMpFileCreationTime = File.GetLastWriteTime(newestFile.DirectoryName);//.ToString() returned some date from year 1601, haha

            if (isLogModeEnabled)//supper shorthand for "if true do the next line"
                richTextBox_log.AppendText(Environment.NewLine + DateTime.Now + ": " +
                    "The most recent mission file was created - " + spOrMpFileCreationTime);
            richTextBox_log.ScrollToEnd();

            if (whatButtonDidTheyPress.Contains("theSpButton"))
            {
                newestFile = new FileInfo(Path.Combine(correctButtonTrackFolder, "tempMission.miz")); //bc i know that should be the filename
            }

                newestFileName = newestFile;


            if (newestFileName.Name.Contains(".trk") || newestFileName.Name.Contains(".miz"))//you can change this to .miz fort testing
            {
                mizName = newestFileName.FullName;

                if (isLogModeEnabled)//supper shorthand for "if true do the next line"
                    richTextBox_log.AppendText(Environment.NewLine + DateTime.Now + ": " +
                    "Newest .trk or .miz has been located - " + mizName);
                richTextBox_log.ScrollToEnd();

                if (File.Exists(mizName))
                {
                    //unzip
                    using (ZipFile zip = ZipFile.Read(mizName))
                    {
                        ZipEntry p = zip["mission"];//i changed e to p
                        p.Extract(DcsWaifuSettingsLocation, ExtractExistingFileAction.OverwriteSilently);//?
                    }
                }
                else
                {
                    if (whatButtonDidTheyPress.Contains("theSpButton"))
                    {
                        richTextBox_log.AppendText(Environment.NewLine + DateTime.Now + ": " +
                    "Please make sure you are in a Single Player mission. No SP mission file was located at  - " + mizName);
                    richTextBox_log.ScrollToEnd();
                    return;
                    }
                    else if (whatButtonDidTheyPress.Contains("theMpButton"))
                    {
                        richTextBox_log.AppendText(Environment.NewLine + DateTime.Now + ": " +
                            "Please make sure you are in a Multiplayer mission. No MP mission file was located at  - " + mizName);
                        richTextBox_log.ScrollToEnd();
                        return;
                    }
                }

                var missionLuaText = LsonVars.Parse(File.ReadAllText(Path.Combine(DcsWaifuSettingsLocation, "mission")));//?

                if (runway1.Equals(runway2))
                {

                    theatre = missionLuaText["mission"]["theatre"].GetString();//result is "theatre" containing "Syria"
                    //we need to strip the name of the theatre because it contains quotes from above
                    //nevermind, i was just using the wrong thing above. i was using ".ToString()" instead of ".GetString()". oops.
                    //https://stackoverflow.com/questions/3210393/how-do-i-remove-all-non-alphanumeric-characters-from-a-string-except-dash
                    //theatre = new string(theatre.Where(c => char.IsLetterOrDigit(c) || char.IsWhiteSpace(c) || c == '-').ToArray());
                    //idk why the cleaning isnt working. i was double wrong. So, im just gonna make a condition to force it

                    if (theatre.Contains("Caucasus"))
                    {
                        theatre = "Caucasus";
                    }
                    else if (theatre.Contains("Persian"))
                    {
                        theatre = "Persian Gulf";
                    }
                    else if (theatre.Contains("Channel"))
                    {
                        theatre = "Channel";
                    }
                    else if (theatre.Contains("Nevada"))
                    {
                        theatre = "Nevada";
                    }
                    else if (theatre.Contains("Normandy"))
                    {
                        theatre = "Normandy";
                    }
                    else if (theatre.Contains("Syria"))
                    {
                        theatre = "Syria";
                    }
                    else//this is to catch errors or new maps
                    {
                        theatre = "General";
                    }
                }
                else
                {
                    //keep the value that was made via the combo box
                }

                getInformationLetter();//result is "atisInformationLetter" containing "Alpha"

                //--getWindsFromLua---//
                windSpeedGroundLua = missionLuaText["mission"]["weather"]["wind"]["atGround"]["speed"].GetDouble();
                //i have to use the "lenient" version because the normal version gives an error bc it thinks the value is 
                //a int (which it is), but for formating purposes we want it to be a string.
                windDirectionGroundLua = missionLuaText["mission"]["weather"]["wind"]["atGround"]["dir"].GetStringLenient();//https://stackoverflow.com/questions/20375109/how-can-i-convert-a-var-type-to-int
                windDirectionGroundLua = Math.Round(Convert.ToDouble(windDirectionGroundLua),2).ToString();//this makes sure that values like 299.99999 get rounded to a three digit number
                //MessageBox.Show(windDirectionGroundLua.ToString());
                getWindsFromLua();//results
                                  //windSpeedGroundString containing "calm" or "25"
                                  //windDirectionGroundLua containing "123"
                                  //the logic for the completed and formated string is already in the method, but currently commented out

                //---Get Visibility from the Lua file---//
                generalVisibilityLua = missionLuaText["mission"]["weather"]["visibility"]["distance"].GetIntLenient();
                dustVisibilityLua = missionLuaText["mission"]["weather"]["dust_density"].GetIntLenient();
                isFogEnabledLua = missionLuaText["mission"]["weather"]["enable_fog"].ToString();
                fogVisibilityLua = missionLuaText["mission"]["weather"]["fog"]["visibility"].GetIntLenient();
                //in dcs, even if fog is enable, if you set the vsibility slider to 0, fog will not appear
                //this recognizes that and basically says "no, there isnt actually fog" by
                //setting the "isFogEnabledLua" value to "false"
                if (fogVisibilityLua == 0)
                {
                    isFogEnabledLua = "false";
                }
                isDustEnabledLua = missionLuaText["mission"]["weather"]["enable_dust"].ToString();
                GetVisibilityFromLua();
                //result: generalVisibilityMiles that contains the number of miles of vis. the logic for the formating is in the method, but currently commented out

                //---Get Clouds from the Lua file---//
                cloudDensityNumberLua = missionLuaText["mission"]["weather"]["clouds"]["density"].ToString();
                cloudHeightLua = missionLuaText["mission"]["weather"]["clouds"]["base"].GetDouble();
                precipValue = missionLuaText["mission"]["weather"]["clouds"]["iprecptns"].GetInt();
                GetCloudsFromLua();
                //results: cloudDensityAmountLua contains the sky condition
                //cloudHeightFeetDecimal_rounded_int contains the height of the cloud in feet rounded to nearest 100
                //precipValue contains 1 or 2. Rain or rain/thunder
                //the logic for the formating is in the method, but currently commented out

                //---Get Temp and Dewpoint from the Lua file---//
                temperatureLua = missionLuaText["mission"]["weather"]["season"]["temperature"].GetDouble();
                GetTempAndDewPointFromLua();
                //Results
                //temperatureActual contains "18"
                //dewpointTemperature_string contains "8"
                //the logic for the formating is in the method, but currently commented out

                //---Get QNH and inHg from the Lua file---//
                qnhLua = missionLuaText["mission"]["weather"]["qnh"].GetDouble();
                GetQnhFromLua();
                //Results:
                //qnhActual contains "765"
                //firstTwoInMgDigits contains "29"
                //lastTwoInMgDigits contains "92"
                //the logic for the formating is in the method, but currently commented out

                generateAtisBrief();
            }
            else
            {
                richTextBox_log.AppendText(Environment.NewLine + DateTime.Now + ": " +
                    "The newest file isn't a .trk or .miz :(");
                richTextBox_log.ScrollToEnd();
            }
        }
        private void dispatcherTimer_Tick(object sender, EventArgs e)//fires every 1 seconds
        {
            //https://stackoverflow.com/questions/262280/how-can-i-know-if-a-process-is-running
            //if DCS is not running, this program will close
            Process[] pname = Process.GetProcessesByName("DCS");
            if (pname.Length == 0)
            {
                if (closeWaifuAfterDcsClose == true)
                {
                    CloseDcsWaifu();
                }
            }
            else//if DCS is running, the program will continue
            { }
        }


        static string ConvertNumbersToWords(string incomingNumber)
        {
            string additivenumber = "";
            //https://www.dotnetperls.com/switch-char
            foreach (char c in incomingNumber)//this is repeated as many times as there are characters
            {
                switch (c)
                {
                    case'0':
                        additivenumber = additivenumber + "zero ";
                        break;
                    case '1':
                        additivenumber = additivenumber + "one ";
                        break;
                    case '2':
                        additivenumber = additivenumber + "two ";
                        break;
                    case '3':
                        additivenumber = additivenumber + "three ";
                        break;
                    case '4':
                        additivenumber = additivenumber + "four ";
                        break;
                    case '5':
                        additivenumber = additivenumber + "five ";
                        break;
                    case '6':
                        additivenumber = additivenumber + "six ";
                        break;
                    case '7':
                        additivenumber = additivenumber + "seven ";
                        break;
                    case '8':
                        additivenumber = additivenumber + "eight ";
                        break;
                    case '9':
                        additivenumber = additivenumber + "niner ";
                        break;
                }
            }
            incomingNumber = additivenumber;
            return incomingNumber;
        }
    }
}
