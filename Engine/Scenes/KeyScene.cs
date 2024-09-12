using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TopDownGame
{
    class KeyScene : Scene
    {
        public Player Player;
        private KeyObject key;
        private int playerStartCellX;
        private int playerStartCellY;

        public KeyScene(int playerStartCellX, int playerStartCellY) : base()
        {
            this.playerStartCellX = playerStartCellX;
            this.playerStartCellY = playerStartCellY;
        }

        protected override void LoadAssets()
        {
            base.LoadAssets();

            TextureMngr.AddTexture("key", "Assets/SPRITES/ITEMS/item8BIT_key.png");
        }

        public override void Start()
        {
            LoadAssets();

            TmxNextScenes = new Dictionary<string, Scene>();

            TmxNextScenes.Add("desertScene", new DesertScene(116, 36));

            InitTiledMap("Assets/MAPS/keyMap.tmx");
            TiledMap.InitTmxObjectNextScenes();

            Player = new Player(0);
            Player.Position = TiledMap.PathFindingMap.Nodes[playerStartCellY, playerStartCellX].Position;
            Player.Agent.CurrentNode = TiledMap.PathFindingMap.Nodes[playerStartCellY, playerStartCellX];

            key = new KeyObject("Assets/MAPS/finalMap.tmx", "endingSceneKeyDoor");
            key.Position = TiledMap.PathFindingMap.Nodes[18, 36].Position;

            CameraMngr.SetTarget(Player, false);
            CameraMngr.MainCamera.position = Player.Position;

            base.Start();
        }

        public override void Update()
        {
            base.Update();

            if (PauseMenu.IsActive)
            {
                return;
            }

            Player.Input();

            //update
            PhysicsMngr.Update();
            UpdateMngr.Update(UpdatingType.Play);
            CameraMngr.Update();

            PhysicsMngr.CheckCollisions();

            //draw
            DrawMngr.Draw(DrawingType.Play);
            DebugMngr.Draw();
        }

        public override void OnExit()
        {
            UpdateMngr.ClearAll();
            PhysicsMngr.ClearAll();
            FontMngr.ClearAll();
            DrawMngr.ClearAll();
            TextureMngr.ClearAll();
            DebugMngr.ClearAll();
            OnChangeWindowSizeMngr.ClearAll();
            TmxNextScenes.Clear();

            Player = null;
            PauseMenu = null;
            TiledMap.Clear();
            TiledMap = null;
        }
    }
}
