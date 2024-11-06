using System.Collections.Generic;
using Aiv.Fast2D;
using OpenTK;
using System.Xml;
using System;

namespace TopDownGame
{
    static class Game
    {
        public static Window Window { get; private set; }
        public static float DeltaTime { get { return Window.DeltaTime; } }

        public static int WindowWidth { get { return Window.Width; } }
        public static int WindowHeight { get { return Window.Height; } }
        public static int HalfWindowWidth { get { return (int)(WindowWidth * 0.5f); } }
        public static int HalfWindowHeight { get { return (int)(WindowHeight * 0.5f); } }

        public static Vector2 StartWindowPosition { get => new Vector2(-9, -38); }

        public static float OrthoWidth { get { return Window.OrthoWidth; } }
        public static float OrthoHeight { get { return Window.OrthoHeight; } }
        public static float OrthoHalfWidth { get { return OrthoWidth * 0.5f; } }
        public static float OrthoHalfHeight { get { return OrthoHeight * 0.5f; } }
        private static float optimalUnitSize;
        private static float optimalScreenHeight;

        public static Vector2 MousePosition { get { return Window.MousePosition; } }
        public static bool IsMouseLeftPressed { get; private set; }
        public static Vector2 LastMousePositionClicked { get; private set; }

        public static Vector2 ScreenCenter { get => new Vector2(OrthoHalfWidth, OrthoHalfHeight); }

        public static Scene CurrentScene;

        public static KeyboardController KeyboardCtrl;

        public static int NumMaxPlayers { get => 1; }

        public static XmlDocument XmlGameConfigDoc { get; private set; }

        public static void Init()
        {
            //init XML nodes and files
            XmlGameConfigDoc = InitXmlDoc("Assets/CONFIG/GameConfig.xml");

            XmlNode windowConfigNode = XmlGameConfigDoc.GetElementsByTagName("windowConfig")[0];

            XmlNode windowSize = windowConfigNode.SelectSingleNode("windowSize");
            XmlNode windowFullScreen = windowConfigNode.SelectSingleNode("fullScreen");

            XmlNode audioVolumeNode = XmlGameConfigDoc.GetElementsByTagName("musicVolume")[0];

            //init window based on settings
            Window = new Window(1920, 1080, "Get The Key And Escape Alive");
            Window.Position = Vector2.Zero;
            Window.SetDefaultViewportOrthographicSize(10);

            Window.SetFullScreen(XmlUtilities.GetBoolAttribute(windowFullScreen, "boolValue"));

            if (XmlUtilities.GetBoolAttribute(windowFullScreen, "boolValue") == true)
            {
                Window.SetResolution(XmlUtilities.GetIntAttribute(windowSize, "width"), XmlUtilities.GetIntAttribute(windowSize, "height"));
                Window.Position = StartWindowPosition;
            }

            Window.SetSize(XmlUtilities.GetIntAttribute(windowSize, "width"), XmlUtilities.GetIntAttribute(windowSize, "height"));

            optimalScreenHeight = 1080;
            optimalUnitSize = optimalScreenHeight / Window.OrthoHeight;

            //sets opened application icon
            Window.SetIcon("Assets/Images/exeIcon.ico");

            InitControllers(NumMaxPlayers);

            //global audio clips init
            SoundMngr.AddClip("mainLoopMusic", "Assets/Audio/Frenk Mochi - The Final Show base with choirs.wav", true);

            //global audio sources init
            SoundEmitter.InitAudioSources(true);

            //init music volume based on settings
            SoundEmitter.GlobalAudioSources["mainLoopMusicSource"] = new SoundEmitter(null, "mainLoopMusic");
            SoundEmitter.GlobalAudioSources["mainLoopMusicSource"].Play(true);

            SoundEmitter.SetAudioVolume(XmlUtilities.GetFloatAttribute(audioVolumeNode, "volumeValue"));

            //loads title scene on game opening
            CurrentScene = new TitleScene();
        }

        public static XmlDocument InitXmlDoc(string path)
        {
            XmlDocument doc = new XmlDocument();

            try
            {
                doc.Load(path);
            }

            catch (XmlException e)
            {
                Console.WriteLine("XML exception: " + e.Message);
            }

            catch (Exception e)
            {
                Console.WriteLine("Generic exception: " + e.Message);
            }

            return doc;
        }

        private static void InitControllers(int numMaxPlayers)
        {
            XmlNodeList playersKeyConfingNodes = XmlGameConfigDoc.GetElementsByTagName("keysConfig")[0].SelectNodes("keys");
            List<KeyCode>[] keysConfig = new List<KeyCode>[playersKeyConfingNodes.Count];

            //the order of keys config is on KeyCodeType enum inside KeyboardController class

            for (int i = 0; i < numMaxPlayers; i++)
            {
                //KEYBOARD CONTROLLER CONFIG
                List<XmlNode> keyNodes = new List<XmlNode>();
                keyNodes.Add(playersKeyConfingNodes[i].SelectSingleNode("interact"));
                keyNodes.Add(playersKeyConfingNodes[i].SelectSingleNode("confirm"));
                keyNodes.Add(playersKeyConfingNodes[i].SelectSingleNode("goBack"));
                keyNodes.Add(playersKeyConfingNodes[i].SelectSingleNode("openPauseMenu"));

                keysConfig[i] = new List<KeyCode>();

                for (int j = 0; j < keyNodes.Count; j++)
                {
                    keysConfig[i].Add((KeyCode)XmlUtilities.GetIntAttribute(keyNodes[j], "value"));
                }

                KeyboardCtrl = new KeyboardController(i, keysConfig[i]);
                //END KEYBOARD CONTROLLER CONFIG
            }
        }

        public static void Run()
        {
            CurrentScene.Start();

            while (Window.IsOpened)
            {
                //System.Console.WriteLine("Mouse X: " + (Window.MouseX) + "\nMouse Y: " + (Window.MouseY));

                if (CurrentScene.IsPlaying)
                {
                    if (Window.MouseLeft)
                    {
                        if (!IsMouseLeftPressed)
                        {
                            LastMousePositionClicked = Window.MousePosition;
                            IsMouseLeftPressed = true;
                        }
                    }

                    else if (IsMouseLeftPressed)
                    {
                        IsMouseLeftPressed = false;
                    }

                    //this won't update the current scene when moving
                    //the window.
                    //I've added the IsFullScreen method editing the Aiv.Fast2D library
                    if (MousePosition == LastMousePositionClicked &&
                        Window.MouseX >= -0.1178782f &&
                        Window.MouseX <= 17.84246f &&
                        Window.MouseY >= -0.476787925f &&
                        Window.MouseY <= -0.01254705f &&
                        IsMouseLeftPressed &&
                        !Window.IsFullScreen())
                    {
                        Window.Update();
                        continue;
                    }

                    CurrentScene.Update();

                    //window update
                    Window.Update();
                }

                else
                {
                    CurrentScene.OnExit();

                    if (CurrentScene.NextScene != null)
                    {
                        CurrentScene = CurrentScene.NextScene;
                        CurrentScene.Start();
                    }

                    else
                    {
                        break;
                    }
                }
            }

            RestoreXmlAttributeValues();
        }

        public static void RestoreXmlAttributeValues()
        {
            if (TileNextSceneLocked.NextSceneLockedNodes != null)
            {
                foreach (var lockedDoorNode in TileNextSceneLocked.NextSceneLockedNodes)
                {
                    lockedDoorNode.Key.Attributes[2].Value = "true";
                    lockedDoorNode.Value.Item1.Save(lockedDoorNode.Value.Item2);
                }
            }

            if (KeyObject.PlayerGotTheKeyNodes != null)
            {
                foreach (var playerGotTheKeyNode in KeyObject.PlayerGotTheKeyNodes)
                {
                    playerGotTheKeyNode.Key.Attributes[2].Value = "false";
                    playerGotTheKeyNode.Value.Item1.Save(playerGotTheKeyNode.Value.Item2);
                }
            }
        }

        public static float PixelsToUnits(float val)
        {
            return val / optimalUnitSize;
        }
    }
}
