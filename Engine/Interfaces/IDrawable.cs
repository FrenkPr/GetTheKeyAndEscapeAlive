

namespace TopDownGame
{
    interface IDrawable
    {
        DrawLayer DrawLayer { get; }

        void Draw();
    }
}
