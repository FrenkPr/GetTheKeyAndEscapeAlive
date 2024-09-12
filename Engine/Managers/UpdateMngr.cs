using System.Collections.Generic;

namespace TopDownGame
{
    enum UpdatingType
    {
        Play,
        PauseMenu
    }

    static class UpdateMngr
    {
        private static List<IUpdatable> playUpdatings;
        private static List<IUpdatable> pauseMenuUpdatings;

        static UpdateMngr()
        {
            playUpdatings = new List<IUpdatable>();
            pauseMenuUpdatings = new List<IUpdatable>();
        }

        public static void Add(IUpdatable item, UpdatingType type = UpdatingType.Play)
        {
            if (type == UpdatingType.Play && !playUpdatings.Contains(item))
            {
                playUpdatings.Add(item);
            }

            else if (type == UpdatingType.PauseMenu && !pauseMenuUpdatings.Contains(item))
            {
                pauseMenuUpdatings.Add(item);
            }
        }

        public static void Remove(IUpdatable item, UpdatingType type = UpdatingType.Play)
        {
            if (type == UpdatingType.Play && playUpdatings.Contains(item))
            {
                playUpdatings.Remove(item);
            }

            else if (type == UpdatingType.PauseMenu && pauseMenuUpdatings.Contains(item))
            {
                pauseMenuUpdatings.Remove(item);
            }
        }

        public static void Clear(UpdatingType type)
        {
            if (type == UpdatingType.Play)
            {
                playUpdatings.Clear();
            }

            else
            {
                pauseMenuUpdatings.Clear();
            }
        }

        public static void ClearAll()
        {
            playUpdatings.Clear();
            pauseMenuUpdatings.Clear();
        }

        public static void Update(UpdatingType type)
        {
            if (type == UpdatingType.Play)
            {
                for (int i = 0; i < playUpdatings.Count; i++)
                {
                    playUpdatings[i].Update();
                }
            }

            else
            {
                for (int i = 0; i < pauseMenuUpdatings.Count; i++)
                {
                    pauseMenuUpdatings[i].Update();
                }
            }
        }
    }
}
