using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using OpenTK;

namespace TopDownGame
{
    class DesertScene : Scene
    {
        public Player Player;
        private int playerStartCellX;
        private int playerStartCellY;

        public DesertScene(int playerStartCellX, int playerStartCellY) : base()
        {
            this.playerStartCellX = playerStartCellX;
            this.playerStartCellY = playerStartCellY;
        }

        public override void Start()
        {
            LoadAssets();

            TmxNextScenes = new Dictionary<string, Scene>();

            TmxNextScenes.Add("playerHome", new PlayerHomeScene(30, 27));
            TmxNextScenes.Add("keyScene", new KeyScene(3, 17));
            TmxNextScenes.Add("finalScene", new FinalScene(8, 9));

            InitTiledMap("Assets/MAPS/desertMap.tmx");
            TiledMap.InitTmxObjectNextScenes();

            Player = new Player(0);
            Player.Position = TiledMap.PathFindingMap.Nodes[playerStartCellY, playerStartCellX].Position;
            Player.Agent.CurrentNode = TiledMap.PathFindingMap.Nodes[playerStartCellY, playerStartCellX];

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
            TmxNextScenes.Clear();

            Player = null;
            PauseMenu = null;
            TiledMap.Clear();
            TiledMap = null;
        }
    }
}
