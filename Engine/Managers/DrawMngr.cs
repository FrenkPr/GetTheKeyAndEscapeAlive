using System.Collections.Generic;

namespace TopDownGame
{
    enum DrawLayer
    {
        Background,
        MiddleGround,
        Playground,
        Foreground,
        GUI,
        Length
    }

    enum DrawingType
    {
        Play,
        PauseMenu
    }

    static class DrawMngr
    {
        private static List<IDrawable>[] playDrawings;
        private static List<IDrawable>[] pauseMenuDrawings;

        static DrawMngr()
        {
            playDrawings = new List<IDrawable>[(int)DrawLayer.Length];
            pauseMenuDrawings = new List<IDrawable>[(int)DrawLayer.Length];

            for (int i = 0; i < playDrawings.Length; i++)
            {
                playDrawings[i] = new List<IDrawable>();
                pauseMenuDrawings[i] = new List<IDrawable>();
            }
        }

        public static void Add(IDrawable item, DrawingType type = DrawingType.Play)
        {
            if (type == DrawingType.Play && !playDrawings[(int)item.DrawLayer].Contains(item))
            {
                playDrawings[(int)item.DrawLayer].Add(item);
            }

            else
            {
                if (!pauseMenuDrawings[(int)item.DrawLayer].Contains(item))
                {
                    pauseMenuDrawings[(int)item.DrawLayer].Add(item);
                }
            }
        }

        public static void Remove(IDrawable item, DrawingType type = DrawingType.Play)
        {
            if (type == DrawingType.Play && playDrawings[(int)item.DrawLayer].Contains(item))
            {
                playDrawings[(int)item.DrawLayer].Remove(item);
            }

            else
            {
                if (pauseMenuDrawings[(int)item.DrawLayer].Contains(item))
                {
                    pauseMenuDrawings[(int)item.DrawLayer].Remove(item);
                }
            }
        }

        public static void Clear(DrawingType type)
        {
            if (type == DrawingType.Play)
            {
                for (int i = 0; i < playDrawings.Length; i++)
                {
                    playDrawings[i].Clear();
                }
            }

            else
            {
                for (int i = 0; i < pauseMenuDrawings.Length; i++)
                {
                    pauseMenuDrawings[i].Clear();
                }
            }
        }

        public static void ClearAll()
        {
            for (int i = 0; i < playDrawings.Length; i++)
            {
                playDrawings[i].Clear();
                pauseMenuDrawings[i].Clear();
            }
        }

        public static void Draw(DrawingType type)
        {
            if (type == DrawingType.Play)
            {
                for (int i = 0; i < playDrawings.Length; i++)
                {
                    for (int j = 0; j < playDrawings[i].Count; j++)
                    {
                        playDrawings[i][j].Draw();
                    }
                }
            }

            else
            {
                for (int i = 0; i < pauseMenuDrawings.Length; i++)
                {
                    for (int j = 0; j < pauseMenuDrawings[i].Count; j++)
                    {
                        pauseMenuDrawings[i][j].Draw();
                    }
                }
            }
        }
    }
}
